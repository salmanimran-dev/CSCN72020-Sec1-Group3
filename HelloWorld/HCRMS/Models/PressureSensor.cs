
namespace HCRMS.Models
{
    public class PressureSensor : Sensor
    {
        // Simulated room pressure in Pascals
        public PressureSensor(string id)
            : base(id, "Pa", 10.0, 20.0)
        {
        }
    }
}
