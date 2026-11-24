using HCRMS.Data;
using HCRMS.Models;

namespace HCRMS.Core
{
    // P17 - SCADA monitoring loop (no UI code, so it can be tested on its own)
    // Each Poll() reads new values (FR-01), updates the sensors, saves every
    // reading (FR-17) and raises an alert when a status changes (FR-06)
    // P18: when AlarmManager (P15) and ThresholdManager (P13) are merged,
    // CheckForAlert is the place to call them
    public class ScadaMonitor
    {
        private readonly IReadingSource source;
        private readonly DatabaseManager database;

        // Last status of each sensor, so alerts are only raised on a change
        private readonly Dictionary<(string RoomID, string DeviceID), string> lastStatus =
            new Dictionary<(string RoomID, string DeviceID), string>();

        private readonly List<SensorAlert> activeAlerts = new List<SensorAlert>();

        public IReadOnlyList<Room> Rooms { get; }

        // Alerts that have not been acknowledged yet
        public IReadOnlyList<SensorAlert> ActiveAlerts => activeAlerts;

        // Raised once for each new alert
        public event EventHandler<SensorAlert>? AlertRaised;

        public ScadaMonitor(IEnumerable<Room> rooms, IReadingSource source, DatabaseManager database)
        {
            Rooms = rooms.ToList();
            this.source = source;
            this.database = database;
        }

        // Saves the rooms, sensors and default thresholds, then logs start-up
        public void Start()
        {
            database.Initialize();

            foreach (Room room in Rooms)
            {
                database.SaveRoom(room);
                foreach (Sensor sensor in room.Sensors)
                {
                    database.SaveDevice(room.RoomID, sensor, SensorNames.For(sensor));
                    database.SaveDefaultThreshold(room.RoomID, sensor);
                }
            }

            database.LogEvent(null, null, "SystemStart", $"Monitoring started using {source.Description}.", "System");
        }

        // Reads the next values for every room, saves them and checks for alerts
        public void Poll()
        {
            DateTime now = DateTime.Now;
            var readings = source.NextReadings();

            foreach (Room room in Rooms)
            {
                foreach (Sensor sensor in room.Sensors)
                {
                    readings.TryGetValue((room.RoomID, sensor.DeviceID), out double? value);
                    ApplyReading(sensor, value);
                    CheckForAlert(room, sensor, now);
                }
                database.LogReadings(room.RoomID, room.Sensors, now);
            }
        }

        // A missing or invalid value marks the sensor Failed instead of showing a made-up value (US-1)
        public static void ApplyReading(Sensor sensor, double? value)
        {
            if (value == null || !double.IsFinite(value.Value))
            {
                sensor.CurrentValue = double.NaN;
                sensor.Status = "Failed";
                return;
            }
            sensor.UpdateValue(value.Value);
        }

        // Raises an alert and saves an alarm when a sensor moves to Warning, Critical or Failed
        private void CheckForAlert(Room room, Sensor sensor, DateTime now)
        {
            var key = (room.RoomID, sensor.DeviceID);
            string previous = lastStatus.TryGetValue(key, out string? status) ? status : "Normal";
            lastStatus[key] = sensor.Status;

            if (sensor.Status == previous || sensor.Status == "Normal")
                return;

            double? value = null;
            if (double.IsFinite(sensor.CurrentValue))
                value = sensor.CurrentValue;

            long? alarmId = database.LogAlarm(now, room.RoomID, sensor.DeviceID, value, sensor.Status);

            SensorAlert alert = new SensorAlert
            {
                AlarmID = alarmId,
                Timestamp = now,
                RoomID = room.RoomID,
                DeviceID = sensor.DeviceID,
                SensorName = SensorNames.For(sensor),
                Value = value,
                Unit = sensor.Unit,
                Level = sensor.Status
            };

            activeAlerts.Add(alert);
            AlertRaised?.Invoke(this, alert);
        }

        // Saves the acknowledgement and its time (FR-07)
        public void Acknowledge(SensorAlert alert)
        {
            if (!activeAlerts.Contains(alert))
                return;

            if (alert.AlarmID != null)
                database.AcknowledgeAlarm(alert.AlarmID.Value, DateTime.Now);

            database.LogEvent(alert.RoomID, alert.DeviceID, "AlarmAcknowledged",
                              $"{alert.Level} alert on {alert.SensorName} ({alert.ValueText}) acknowledged.",
                              "HMI user");

            activeAlerts.Remove(alert);
        }

        // Logs shut-down and saves anything still waiting in memory
        public void Stop()
        {
            database.LogEvent(null, null, "SystemStop", "Monitoring stopped.", "System");
            database.FlushPending();
        }
    }
}
