using HCRMS.Models;

namespace HCRMS
{
    public partial class Form1 : Form
    {
        TemperatureSensor tempSensor = new TemperatureSensor("TEMP-01");
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblRoomName_Click(object sender, EventArgs e)
        {

        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            if (tempSensor.CurrentValue == 0)
                tempSensor.UpdateValue(22.5); // Normal
            else if (tempSensor.CurrentValue < 25)
                tempSensor.UpdateValue(27.0); // Warning
            else if (tempSensor.CurrentValue < 30)
                tempSensor.UpdateValue(32.0); // Critical
            else
                tempSensor.UpdateValue(0); // Fault

            // Update the UI labels
            if (tempSensor.CurrentValue == 0)
            {
                lblTempValue.Text = "-- °C";
                lblStatus.Text = "FAULT";
                lblStatus.ForeColor = Color.Gray;
            }
            else
            {
                lblTempValue.Text = $"{tempSensor.CurrentValue} °C";
                lblStatus.Text = tempSensor.Status.ToUpper();

                if (tempSensor.Status == "Normal") lblStatus.ForeColor = Color.Green;
                else if (tempSensor.Status == "Warning") lblStatus.ForeColor = Color.Orange;
                else if (tempSensor.Status == "Critical") lblStatus.ForeColor = Color.Red;
            }

        }
    }
}
