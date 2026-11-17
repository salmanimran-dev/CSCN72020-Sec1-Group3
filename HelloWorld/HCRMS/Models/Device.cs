namespace HCRMS.Models
{
    public abstract class Device
    {
        public string DeviceID { get; set; }
        public string Status { get; set; }

        public Device(string id)
        {
            DeviceID = id;
            Status = "Normal";
        }
    }
}