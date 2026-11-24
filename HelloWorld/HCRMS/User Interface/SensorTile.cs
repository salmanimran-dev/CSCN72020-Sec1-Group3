using HCRMS.Core;
using HCRMS.Models;

namespace HCRMS
{
    // Colour and text label for each sensor status.
    // Colour is never the only signal: each status also has a text label (IR-02, UR-06).
    internal readonly struct StatusStyle
    {
        public Color Back { get; }
        public Color Fore { get; }
        public string Label { get; }

        // Sort order for the alert list: Critical, Failed, Warning, Normal
        public int Rank { get; }

        public StatusStyle(Color back, Color fore, string label, int rank)
        {
            Back = back;
            Fore = fore;
            Label = label;
            Rank = rank;
        }

        public static StatusStyle For(string status)
        {
            if (status == "Normal")
                return new StatusStyle(Color.FromArgb(46, 158, 107), Color.White, "NORMAL", 3);
            else if (status == "Warning")
                return new StatusStyle(Color.FromArgb(232, 163, 58), Color.White, "WARNING", 2);
            else if (status == "Critical")
                return new StatusStyle(Color.FromArgb(198, 40, 40), Color.White, "CRITICAL", 0);
            else
                return new StatusStyle(Color.FromArgb(117, 117, 117), Color.White, "FAILED", 1);
        }
    }

    // One sensor card: name, value with unit and a status badge (FR-02, UR-03)
    internal class SensorTile : Panel
    {
        private readonly Label lblName = new Label();
        private readonly Label lblValue = new Label();
        private readonly Label lblStatus = new Label();

        public SensorTile()
        {
            Dock = DockStyle.Fill;
            Margin = new Padding(8);
            BackColor = Color.White;

            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 9F);
            lblName.ForeColor = Color.FromArgb(97, 97, 97);

            // Fixed size so the value stays on one line instead of wrapping
            lblValue.AutoSize = false;
            lblValue.Dock = DockStyle.Fill;
            lblValue.AutoEllipsis = true;
            lblValue.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblValue.ForeColor = Color.FromArgb(18, 38, 63);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            lblStatus.AutoSize = true;
            lblStatus.Anchor = AnchorStyles.Left;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Padding = new Padding(10, 6, 10, 6);

            // Name, value and status badge stacked from top to bottom
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.Padding = new Padding(16, 12, 16, 12);
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(lblName, 0, 0);
            layout.Controls.Add(lblValue, 0, 1);
            layout.Controls.Add(lblStatus, 0, 2);
            Controls.Add(layout);
        }

        // Updates the card with the sensor's latest reading
        public void ShowSensor(Sensor sensor)
        {
            lblName.Text = SensorNames.For(sensor);

            // A failed sensor shows "--" instead of a made-up value (US-1)
            if (sensor.Status == "Failed" || !double.IsFinite(sensor.CurrentValue))
                lblValue.Text = $"-- {sensor.Unit}";
            else
                lblValue.Text = $"{sensor.CurrentValue:0.##} {sensor.Unit}";

            StatusStyle style = StatusStyle.For(sensor.Status);
            lblStatus.Text = style.Label;
            lblStatus.BackColor = style.Back;
            lblStatus.ForeColor = style.Fore;
        }
    }
}
