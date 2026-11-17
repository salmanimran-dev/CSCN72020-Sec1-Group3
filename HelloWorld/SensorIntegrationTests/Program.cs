
using HCRMS.Models;

// Create the sensors using the existing classes.
TemperatureSensor temperature = new TemperatureSensor("T001");
PowerSensor power = new PowerSensor("P001");

int passed = 0;
int failed = 0;

// A simple helper to report test results.
void Check(string name, bool condition)
{
    if (condition)
    {
        Console.WriteLine($"PASS: {name}");
        passed++;
    }
    else
    {
        Console.WriteLine($"FAIL: {name}");
        failed++;
    }
}

// Test 1: Temperature sensor normal reading.
temperature.UpdateValue(22);
Check("Temperature Normal", temperature.Status == "Normal");

// Test 2: Temperature sensor warning reading.
temperature.UpdateValue(27);
Check("Temperature Warning", temperature.Status == "Warning");

// Test 3: Temperature sensor critical reading.
temperature.UpdateValue(32);
Check("Temperature Critical", temperature.Status == "Critical");

// Test 4: Power sensor normal reading.
power.UpdateValue(220);
Check("Power Normal", power.Status == "Normal");

// Test 5: Power sensor warning reading.
power.UpdateValue(240);
Check("Power Warning", power.Status == "Warning");

// Test 6: Power sensor critical reading.
power.UpdateValue(255);
Check("Power Critical", power.Status == "Critical");

// Test 7: Verify that both sensors inherit from Sensor.
Check("Temperature Inheritance", temperature is Sensor);
Check("Power Inheritance", power is Sensor);


power.UpdateValue(0);
Check("Power Outage", power.Status == "Critical");

power.UpdateValue(195);
Check("Low Voltage Critical", power.Status == "Critical");

power.UpdateValue(205);
Check("Low Voltage Warning", power.Status == "Warning");

power.UpdateValue(-5);
Check("Invalid Voltage", power.Status == "Failed");

power.UpdateValue(220);
Check("Power Recovery", power.Status == "Normal");

// Test power failure through a Sensor reference.
Sensor sensorReference = power;
sensorReference.UpdateValue(0);

Check("Power Outage Through Base Reference",
    sensorReference.Status == "Critical");

// Create an ICU room and attach the power sensor.
HCRMS.Room icu = new HCRMS.Room("ICU-01", "Intensive Care");

icu.Sensors.Add(power);

// Verify that the room contains our power sensor.
Check("Power Sensor Room Integration",
    icu.Sensors.Contains(power));

// Verify that the power sensor can be retrieved from the room.
Check("Power Sensor ID",
    icu.Sensors[0].DeviceID == "P001");


// Display the results after all 16 tests have run.
Console.WriteLine($"\nTests passed: {passed}");
Console.WriteLine($"Tests failed: {failed}");

// Return a failure exit code if any test failed.
if (failed > 0)
{
    Environment.ExitCode = 1;
}