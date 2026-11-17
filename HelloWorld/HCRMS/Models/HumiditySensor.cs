
namespace HCRMS.Models
{
    public class HumiditySensor : Sensor
    {
        public HumiditySensor(string id)
            : base(id, "%RH", 60.0, 70.0)
        {
        }
    }
}

