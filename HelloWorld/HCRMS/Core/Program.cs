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


            // P10 - Create sensors
            HumiditySensor humidity = new HumiditySensor("HUM-01");
            OxygenSensor oxygen = new OxygenSensor("OXY-01");
            AirQualitySensor airQuality = new AirQualitySensor("AQI-01");

            icuRoom.Sensors.Add(humidity);
            icuRoom.Sensors.Add(oxygen);
            icuRoom.Sensors.Add(airQuality);

            // Test normal readings
            humidity.UpdateValue(45.0);
            oxygen.UpdateValue(20.9);
            airQuality.UpdateValue(50.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Normal Reading Test");

            // Test warning readings
            humidity.UpdateValue(65.0);
            oxygen.UpdateValue(23.2);
            airQuality.UpdateValue(120.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Warning Reading Test");

            // Test critical readings
            humidity.UpdateValue(80.0);
            oxygen.UpdateValue(24.0);
            airQuality.UpdateValue(180.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Critical Reading Test");

            
            // P12 - Create a power sensor for the ICU room.
            PowerSensor power = new PowerSensor("PWR-01");
            icuRoom.Sensors.Add(power);

            // Test 1: Normal power reading.
            power.UpdateValue(220);
            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\nStatus: {power.Status}",
                "P12 - Normal Power");

            // Test 2: Low voltage warning.
            power.UpdateValue(205);
            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\nStatus: {power.Status}",
                "P12 - Low Voltage");

            // Test 3: Power outage.
            power.UpdateValue(0);
            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\nStatus: {power.Status}",
                "P12 - Power Outage");


            // 6. Launch the actual Dashboard (Form1) so the app doesn't close immediately
            Application.Run(new Form1());
        }
    }
}