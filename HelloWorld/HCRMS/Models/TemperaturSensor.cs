namespace HCRMS.Models
{
    public class TemperatureSensor : Sensor
    {
        public TemperatureSensor(string id) : base(id, "°C", 25.0, 30.0)
        {
        }
    }
}