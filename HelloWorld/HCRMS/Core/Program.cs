
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
            // Create an ICU room
            Room icuRoom = new Room("ICU-01", "Intensive Care");

            // =====================================
            // P01-P09 - Temperature Sensor Tests
            // =====================================

            Sensor tempSensor = new Sensor(
                "TEMP-01", "°C", 25.0, 30.0);

            icuRoom.Sensors.Add(tempSensor);

            // Normal
            tempSensor.UpdateValue(22.5);
            MessageBox.Show(
                $"Room: {icuRoom.RoomID}\n" +
                $"Sensor: {tempSensor.DeviceID}\n" +
                $"Value: {tempSensor.CurrentValue}{tempSensor.Unit}\n" +
                $"Status: {tempSensor.Status}",
                "Normal Reading Test");

            // Warning
            tempSensor.UpdateValue(27.0);
            MessageBox.Show(
                $"Room: {icuRoom.RoomID}\n" +
                $"Sensor: {tempSensor.DeviceID}\n" +
                $"Value: {tempSensor.CurrentValue}{tempSensor.Unit}\n" +
                $"Status: {tempSensor.Status}",
                "Warning Reading Test");

            // Critical
            tempSensor.UpdateValue(32.0);
            MessageBox.Show(
                $"Room: {icuRoom.RoomID}\n" +
                $"Sensor: {tempSensor.DeviceID}\n" +
                $"Value: {tempSensor.CurrentValue}{tempSensor.Unit}\n" +
                $"Status: {tempSensor.Status}",
                "Critical Reading Test");


            // =====================================
            // P10 - Humidity, Oxygen, Air Quality
            // =====================================

            HumiditySensor humidity =
                new HumiditySensor("HUM-01");

            OxygenSensor oxygen =
                new OxygenSensor("OXY-01");

            AirQualitySensor airQuality =
                new AirQualitySensor("AQI-01");

            icuRoom.Sensors.Add(humidity);
            icuRoom.Sensors.Add(oxygen);
            icuRoom.Sensors.Add(airQuality);

            // Normal readings
            humidity.UpdateValue(45.0);
            oxygen.UpdateValue(20.9);
            airQuality.UpdateValue(50.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Normal Reading Test");

            // Warning readings
            humidity.UpdateValue(65.0);
            oxygen.UpdateValue(23.2);
            airQuality.UpdateValue(120.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Warning Reading Test");

            // Critical readings
            humidity.UpdateValue(80.0);
            oxygen.UpdateValue(24.0);
            airQuality.UpdateValue(180.0);

            MessageBox.Show(
                $"Humidity: {humidity.CurrentValue}% - {humidity.Status}\n" +
                $"Oxygen: {oxygen.CurrentValue}% - {oxygen.Status}\n" +
                $"Air Quality: {airQuality.CurrentValue} AQI - {airQuality.Status}",
                "P10 - Critical Reading Test");


            // =====================================
            // P12 - Power Sensor Tests
            // =====================================

            PowerSensor power =
                new PowerSensor("PWR-01");

            icuRoom.Sensors.Add(power);

            // Normal power
            power.UpdateValue(220);

            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\n" +
                $"Status: {power.Status}",
                "P12 - Normal Power");

            // Low voltage warning
            power.UpdateValue(205);

            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\n" +
                $"Status: {power.Status}",
                "P12 - Low Voltage");

            // Power outage
            power.UpdateValue(0);

            MessageBox.Show(
                $"Power: {power.CurrentValue}{power.Unit}\n" +
                $"Status: {power.Status}",
                "P12 - Power Outage");


            // =====================================
            // P11 - Smoke and Pressure Sensors
            // =====================================

            SmokeSensor smoke =
                new SmokeSensor("SMOKE-01");

            PressureSensor pressure =
                new PressureSensor("PRESS-01");

            icuRoom.Sensors.Add(smoke);
            icuRoom.Sensors.Add(pressure);

            // P11 - Normal readings
            smoke.UpdateValue(5.0);
            pressure.UpdateValue(5.0);

            MessageBox.Show(
                $"Smoke: {smoke.CurrentValue} {smoke.Unit} - {smoke.Status}\n" +
                $"Pressure: {pressure.CurrentValue} {pressure.Unit} - {pressure.Status}",
                "P11 - Normal Reading Test");

            // P11 - Warning readings
            smoke.UpdateValue(15.0);
            pressure.UpdateValue(15.0);

            MessageBox.Show(
                $"Smoke: {smoke.CurrentValue} {smoke.Unit} - {smoke.Status}\n" +
                $"Pressure: {pressure.CurrentValue} {pressure.Unit} - {pressure.Status}",
                "P11 - Warning Reading Test");

            // P11 - Critical readings
            smoke.UpdateValue(30.0);
            pressure.UpdateValue(25.0);

            MessageBox.Show(
                $"Smoke: {smoke.CurrentValue} {smoke.Unit} - {smoke.Status}\n" +
                $"Pressure: {pressure.CurrentValue} {pressure.Unit} - {pressure.Status}",
                "P11 - Critical Reading Test");


            // =====================================
            // P17 - Launch the SCADA dashboard
            // =====================================

            Application.Run(new SCADADashboard());
        }
    }
}
