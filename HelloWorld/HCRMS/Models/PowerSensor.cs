
namespace HCRMS.Models
{
    public class PowerSensor : Sensor
    {
        public PowerSensor(string id)
            : base(id, "V", 230.0, 250.0)
        {
        }

        // Handles both low and high voltage readings.
        public override void UpdateValue(double newValue)
        {
            CurrentValue = newValue;

            // Invalid sensor readings indicate a failed sensor.
            if (double.IsNaN(newValue) ||
                double.IsInfinity(newValue) ||
                newValue < 0)
            {
                Status = "Failed";
            }
            // Zero voltage indicates a power outage.
            else if (newValue == 0)
            {
                Status = "Critical";
            }
            // Low voltage or dangerously high voltage.
            else if (newValue < 200 || newValue >= CriticalLimit)
            {
                Status = "Critical";
            }
            // Voltage approaching the unsafe range.
            else if (newValue < 210 || newValue >= WarningLimit)
            {
                Status = "Warning";
            }
            else
            {
                Status = "Normal";
            }
        }
    }
}
