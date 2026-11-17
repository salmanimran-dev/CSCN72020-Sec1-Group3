using System;
using System.Windows.Forms; 
using HCRMS.Models;

namespace HCRMS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. Create a Room
            Room icuRoom = new Room("ICU-01", "Intensive Care");

            // 2. Create a Temperature Sensor
            Sensor tempSensor = new Sensor("TEMP-01", "°C", 25.0, 30.0);
            icuRoom.Sensors.Add(tempSensor);

            // 3. Test Normal
            tempSensor.UpdateValue(22.5);
            string normalMsg = $"Room: {icuRoom.RoomID}\nSensor: {tempSensor.DeviceID}\nValue: {tempSensor.CurrentValue}{tempSensor.Unit}\nStatus: {tempSensor.Status}";
            MessageBox.Show(normalMsg, "Normal Reading Test");

            // 4. Test Warning
            tempSensor.UpdateValue(27.0);
            string warningMsg = $"Room: {icuRoom.RoomID}\nSensor: {tempSensor.DeviceID}\nValue: {tempSensor.CurrentValue}{tempSensor.Unit}\nStatus: {tempSensor.Status}";
            MessageBox.Show(warningMsg, "Warning Reading Test");

            // 5. Test Critical
            tempSensor.UpdateValue(32.0);
            string criticalMsg = $"Room: {icuRoom.RoomID}\nSensor: {tempSensor.DeviceID}\nValue: {tempSensor.CurrentValue}{tempSensor.Unit}\nStatus: {tempSensor.Status}";
            MessageBox.Show(criticalMsg, "Critical Reading Test");

            // 6. Launch the actual Dashboard (Form1) so the app doesn't close immediately
            Application.Run(new Form1());
        }
    }
}