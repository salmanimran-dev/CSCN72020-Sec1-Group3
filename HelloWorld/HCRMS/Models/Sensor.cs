namespace HCRMS.Models
{
    public class Sensor : Device
    {
        // Stores the current sensor reading
        public double CurrentValue { get; set; }

        // Stores the sensor unit
        public string Unit { get; set; }

        // Sets the warning and critical limits
        public double WarningLimit { get; set; }
        public double CriticalLimit { get; set; }

        // Creates a new sensor with its ID, unit, and limits
        public Sensor(string id, string unit, double warningLimit, double criticalLimit) : base(id)
        {
            Unit = unit;
            WarningLimit = warningLimit;
            CriticalLimit = criticalLimit;
        }

        // Updates the reading and checks the sensor status
        public virtual void UpdateValue(double newValue)
        {
            CurrentValue = newValue;

            if (newValue >= CriticalLimit)
                Status = "Critical";
            else if (newValue >= WarningLimit)
                Status = "Warning";
            else
                Status = "Normal";
        }
    }
}