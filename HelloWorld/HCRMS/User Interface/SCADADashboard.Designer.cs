namespace HCRMS
{
    partial class SCADADashboard
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
            components = new System.ComponentModel.Container();
            pnlSidebar = new Panel();
            flpNav = new FlowLayoutPanel();
            lblLogo = new Label();
            btnNavDashboard = new Button();
            btnNavControls = new Button();
            btnNavAlarmHistory = new Button();
            btnNavHistoricalData = new Button();
            btnNavSettings = new Button();
            tlpContent = new TableLayoutPanel();
            flpHeader = new FlowLayoutPanel();
            lblRoomName = new Label();
            cboRoom = new ComboBox();
            lblCriticalBanner = new Label();
            pnlPages = new Panel();
            tlpDashboard = new TableLayoutPanel();
            tlpSensors = new TableLayoutPanel();
            grpAlerts = new GroupBox();
            tlpAlerts = new TableLayoutPanel();
            lvAlerts = new ListView();
            colTime = new ColumnHeader();
            colRoom = new ColumnHeader();
            colSensor = new ColumnHeader();
            colValue = new ColumnHeader();
            colLevel = new ColumnHeader();
            btnAcknowledge = new Button();
            dgvAlarms = new DataGridView();
            lblControlsPage = new Label();
            lblHistoricalDataPage = new Label();
            lblSettingsPage = new Label();
            statusStrip = new StatusStrip();
            lblDatabase = new ToolStripStatusLabel();
            tmrPoll = new System.Windows.Forms.Timer(components);
            pnlSidebar.SuspendLayout();
            flpNav.SuspendLayout();
            tlpContent.SuspendLayout();
            flpHeader.SuspendLayout();
            pnlPages.SuspendLayout();
            tlpDashboard.SuspendLayout();
            grpAlerts.SuspendLayout();
            tlpAlerts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAlarms).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // pnlSidebar
            //
            pnlSidebar.BackColor = Color.FromArgb(18, 38, 63);
            pnlSidebar.Controls.Add(flpNav);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(16, 24, 16, 16);
            pnlSidebar.Size = new Size(230, 774);
            pnlSidebar.TabIndex = 1;
            //
            // flpNav
            //
            flpNav.Controls.Add(lblLogo);
            flpNav.Controls.Add(btnNavDashboard);
            flpNav.Controls.Add(btnNavControls);
            flpNav.Controls.Add(btnNavAlarmHistory);
            flpNav.Controls.Add(btnNavHistoricalData);
            flpNav.Controls.Add(btnNavSettings);
            flpNav.Dock = DockStyle.Fill;
            flpNav.FlowDirection = FlowDirection.TopDown;
            flpNav.Name = "flpNav";
            flpNav.TabIndex = 0;
            flpNav.WrapContents = false;
            //
            // lblLogo
            //
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLogo.ForeColor = Color.White;
            lblLogo.Margin = new Padding(12, 0, 0, 32);
            lblLogo.Name = "lblLogo";
            lblLogo.TabIndex = 0;
            lblLogo.Text = "HCRMS";
            //
            // btnNavDashboard
            //
            btnNavDashboard.BackColor = Color.FromArgb(31, 111, 178);
            btnNavDashboard.FlatAppearance.BorderSize = 0;
            btnNavDashboard.FlatStyle = FlatStyle.Flat;
            btnNavDashboard.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavDashboard.ForeColor = Color.White;
            btnNavDashboard.Margin = new Padding(0, 0, 0, 8);
            btnNavDashboard.Name = "btnNavDashboard";
            btnNavDashboard.Padding = new Padding(12, 0, 0, 0);
            btnNavDashboard.Size = new Size(198, 44);
            btnNavDashboard.TabIndex = 1;
            btnNavDashboard.Text = "Dashboard";
            btnNavDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnNavDashboard.UseVisualStyleBackColor = false;
            btnNavDashboard.Click += btnNavDashboard_Click;
            //
            // btnNavControls
            // Controls page is filled in by P18
            //
            btnNavControls.BackColor = Color.FromArgb(18, 38, 63);
            btnNavControls.FlatAppearance.BorderSize = 0;
            btnNavControls.FlatStyle = FlatStyle.Flat;
            btnNavControls.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnNavControls.ForeColor = Color.White;
            btnNavControls.Margin = new Padding(0, 0, 0, 8);
            btnNavControls.Name = "btnNavControls";
            btnNavControls.Padding = new Padding(12, 0, 0, 0);
            btnNavControls.Size = new Size(198, 44);
            btnNavControls.TabIndex = 2;
            btnNavControls.Text = "Controls";
            btnNavControls.TextAlign = ContentAlignment.MiddleLeft;
            btnNavControls.UseVisualStyleBackColor = false;
            btnNavControls.Click += btnNavControls_Click;
            //
            // btnNavAlarmHistory
            //
            btnNavAlarmHistory.BackColor = Color.FromArgb(18, 38, 63);
            btnNavAlarmHistory.FlatAppearance.BorderSize = 0;
            btnNavAlarmHistory.FlatStyle = FlatStyle.Flat;
            btnNavAlarmHistory.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnNavAlarmHistory.ForeColor = Color.White;
            btnNavAlarmHistory.Margin = new Padding(0, 0, 0, 8);
            btnNavAlarmHistory.Name = "btnNavAlarmHistory";
            btnNavAlarmHistory.Padding = new Padding(12, 0, 0, 0);
            btnNavAlarmHistory.Size = new Size(198, 44);
            btnNavAlarmHistory.TabIndex = 3;
            btnNavAlarmHistory.Text = "Alarm History";
            btnNavAlarmHistory.TextAlign = ContentAlignment.MiddleLeft;
            btnNavAlarmHistory.UseVisualStyleBackColor = false;
            btnNavAlarmHistory.Click += btnNavAlarmHistory_Click;
            //
            // btnNavHistoricalData
            // Historical Data page (graphs) is filled in by P19
            //
            btnNavHistoricalData.BackColor = Color.FromArgb(18, 38, 63);
            btnNavHistoricalData.FlatAppearance.BorderSize = 0;
            btnNavHistoricalData.FlatStyle = FlatStyle.Flat;
            btnNavHistoricalData.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnNavHistoricalData.ForeColor = Color.White;
            btnNavHistoricalData.Margin = new Padding(0, 0, 0, 8);
            btnNavHistoricalData.Name = "btnNavHistoricalData";
            btnNavHistoricalData.Padding = new Padding(12, 0, 0, 0);
            btnNavHistoricalData.Size = new Size(198, 44);
            btnNavHistoricalData.TabIndex = 4;
            btnNavHistoricalData.Text = "Historical Data";
            btnNavHistoricalData.TextAlign = ContentAlignment.MiddleLeft;
            btnNavHistoricalData.UseVisualStyleBackColor = false;
            btnNavHistoricalData.Click += btnNavHistoricalData_Click;
            //
            // btnNavSettings
            // Settings page (thresholds) is filled in with the ThresholdManager (P13)
            //
            btnNavSettings.BackColor = Color.FromArgb(18, 38, 63);
            btnNavSettings.FlatAppearance.BorderSize = 0;
            btnNavSettings.FlatStyle = FlatStyle.Flat;
            btnNavSettings.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnNavSettings.ForeColor = Color.White;
            btnNavSettings.Margin = new Padding(0, 0, 0, 8);
            btnNavSettings.Name = "btnNavSettings";
            btnNavSettings.Padding = new Padding(12, 0, 0, 0);
            btnNavSettings.Size = new Size(198, 44);
            btnNavSettings.TabIndex = 5;
            btnNavSettings.Text = "Settings";
            btnNavSettings.TextAlign = ContentAlignment.MiddleLeft;
            btnNavSettings.UseVisualStyleBackColor = false;
            btnNavSettings.Click += btnNavSettings_Click;
            //
            // tlpContent
            //
            tlpContent.ColumnCount = 1;
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpContent.Controls.Add(flpHeader, 0, 0);
            tlpContent.Controls.Add(lblCriticalBanner, 0, 1);
            tlpContent.Controls.Add(pnlPages, 0, 2);
            tlpContent.Dock = DockStyle.Fill;
            tlpContent.Name = "tlpContent";
            tlpContent.Padding = new Padding(24, 20, 24, 12);
            tlpContent.RowCount = 3;
            tlpContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpContent.TabIndex = 0;
            //
            // flpHeader
            //
            flpHeader.AutoSize = true;
            flpHeader.Controls.Add(lblRoomName);
            flpHeader.Controls.Add(cboRoom);
            flpHeader.Margin = new Padding(0, 0, 0, 12);
            flpHeader.Name = "flpHeader";
            flpHeader.TabIndex = 0;
            flpHeader.WrapContents = false;
            //
            // lblRoomName
            // The selected room stays visible at the top (US-2)
            //
            lblRoomName.AutoSize = true;
            lblRoomName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRoomName.ForeColor = Color.FromArgb(18, 38, 63);
            lblRoomName.Margin = new Padding(8, 0, 24, 0);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.TabIndex = 0;
            lblRoomName.Text = "ICU-01";
            //
            // cboRoom
            //
            cboRoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRoom.Margin = new Padding(0, 10, 0, 0);
            cboRoom.Name = "cboRoom";
            cboRoom.Size = new Size(220, 28);
            cboRoom.TabIndex = 1;
            cboRoom.SelectedIndexChanged += cboRoom_SelectedIndexChanged;
            //
            // lblCriticalBanner
            //
            lblCriticalBanner.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblCriticalBanner.BackColor = Color.FromArgb(198, 40, 40);
            lblCriticalBanner.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCriticalBanner.ForeColor = Color.White;
            lblCriticalBanner.Margin = new Padding(8, 0, 8, 8);
            lblCriticalBanner.Name = "lblCriticalBanner";
            lblCriticalBanner.Padding = new Padding(12, 0, 0, 0);
            lblCriticalBanner.Size = new Size(1100, 40);
            lblCriticalBanner.TabIndex = 1;
            lblCriticalBanner.Text = "CRITICAL";
            lblCriticalBanner.TextAlign = ContentAlignment.MiddleLeft;
            lblCriticalBanner.Visible = false;
            //
            // pnlPages
            // Holds one page for each sidebar button
            //
            pnlPages.Controls.Add(tlpDashboard);
            pnlPages.Controls.Add(dgvAlarms);
            pnlPages.Controls.Add(lblControlsPage);
            pnlPages.Controls.Add(lblHistoricalDataPage);
            pnlPages.Controls.Add(lblSettingsPage);
            pnlPages.Dock = DockStyle.Fill;
            pnlPages.Name = "pnlPages";
            pnlPages.TabIndex = 2;
            //
            // tlpDashboard
            //
            tlpDashboard.ColumnCount = 2;
            tlpDashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
            tlpDashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            tlpDashboard.Controls.Add(tlpSensors, 0, 0);
            tlpDashboard.Controls.Add(grpAlerts, 1, 0);
            tlpDashboard.Dock = DockStyle.Fill;
            tlpDashboard.Name = "tlpDashboard";
            tlpDashboard.RowCount = 1;
            tlpDashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpDashboard.TabIndex = 0;
            //
            // tlpSensors
            // The sensor tiles are added in code when a room is selected (3 per row, as in the pitch)
            //
            tlpSensors.ColumnCount = 3;
            tlpSensors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpSensors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpSensors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tlpSensors.Dock = DockStyle.Fill;
            tlpSensors.Name = "tlpSensors";
            tlpSensors.RowCount = 3;
            tlpSensors.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tlpSensors.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tlpSensors.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
            tlpSensors.TabIndex = 0;
            //
            // grpAlerts
            //
            grpAlerts.BackColor = Color.White;
            grpAlerts.Controls.Add(tlpAlerts);
            grpAlerts.Dock = DockStyle.Fill;
            grpAlerts.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpAlerts.ForeColor = Color.FromArgb(18, 38, 63);
            grpAlerts.Margin = new Padding(8);
            grpAlerts.Name = "grpAlerts";
            grpAlerts.TabIndex = 1;
            grpAlerts.TabStop = false;
            grpAlerts.Text = "Active alerts (all rooms)";
            //
            // tlpAlerts
            //
            tlpAlerts.ColumnCount = 1;
            tlpAlerts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpAlerts.Controls.Add(lvAlerts, 0, 0);
            tlpAlerts.Controls.Add(btnAcknowledge, 0, 1);
            tlpAlerts.Dock = DockStyle.Fill;
            tlpAlerts.Name = "tlpAlerts";
            tlpAlerts.RowCount = 2;
            tlpAlerts.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpAlerts.RowStyles.Add(new RowStyle());
            tlpAlerts.TabIndex = 0;
            //
            // lvAlerts
            //
            lvAlerts.BorderStyle = BorderStyle.None;
            lvAlerts.Columns.AddRange(new ColumnHeader[] { colTime, colRoom, colSensor, colValue, colLevel });
            lvAlerts.Dock = DockStyle.Fill;
            lvAlerts.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvAlerts.ForeColor = Color.Black;
            lvAlerts.FullRowSelect = true;
            lvAlerts.HideSelection = false;
            lvAlerts.Name = "lvAlerts";
            lvAlerts.TabIndex = 0;
            lvAlerts.UseCompatibleStateImageBehavior = false;
            lvAlerts.View = View.Details;
            //
            // colTime
            //
            colTime.Text = "Time";
            colTime.Width = 62;
            //
            // colRoom
            //
            colRoom.Text = "Room";
            colRoom.Width = 58;
            //
            // colSensor
            //
            colSensor.Text = "Sensor";
            colSensor.Width = 88;
            //
            // colValue
            //
            colValue.Text = "Value";
            colValue.Width = 68;
            //
            // colLevel
            //
            colLevel.Text = "Level";
            colLevel.Width = 72;
            //
            // btnAcknowledge
            //
            btnAcknowledge.Anchor = AnchorStyles.Right;
            btnAcknowledge.AutoSize = true;
            btnAcknowledge.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAcknowledge.ForeColor = Color.Black;
            btnAcknowledge.Name = "btnAcknowledge";
            btnAcknowledge.TabIndex = 1;
            btnAcknowledge.Text = "Acknowledge selected";
            btnAcknowledge.UseVisualStyleBackColor = true;
            btnAcknowledge.Click += btnAcknowledge_Click;
            //
            // dgvAlarms
            // Alarm History page (read-only, NFR-05)
            //
            dgvAlarms.AllowUserToAddRows = false;
            dgvAlarms.AllowUserToDeleteRows = false;
            dgvAlarms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAlarms.BackgroundColor = Color.White;
            dgvAlarms.BorderStyle = BorderStyle.None;
            dgvAlarms.Dock = DockStyle.Fill;
            dgvAlarms.Name = "dgvAlarms";
            dgvAlarms.ReadOnly = true;
            dgvAlarms.RowHeadersVisible = false;
            dgvAlarms.TabIndex = 1;
            dgvAlarms.Visible = false;
            // 
            // lblControlsPage
            // 
            lblControlsPage.BackColor = Color.White;
            lblControlsPage.Dock = DockStyle.Fill;
            lblControlsPage.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblControlsPage.ForeColor = Color.FromArgb(18, 38, 63);
            lblControlsPage.Name = "lblControlsPage";
            lblControlsPage.Padding = new Padding(24);
            lblControlsPage.TabIndex = 2;
            lblControlsPage.Text = "Controls\n\nEquipment controls for the selected room (HVAC, ventilation fans, humidifier, dehumidifier and alarm) will appear here.\n\nComing in P18 – Integrate controllers, alerts and test.";
            lblControlsPage.Visible = false;
            // 
            // lblHistoricalDataPage
            // 
            lblHistoricalDataPage.BackColor = Color.White;
            lblHistoricalDataPage.Dock = DockStyle.Fill;
            lblHistoricalDataPage.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblHistoricalDataPage.ForeColor = Color.FromArgb(18, 38, 63);
            lblHistoricalDataPage.Name = "lblHistoricalDataPage";
            lblHistoricalDataPage.Padding = new Padding(24);
            lblHistoricalDataPage.TabIndex = 3;
            lblHistoricalDataPage.Text = "Historical Data\n\nLive and historical graphs of each sensor's readings will appear here.\n\nComing in P19 – Dashboard improvements and visualization.";
            lblHistoricalDataPage.Visible = false;
            // 
            // lblSettingsPage
            // 
            lblSettingsPage.BackColor = Color.White;
            lblSettingsPage.Dock = DockStyle.Fill;
            lblSettingsPage.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSettingsPage.ForeColor = Color.FromArgb(18, 38, 63);
            lblSettingsPage.Name = "lblSettingsPage";
            lblSettingsPage.Padding = new Padding(24);
            lblSettingsPage.TabIndex = 4;
            lblSettingsPage.Text = "Settings\n\nThe warning and critical thresholds for each room and sensor will be shown and changed here.\n\nComing with the ThresholdManager (P13).";
            lblSettingsPage.Visible = false;
            //
            // statusStrip
            //
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { lblDatabase });
            statusStrip.Name = "statusStrip";
            statusStrip.TabIndex = 2;
            //
            // lblDatabase
            //
            lblDatabase.Name = "lblDatabase";
            lblDatabase.Text = "Database:";
            //
            // tmrPoll
            //
            tmrPoll.Interval = 2000;
            tmrPoll.Tick += tmrPoll_Tick;
            //
            // SCADADashboard
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 247, 250);
            ClientSize = new Size(1394, 800);
            Controls.Add(tlpContent);
            Controls.Add(pnlSidebar);
            Controls.Add(statusStrip);
            MinimumSize = new Size(1100, 700);
            Name = "SCADADashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HCRMS – Hospital Critical Room Monitoring System";
            pnlSidebar.ResumeLayout(false);
            flpNav.ResumeLayout(false);
            flpNav.PerformLayout();
            tlpContent.ResumeLayout(false);
            tlpContent.PerformLayout();
            flpHeader.ResumeLayout(false);
            flpHeader.PerformLayout();
            pnlPages.ResumeLayout(false);
            tlpDashboard.ResumeLayout(false);
            grpAlerts.ResumeLayout(false);
            tlpAlerts.ResumeLayout(false);
            tlpAlerts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAlarms).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlSidebar;
        private FlowLayoutPanel flpNav;
        private Label lblLogo;
        private Button btnNavDashboard;
        private Button btnNavControls;
        private Button btnNavAlarmHistory;
        private Button btnNavHistoricalData;
        private Button btnNavSettings;
        private TableLayoutPanel tlpContent;
        private FlowLayoutPanel flpHeader;
        private Label lblRoomName;
        private ComboBox cboRoom;
        private Label lblCriticalBanner;
        private Panel pnlPages;
        private TableLayoutPanel tlpDashboard;
        private TableLayoutPanel tlpSensors;
        private GroupBox grpAlerts;
        private TableLayoutPanel tlpAlerts;
        private ListView lvAlerts;
        private ColumnHeader colTime;
        private ColumnHeader colRoom;
        private ColumnHeader colSensor;
        private ColumnHeader colValue;
        private ColumnHeader colLevel;
        private Button btnAcknowledge;
        private DataGridView dgvAlarms;
        private Label lblControlsPage;
        private Label lblHistoricalDataPage;
        private Label lblSettingsPage;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblDatabase;
        private System.Windows.Forms.Timer tmrPoll;
    }
}
