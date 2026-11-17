namespace HCRMS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblRoomName = new Label();
            lblTempValue = new Label();
            lblStatus = new Label();
            btnSimulate = new Label();
            SuspendLayout();
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRoomName.Location = new Point(116, 120);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(106, 38);
            lblRoomName.TabIndex = 0;
            lblRoomName.Text = "ICU-01";
            lblRoomName.Click += lblRoomName_Click;
            // 
            // lblTempValue
            // 
            lblTempValue.AutoSize = true;
            lblTempValue.Font = new Font("Segoe UI", 36F);
            lblTempValue.Location = new Point(117, 159);
            lblTempValue.Name = "lblTempValue";
            lblTempValue.Size = new Size(220, 81);
            lblTempValue.TabIndex = 1;
            lblTempValue.Text = "22.5 °C";
            lblTempValue.Click += label1_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Location = new Point(117, 249);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(114, 31);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "NORMAL";
            // 
            // btnSimulate
            // 
            btnSimulate.AutoSize = true;
            btnSimulate.Location = new Point(117, 293);
            btnSimulate.Name = "btnSimulate";
            btnSimulate.Size = new Size(161, 20);
            btnSimulate.TabIndex = 3;
            btnSimulate.Text = "Simulate Next Reading";
            btnSimulate.Click += btnSimulate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1394, 716);
            Controls.Add(btnSimulate);
            Controls.Add(lblStatus);
            Controls.Add(lblTempValue);
            Controls.Add(lblRoomName);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblRoomName;
        private Label lblTempValue;
        private Label lblStatus;
        private Label btnSimulate;
    }
}
