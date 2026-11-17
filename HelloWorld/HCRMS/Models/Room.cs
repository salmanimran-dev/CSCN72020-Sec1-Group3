using HCRMS.Models;
using System.Collections.Generic;

namespace HCRMS
{
    public class Room
    {
        public string RoomID { get; set; }
        public string RoomType { get; set; }
        public List<Sensor> Sensors { get; set; }

        public Room(string id, string type)
        {
            RoomID = id;
            RoomType = type;
            Sensors = new List<Sensor>();
        }
    }
}