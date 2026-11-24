namespace HCRMS.Core
{
    // An alert shown on the dashboard when a sensor becomes Warning, Critical or Failed (FR-06)
    public class SensorAlert
    {
        // Links the alert to its row in the Alarms table
        public long? AlarmID { get; init; }

        public DateTime Timestamp { get; init; }
        public string RoomID { get; init; } = "";
        public string DeviceID { get; init; } = "";
        public string SensorName { get; init; } = "";
        public double? Value { get; init; }
        public string Unit { get; init; } = "";
        public string Level { get; init; } = "";

        // Value with its unit, or "No data" for a failed sensor
        public string ValueText
        {
            get { return Value == null ? "No data" : $"{Value.Value:0.##} {Unit}"; }
        }
    }
}
