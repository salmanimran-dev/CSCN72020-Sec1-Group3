
namespace HCRMS.Models
{
    public class AirQualitySensor : Sensor
    {
        public AirQualitySensor(string id)
            : base(id, "AQI", 100.0, 150.0)
        {
        }
    }
}

