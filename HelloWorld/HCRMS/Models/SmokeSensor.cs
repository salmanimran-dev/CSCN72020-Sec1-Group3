
namespace HCRMS.Models
{
    public class SmokeSensor : Sensor
    {
        // Simulated smoke concentration in ppm
        public SmokeSensor(string id)
            : base(id, "ppm", 10.0, 25.0)
        {
        }
    }
}
