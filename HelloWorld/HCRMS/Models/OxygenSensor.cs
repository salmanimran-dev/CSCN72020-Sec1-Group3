
namespace HCRMS.Models
{
    public class OxygenSensor : Sensor
    {
        public OxygenSensor(string id)
            : base(id, "%O2", 23.0, 23.5)
        {
        }
    }
}
