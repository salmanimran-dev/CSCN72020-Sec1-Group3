using System.Globalization;
using HCRMS.Models;
using Microsoft.Data.Sqlite;

namespace HCRMS.Data
{
    // Read-only alarm record shown in the Alarm History page (NFR-05)
    public record AlarmRecord(long AlarmID, DateTime Timestamp, string RoomID, string DeviceID,
                              double? Value, string Level, bool Acknowledged, DateTime? AcknowledgedAt);

    // P17 - The only class that talks to SQLite (IR-04)
    // Tables: Rooms, Devices, Thresholds, Readings, Alarms and Events
    // If the database is locked, writes wait in memory and are saved later (NFR-04)
    // There are no methods to edit or delete logged records (NFR-05, NFR-09)
    public class DatabaseManager
    {
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

        // Stops memory growing forever if the database stays down
        private const int MaxPendingWrites = 50000;

        private readonly string connectionString;
        private readonly Queue<WriteOperation> pendingWrites = new Queue<WriteOperation>();
        private readonly object sync = new object();
        private bool schemaReady;

        public string DatabasePath { get; }

        // Description of the last database problem, or null if there is none
        public string? LastError { get; private set; }

        // Number of records waiting to be saved
        public int PendingWriteCount
        {
            get { lock (sync) { return pendingWrites.Count; } }
        }

        public DatabaseManager(string databasePath, int busyTimeoutSeconds = 5)
        {
            DatabasePath = databasePath;

            try
            {
                string? folder = Path.GetDirectoryName(Path.GetFullPath(databasePath));
                if (!string.IsNullOrEmpty(folder))
                    Directory.CreateDirectory(folder);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                LastError = $"Could not create the database folder: {ex.Message}";
            }

            connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false,
                DefaultTimeout = busyTimeoutSeconds
            }.ToString();
        }

