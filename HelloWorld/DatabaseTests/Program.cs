using HCRMS;
using HCRMS.Core;
using HCRMS.Data;
using HCRMS.Models;
using Microsoft.Data.Sqlite;

// =====================================
// P17 - Database and dashboard logic tests
// =====================================

// Each run uses a new database file in the temp folder

int passed = 0;
int failed = 0;

// A simple helper to report test results.
void Check(string name, bool condition)
{
    if (condition)
    {
        Console.WriteLine($"PASS: {name}");
        passed++;
    }
    else
    {
        Console.WriteLine($"FAIL: {name}");
        failed++;
    }
}

string tempFolder = Path.Combine(Path.GetTempPath(), "hcrms-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(tempFolder);
string dbPath = Path.Combine(tempFolder, "test.db");

try
{
    // Tables are created (IR-04)
    DatabaseManager db = new DatabaseManager(dbPath, busyTimeoutSeconds: 1);
    Check("Database initializes", db.Initialize());

    foreach (string table in new[] { "Rooms", "Devices", "Thresholds", "Readings", "Alarms", "Events" })
        Check($"Table {table} exists", Count(dbPath, $"SELECT COUNT(*) FROM sqlite_master WHERE name = '{table}'") == 1);

    List<Room> rooms = RoomFactory.CreateDefaultRooms();
    Check("Three rooms (ICU, NICU, OR)", rooms.Count == 3);
    Check("Each room has seven sensors", rooms.All(r => r.Sensors.Count == 7));

    // Start-up saves rooms, sensors and default thresholds
    ScriptedSource source = new ScriptedSource();
    ScadaMonitor monitor = new ScadaMonitor(rooms, source, db);
    List<SensorAlert> raised = new List<SensorAlert>();
    monitor.AlertRaised += (_, alert) => raised.Add(alert);
    monitor.Start();

    Check("Rooms saved", Count(dbPath, "SELECT COUNT(*) FROM Rooms") == 3);
    Check("Devices saved (3 rooms x 7 sensors)", Count(dbPath, "SELECT COUNT(*) FROM Devices") == 21);
    Check("Default thresholds saved", Count(dbPath, "SELECT COUNT(*) FROM Thresholds") == 21);
    Check("Start-up event logged", Count(dbPath, "SELECT COUNT(*) FROM Events WHERE EventType = 'SystemStart'") == 1);

    Room icu = rooms[0];
    Room nicu = rooms[1];
    Sensor icuTemp = icu.Sensors.First(s => s.DeviceID == "TEMP-01");
    Sensor nicuTemp = nicu.Sensors.First(s => s.DeviceID == "TEMP-01");

    // US-3 AC1: normal readings show Normal with no alert, and every reading is saved (FR-17)
    source.Enqueue(ScriptedSource.NormalFrame(rooms));
    monitor.Poll();
    Check("Normal reading shows Normal", icuTemp.Status == "Normal" && icuTemp.CurrentValue == 22.0);
    Check("Normal readings raise no alert", monitor.ActiveAlerts.Count == 0 && raised.Count == 0);
    Check("Every reading saved (21 per cycle)", Count(dbPath, "SELECT COUNT(*) FROM Readings") == 21);

    // US-3 AC2: warning reading raises a Warning alert with room, sensor and time
    var frame = ScriptedSource.NormalFrame(rooms);
    frame[("ICU-01", "TEMP-01")] = 27.0;
    source.Enqueue(frame);
    monitor.Poll();
    Check("Warning reading shows Warning", icuTemp.Status == "Warning");
    Check("Warning alert raised", raised.Count == 1 && raised[0].Level == "Warning");
    Check("Alert has room, sensor and time",
        raised[0].RoomID == "ICU-01" && raised[0].DeviceID == "TEMP-01" && raised[0].Timestamp != default);

    // US-2: other rooms are not affected
    Check("NICU temperature unaffected by ICU reading", nicuTemp.Status == "Normal");

    // US-3 AC3: critical reading shows Critical and is saved in SQLite
    frame = ScriptedSource.NormalFrame(rooms);
    frame[("ICU-01", "TEMP-01")] = 32.0;
    source.Enqueue(frame);
    monitor.Poll();
    Check("Critical reading shows Critical", icuTemp.Status == "Critical");
    Check("Critical alert raised", raised.Count == 2 && raised[1].Level == "Critical");

    List<AlarmRecord> alarms = db.GetAlarms("ICU-01", 100);
    Check("Critical alarm saved in SQLite",
        alarms.Count == 2 && alarms[0].Level == "Critical" && alarms[0].Value == 32.0);
    Check("Alert linked to its alarm row", raised[1].AlarmID == alarms[0].AlarmID);

    // Same status again: no duplicate alert
    source.Enqueue(frame);
    monitor.Poll();
    Check("No duplicate alert while still Critical", raised.Count == 2);

    // US-1 AC3: missing data marks the sensor Failed and saves no value
    frame = ScriptedSource.NormalFrame(rooms);
    frame.Remove(("ICU-01", "HUM-01"));
    source.Enqueue(frame);
    monitor.Poll();
    Sensor icuHum = icu.Sensors.First(s => s.DeviceID == "HUM-01");
    Check("Missing reading marks sensor Failed", icuHum.Status == "Failed");
    Check("Failed alert raised", raised.Last().Level == "Failed" && raised.Last().Value == null);
    Check("Failed reading saved without a value",
        Count(dbPath, "SELECT COUNT(*) FROM Readings WHERE DeviceID = 'HUM-01' AND Status = 'Failed' AND Value IS NULL") == 1);

    // FR-07: acknowledgement and its time are saved
    SensorAlert criticalAlert = raised[1];
    int activeBefore = monitor.ActiveAlerts.Count;
    monitor.Acknowledge(criticalAlert);
    Check("Acknowledged alert leaves the active list", monitor.ActiveAlerts.Count == activeBefore - 1);
    AlarmRecord ackRow = db.GetAlarms("ICU-01", 100).First(a => a.AlarmID == criticalAlert.AlarmID);
    Check("Acknowledgement saved with its time", ackRow.Acknowledged && ackRow.AcknowledgedAt != null);
    Check("Acknowledgement event logged",
        Count(dbPath, "SELECT COUNT(*) FROM Events WHERE EventType = 'AlarmAcknowledged'") == 1);

    // US-2: alarm history only shows the selected room
    Check("Alarm history filtered by room", db.GetAlarms("NICU-01", 100).Count == 0);

    // NFR-04: while the database is locked, records wait in memory and are saved later
    long eventsBefore = Count(dbPath, "SELECT COUNT(*) FROM Events");
    using (SqliteConnection locker = new SqliteConnection($"Data Source={dbPath};Pooling=False"))
    {
        locker.Open();
        using SqliteCommand begin = locker.CreateCommand();
        begin.CommandText = "BEGIN EXCLUSIVE;";
        begin.ExecuteNonQuery();

        bool saved = db.LogEvent("ICU-01", null, "Test", "Written while the database was locked.", "System");
        Check("Write while locked is kept, not lost", !saved && db.PendingWriteCount == 1);
        Check("Locked database reported", db.LastError != null);

        using SqliteCommand rollback = locker.CreateCommand();
        rollback.CommandText = "ROLLBACK;";
        rollback.ExecuteNonQuery();
    }
    Check("Kept record saved once the database is back",
        db.FlushPending() && db.PendingWriteCount == 0 && Count(dbPath, "SELECT COUNT(*) FROM Events") == eventsBefore + 1);

    // IR-03: CSV reading source
    string csvFolder = Path.Combine(tempFolder, "SimulatedData");
    Directory.CreateDirectory(csvFolder);
    File.WriteAllLines(Path.Combine(csvFolder, "icu.csv"), new[]
    {
        "timestamp,roomId,sensorId,value",
        "2026-11-20 08:00:00,ICU-01,TEMP-01,22.5",
        "2026-11-20 08:00:00,icu-01,hum-01,45",
        "2026-11-20 08:00:02,ICU-01,TEMP-01,31.0",
        "2026-11-20 08:00:02,ICU-01,HUM-01,abc",
        "not-a-date,ICU-01,TEMP-01,20"
    });

    CsvReadingSource? csv = CsvReadingSource.FromFolder(csvFolder);
    Check("CSV file loads (bad line skipped)", csv != null && csv.FrameCount == 2);
    if (csv != null)
    {
        var first = csv.NextReadings();
        Check("CSV first time step read", first[("ICU-01", "TEMP-01")] == 22.5 && first[("ICU-01", "HUM-01")] == 45);
        var second = csv.NextReadings();
        Check("CSV invalid value becomes no data", second[("ICU-01", "TEMP-01")] == 31.0 && second[("ICU-01", "HUM-01")] == null);
        Check("CSV loops back to the start", csv.NextReadings()[("ICU-01", "TEMP-01")] == 22.5);
    }
    Check("No CSV files uses the simulator",
        ReadingSources.Create(Path.Combine(tempFolder, "missing"), rooms) is SimulatedReadingSource);
    Check("Simulator gives a reading for all 21 sensors",
        new SimulatedReadingSource(rooms, seed: 1).NextReadings().Count == 21);

    monitor.Stop();
    Check("Stop event logged", Count(dbPath, "SELECT COUNT(*) FROM Events WHERE EventType = 'SystemStop'") == 1);
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: Unexpected exception: {ex}");
    failed++;
}
finally
{
    try { Directory.Delete(tempFolder, recursive: true); } catch { }
}

// Display the results after all tests have run.
Console.WriteLine($"\nTests passed: {passed}");
Console.WriteLine($"Tests failed: {failed}");

// Return a failure exit code if any test failed.
if (failed > 0)
{
    Environment.ExitCode = 1;
}

// Runs a COUNT query directly on the database file.
static long Count(string path, string sql)
{
    using SqliteConnection connection = new SqliteConnection($"Data Source={path};Pooling=False");
    connection.Open();
    using SqliteCommand command = connection.CreateCommand();
    command.CommandText = sql;
    return Convert.ToInt64(command.ExecuteScalar());
}

// Gives the monitor the readings each test needs.
class ScriptedSource : IReadingSource
{
    private readonly Queue<Dictionary<(string RoomID, string DeviceID), double?>> frames =
        new Queue<Dictionary<(string RoomID, string DeviceID), double?>>();

    public string Description => "Scripted test data";

    public void Enqueue(Dictionary<(string RoomID, string DeviceID), double?> frame)
    {
        frames.Enqueue(frame);
    }

    public Dictionary<(string RoomID, string DeviceID), double?> NextReadings()
    {
        return frames.Count > 0 ? frames.Dequeue() : new Dictionary<(string RoomID, string DeviceID), double?>();
    }

    // A normal reading for every sensor in every room.
    public static Dictionary<(string RoomID, string DeviceID), double?> NormalFrame(IEnumerable<Room> rooms)
    {
        Dictionary<string, double> normal = new Dictionary<string, double>
        {
            ["TEMP-01"] = 22.0, ["HUM-01"] = 45.0, ["OXY-01"] = 20.9, ["AQI-01"] = 40.0,
            ["PRESS-01"] = 5.0, ["SMOKE-01"] = 1.0, ["PWR-01"] = 220.0
        };

        Dictionary<(string RoomID, string DeviceID), double?> frame = new Dictionary<(string RoomID, string DeviceID), double?>();
        foreach (Room room in rooms)
        {
            foreach (Sensor sensor in room.Sensors)
                frame[(room.RoomID, sensor.DeviceID)] = normal[sensor.DeviceID];
        }
        return frame;
    }
}
