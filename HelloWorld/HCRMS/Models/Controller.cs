namespace HCRMS.Models
{
    // Base class for all equipment (HVAC, fans, humidifier, dehumidifier, alarm).
    public abstract class Controller : Device
    {
        public bool IsOn { get; private set; }

        protected Controller(string id) : base(id)
        {
            Status = "Off";
        }

        public void TurnOn()
        {
            IsOn = true;
            Status = "On";
        }

        public void TurnOff()
        {
            IsOn = false;
            Status = "Off";
        }
    }
}