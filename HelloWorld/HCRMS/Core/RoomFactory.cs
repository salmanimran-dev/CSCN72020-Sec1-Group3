using HCRMS.Models;

namespace HCRMS.Core
{
    // Builds the three critical rooms (SR-07), each with the seven sensors (SR-03).
    public static class RoomFactory
    {
        public static List<Room> CreateDefaultRooms()
        {
            return new List<Room>
            {
                CreateRoom("ICU-01", "ICU"),
                CreateRoom("NICU-01", "NICU"),
                CreateRoom("OR-01", "Operating Room")
            };
        }

        public static Room CreateRoom(string roomId, string roomType)
        {
            var room = new Room(roomId, roomType);
            room.Sensors.Add(new TemperatureSensor("TEMP-01"));
            room.Sensors.Add(new HumiditySensor("HUM-01"));
            room.Sensors.Add(new OxygenSensor("OXY-01"));
            room.Sensors.Add(new AirQualitySensor("AQI-01"));
            room.Sensors.Add(new PressureSensor("PRESS-01"));
            room.Sensors.Add(new SmokeSensor("SMOKE-01"));
            room.Sensors.Add(new PowerSensor("PWR-01"));
            return room;
        }
    }

    public static class SensorNames
    {
        // Friendly name shown on the dashboard and stored as the device type.
        public static string For(Sensor sensor)
        {
            return sensor switch
            {
                TemperatureSensor => "Temperature",
                HumiditySensor => "Humidity",
                OxygenSensor => "Oxygen",
                AirQualitySensor => "Air Quality",
                PressureSensor => "Room Pressure",
                SmokeSensor => "Smoke",
                PowerSensor => "Power Supply",
                _ => sensor.GetType().Name
            };
        }
    }
}