        // Default location: %LocalAppData%\HCRMS\hcrms.db
        public static string DefaultDatabasePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appData, "HCRMS", "hcrms.db");
        }

        // Creates the tables if needed. Returns false if the database can't be opened.
        public bool Initialize()
        {
            try
            {
                using SqliteConnection connection = Open();
                return true;
            }
            catch (Exception ex) when (IsDatabaseError(ex))
            {
                LastError = $"Could not open the database: {ex.Message}";
                return false;
            }
        }

        // =====================================
        // Rooms, devices and thresholds
        // =====================================

        public bool SaveRoom(Room room)
        {
            return Write(new WriteOperation(
                @"INSERT INTO Rooms (RoomID, RoomType) VALUES ($room, $type)
                  ON CONFLICT(RoomID) DO UPDATE SET RoomType = excluded.RoomType;",
                ("$room", room.RoomID), ("$type", room.RoomType)));
        }

        public bool SaveDevice(string roomId, Sensor sensor, string deviceType)
        {
            return Write(new WriteOperation(
                @"INSERT INTO Devices (RoomID, DeviceID, DeviceType, Unit)
                  VALUES ($room, $device, $type, $unit)
                  ON CONFLICT(RoomID, DeviceID) DO UPDATE
                  SET DeviceType = excluded.DeviceType, Unit = excluded.Unit;",
                ("$room", roomId), ("$device", sensor.DeviceID), ("$type", deviceType), ("$unit", sensor.Unit)));
        }

        // Saves the sensor's default limits, without overwriting limits already saved
        public bool SaveDefaultThreshold(string roomId, Sensor sensor)
        {
            return Write(new WriteOperation(
                @"INSERT OR IGNORE INTO Thresholds (RoomID, DeviceID, WarningLimit, CriticalLimit, UpdatedAt)
                  VALUES ($room, $device, $warn, $crit, $at);",
                ("$room", roomId), ("$device", sensor.DeviceID), ("$warn", sensor.WarningLimit),
                ("$crit", sensor.CriticalLimit), ("$at", Format(DateTime.Now))));
        }

        // =====================================
        // Readings (FR-17)
        // =====================================

        // Saves one reading for each sensor in the room
        public bool LogReadings(string roomId, IEnumerable<Sensor> sensors, DateTime timestamp)
        {
            List<WriteOperation> operations = new List<WriteOperation>();

            foreach (Sensor sensor in sensors)
            {
                // A failed sensor has no valid value, so save NULL instead of a made-up number
                double? value = null;
                if (sensor.Status != "Failed" && double.IsFinite(sensor.CurrentValue))
                    value = sensor.CurrentValue;

                operations.Add(new WriteOperation(
                    @"INSERT INTO Readings (Timestamp, RoomID, DeviceID, Value, Unit, Status)
                      VALUES ($at, $room, $device, $value, $unit, $status);",
                    ("$at", Format(timestamp)), ("$room", roomId), ("$device", sensor.DeviceID),
                    ("$value", value), ("$unit", sensor.Unit), ("$status", sensor.Status)));
            }

            return Write(operations);
        }

        // =====================================
        // Alarms (US-3)
        // =====================================

        // Returns the new AlarmID, or null if the database was unavailable
        // (the alarm is then kept in memory and saved later)
        public long? LogAlarm(DateTime timestamp, string roomId, string deviceId, double? value, string level)
        {
            WriteOperation insert = new WriteOperation(
                @"INSERT INTO Alarms (Timestamp, RoomID, DeviceID, Value, Level)
                  VALUES ($at, $room, $device, $value, $level)
                  RETURNING AlarmID;",
                ("$at", Format(timestamp)), ("$room", roomId), ("$device", deviceId),
                ("$value", value), ("$level", level));

            lock (sync)
            {
                // Older records waiting in memory are saved first, so the history stays in order
                if (!FlushLocked())
                {
                    Enqueue(insert);
                    return null;
                }

                try
                {
                    using SqliteConnection connection = Open();
                    using SqliteCommand command = CreateCommand(connection, null, insert);
                    return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
                }
                catch (SqliteException ex) when (IsTemporary(ex))
                {
                    LastError = $"Database busy, alarm will be saved later: {ex.Message}";
                    Enqueue(insert);
                    return null;
                }
                catch (Exception ex) when (IsDatabaseError(ex))
                {
                    LastError = $"Alarm could not be saved: {ex.Message}";
                    return null;
                }
            }
        }

        // Saves when an alarm was acknowledged (FR-07)
        public bool AcknowledgeAlarm(long alarmId, DateTime acknowledgedAt)
        {
            return Write(new WriteOperation(
                @"UPDATE Alarms SET Acknowledged = 1, AcknowledgedAt = $at
                  WHERE AlarmID = $id AND Acknowledged = 0;",
                ("$at", Format(acknowledgedAt)), ("$id", alarmId)));
        }

        // Alarm history for one room, newest first
        public List<AlarmRecord> GetAlarms(string roomId, int limit)
        {
            return Query(
                @"SELECT AlarmID, Timestamp, RoomID, DeviceID, Value, Level,
                         Acknowledged, AcknowledgedAt
                  FROM Alarms WHERE RoomID = $room
                  ORDER BY Timestamp DESC, AlarmID DESC LIMIT $limit;",
                r => new AlarmRecord(
                    r.GetInt64(0), Parse(r.GetString(1)), r.GetString(2), r.GetString(3),
                    r.IsDBNull(4) ? null : r.GetDouble(4), r.GetString(5), r.GetInt64(6) == 1,
                    r.IsDBNull(7) ? null : Parse(r.GetString(7))),
                ("$room", roomId), ("$limit", limit));
        }

        // =====================================
        // Events
        // =====================================

        // Saves an event such as start-up or an acknowledgement
        public bool LogEvent(string? roomId, string? deviceId, string eventType,
                             string description, string userRole)
        {
            return Write(new WriteOperation(
                @"INSERT INTO Events (Timestamp, RoomID, DeviceID, EventType, Description, UserRole)
                  VALUES ($at, $room, $device, $type, $desc, $role);",
                ("$at", Format(DateTime.Now)), ("$room", roomId), ("$device", deviceId),
                ("$type", eventType), ("$desc", description), ("$role", userRole)));
        }

        // Tries to save anything still waiting in memory
        public bool FlushPending()
        {
            lock (sync) { return FlushLocked(); }
        }

        // =====================================
        // Internals
        // =====================================

        private const string SchemaSql = @"
CREATE TABLE IF NOT EXISTS Rooms (
    RoomID   TEXT PRIMARY KEY,
    RoomType TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Devices (
    RoomID     TEXT NOT NULL,
    DeviceID   TEXT NOT NULL,
    DeviceType TEXT NOT NULL,
    Unit       TEXT,
    PRIMARY KEY (RoomID, DeviceID)
);

CREATE TABLE IF NOT EXISTS Thresholds (
    RoomID        TEXT NOT NULL,
    DeviceID      TEXT NOT NULL,
    WarningLimit  REAL NOT NULL,
    CriticalLimit REAL NOT NULL,
    UpdatedAt     TEXT NOT NULL,
    PRIMARY KEY (RoomID, DeviceID)
);

CREATE TABLE IF NOT EXISTS Readings (
    ReadingID INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT NOT NULL,
    RoomID    TEXT NOT NULL,
    DeviceID  TEXT NOT NULL,
    Value     REAL,
    Unit      TEXT,
    Status    TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Alarms (
    AlarmID        INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp      TEXT NOT NULL,
    RoomID         TEXT NOT NULL,
    DeviceID       TEXT NOT NULL,
    Value          REAL,
    Level          TEXT NOT NULL,
    Acknowledged   INTEGER NOT NULL DEFAULT 0,
    AcknowledgedAt TEXT
);

CREATE TABLE IF NOT EXISTS Events (
    EventID     INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp   TEXT NOT NULL,
    RoomID      TEXT,
    DeviceID    TEXT,
    EventType   TEXT NOT NULL,
    Description TEXT NOT NULL,
    UserRole    TEXT NOT NULL
);
";

        // One INSERT or UPDATE waiting to be saved
        private class WriteOperation
        {
            public string Sql { get; }
            public (string Name, object? Value)[] Parameters { get; }

            public WriteOperation(string sql, params (string Name, object? Value)[] parameters)
            {
                Sql = sql;
                Parameters = parameters;
            }
        }

        // Opens a connection and creates the tables the first time
        private SqliteConnection Open()
        {
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();
                if (!schemaReady)
                {
                    using SqliteCommand command = connection.CreateCommand();
                    command.CommandText = SchemaSql;
                    command.ExecuteNonQuery();
                    schemaReady = true;
                }
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        private static SqliteCommand CreateCommand(SqliteConnection connection, SqliteTransaction? transaction,
                                                   WriteOperation operation)
        {
            SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = operation.Sql;
            foreach ((string name, object? value) in operation.Parameters)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }
            return command;
        }

        private bool Write(params WriteOperation[] operations)
        {
            return Write((IReadOnlyList<WriteOperation>)operations);
        }

        private bool Write(IReadOnlyList<WriteOperation> operations)
        {
            lock (sync)
            {
                foreach (WriteOperation operation in operations)
                {
                    Enqueue(operation);
                }
                return FlushLocked();
            }
        }

        private void Enqueue(WriteOperation operation)
        {
            pendingWrites.Enqueue(operation);
            while (pendingWrites.Count > MaxPendingWrites)
            {
                pendingWrites.Dequeue();
            }
        }

        // Saves everything waiting in memory in one transaction
        private bool FlushLocked()
        {
            if (pendingWrites.Count == 0)
                return true;

            try
            {
                using SqliteConnection connection = Open();
                using SqliteTransaction transaction = connection.BeginTransaction();
                foreach (WriteOperation operation in pendingWrites)
                {
                    using SqliteCommand command = CreateCommand(connection, transaction, operation);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();

                pendingWrites.Clear();
                LastError = null;
                return true;
            }
            catch (SqliteException ex) when (!IsTemporary(ex))
            {
                // One record is bad. Save the rest one by one so it can't block logging.
                LastError = $"A record could not be saved: {ex.Message}";
                return SaveIndividually();
            }
            catch (Exception ex) when (IsDatabaseError(ex))
            {
                // Database locked or unavailable: keep everything in memory
                LastError = $"Database unavailable, {pendingWrites.Count} record(s) waiting: {ex.Message}";
                return false;
            }
        }

        private bool SaveIndividually()
        {
            try
            {
                using SqliteConnection connection = Open();
                while (pendingWrites.Count > 0)
                {
                    WriteOperation operation = pendingWrites.Peek();
                    try
                    {
                        using SqliteCommand command = CreateCommand(connection, null, operation);
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException ex) when (!IsTemporary(ex))
                    {
                        // This record can never be saved, so drop it
                    }
                    pendingWrites.Dequeue();
                }
                return true;
            }
            catch (Exception ex) when (IsDatabaseError(ex))
            {
                LastError = $"Database unavailable, {pendingWrites.Count} record(s) waiting: {ex.Message}";
                return false;
            }
        }

        private List<T> Query<T>(string sql, Func<SqliteDataReader, T> map,
                                 params (string Name, object? Value)[] parameters)
        {
            List<T> results = new List<T>();
            try
            {
                lock (sync)
                {
                    using SqliteConnection connection = Open();
                    using SqliteCommand command = CreateCommand(connection, null, new WriteOperation(sql, parameters));
                    using SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        results.Add(map(reader));
                    }
                }
            }
            catch (Exception ex) when (IsDatabaseError(ex))
            {
                LastError = $"Could not read from the database: {ex.Message}";
            }
            return results;
        }

        // Locked, busy, read-only, disk or file errors that may go away if we try again
        private static bool IsTemporary(SqliteException ex)
        {
            int code = ex.SqliteErrorCode & 0xFF;
            return code == 5 || code == 6 || code == 8 || code == 10 || code == 13 || code == 14;
        }

        private static bool IsDatabaseError(Exception ex)
        {
            return ex is SqliteException || ex is IOException ||
                   ex is UnauthorizedAccessException || ex is InvalidOperationException;
        }

        private static string Format(DateTime value)
        {
            return value.ToString(TimestampFormat, CultureInfo.InvariantCulture);
        }

        private static DateTime Parse(string value)
        {
            return DateTime.ParseExact(value, TimestampFormat, CultureInfo.InvariantCulture);
        }
    }
}
