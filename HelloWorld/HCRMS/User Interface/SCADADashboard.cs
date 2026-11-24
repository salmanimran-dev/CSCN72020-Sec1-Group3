using System.Media;
using HCRMS.Core;
using HCRMS.Data;
using HCRMS.Models;

namespace HCRMS
{
    // P17 - Main SCADA dashboard (IR-01), laid out as in the project pitch
    public partial class SCADADashboard : Form
    {
        // Sidebar colours for the selected and unselected pages
        private static readonly Color NavSelected = Color.FromArgb(31, 111, 178);
        private static readonly Color NavNormal = Color.FromArgb(18, 38, 63);

        // Saves readings, alarms and events to SQLite
        private readonly DatabaseManager database;

        // Reads new values for every room and raises alerts
        private readonly ScadaMonitor monitor;

        // One tile for each sensor in the selected room
        private readonly List<SensorTile> tiles = new List<SensorTile>();

        // Remembers what the alarm history last showed, so it doesn't reload
        // (and jump back to the top) every 2 seconds when nothing changed
        private string lastAlarmsKey = "";

        public SCADADashboard()
        {
            InitializeComponent();

            // Create the ICU, NICU and operating room with their seven sensors
            List<Room> rooms = RoomFactory.CreateDefaultRooms();

            // Use CSV files from the SimulatedData folder if there are any,
            // otherwise use the built-in simulator
            string dataFolder = Path.Combine(AppContext.BaseDirectory, "SimulatedData");
            IReadingSource source = ReadingSources.Create(dataFolder, rooms);

            database = new DatabaseManager(DatabaseManager.DefaultDatabasePath());
            monitor = new ScadaMonitor(rooms, source, database);
            monitor.AlertRaised += Monitor_AlertRaised;

            foreach (Room room in monitor.Rooms)
            {
                cboRoom.Items.Add($"{room.RoomID}  ({room.RoomType})");
            }

            // Save the rooms and sensors, then read the first values
            monitor.Start();
            monitor.Poll();

            // Selecting the first room draws the tiles
            cboRoom.SelectedIndex = 0;

            tmrPoll.Start();
        }

        // Returns the room picked in the room selector
        private Room SelectedRoom
        {
            get { return monitor.Rooms[Math.Max(cboRoom.SelectedIndex, 0)]; }
        }

        // =====================================
        // Events
        // =====================================

        // Reads new values every 2 seconds (NFR-01)
        private void tmrPoll_Tick(object? sender, EventArgs e)
        {
            tmrPoll.Stop();
            try
            {
                monitor.Poll();
                RefreshDashboard(false);
            }
            catch (Exception ex)
            {
                lblDatabase.Text = $"Update failed: {ex.Message}. Monitoring will try again in 2 seconds.";
            }
            finally
            {
                tmrPoll.Start();
            }
        }

        private void cboRoom_SelectedIndexChanged(object? sender, EventArgs e)
        {
            Room room = SelectedRoom;
            lblRoomName.Text = room.RoomID;

            BuildSensorTiles(room);
            RefreshDashboard(true);
        }

        private void btnNavDashboard_Click(object? sender, EventArgs e)
        {
            ShowPage(btnNavDashboard);
        }

        private void btnNavAlarmHistory_Click(object? sender, EventArgs e)
        {
            ShowPage(btnNavAlarmHistory);
        }

        private void btnNavControls_Click(object? sender, EventArgs e)
        {
            ShowPage(btnNavControls);
        }

        private void btnNavHistoricalData_Click(object? sender, EventArgs e)
        {
            ShowPage(btnNavHistoricalData);
        }

        private void btnNavSettings_Click(object? sender, EventArgs e)
        {
            ShowPage(btnNavSettings);
        }

        private void btnAcknowledge_Click(object? sender, EventArgs e)
        {
            List<SensorAlert> alerts = new List<SensorAlert>();
            foreach (ListViewItem item in lvAlerts.SelectedItems)
            {
                if (item.Tag is SensorAlert alert)
                    alerts.Add(alert);
            }

            if (alerts.Count == 0)
            {
                MessageBox.Show(
                    "Select one or more alerts in the list first, then click \"Acknowledge selected\".",
                    "No alert selected");
                return;
            }

            // The acknowledgement and its time are logged (FR-07)
            foreach (SensorAlert alert in alerts)
            {
                monitor.Acknowledge(alert);
            }

            RefreshDashboard(true);
        }

        // Plays a sound for critical alerts (UR-02)
        private void Monitor_AlertRaised(object? sender, SensorAlert alert)
        {
            if (alert.Level == "Critical")
                SystemSounds.Hand.Play();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            tmrPoll.Stop();
            monitor.Stop();
            base.OnFormClosing(e);
        }

        // =====================================
        // Dashboard updates
        // =====================================

        // Shows the page for the clicked sidebar button and highlights that button
        private void ShowPage(Button navButton)
        {
            tlpDashboard.Visible = navButton == btnNavDashboard;
            dgvAlarms.Visible = navButton == btnNavAlarmHistory;
            lblControlsPage.Visible = navButton == btnNavControls;
            lblHistoricalDataPage.Visible = navButton == btnNavHistoricalData;
            lblSettingsPage.Visible = navButton == btnNavSettings;

            Button[] navButtons = { btnNavDashboard, btnNavControls, btnNavAlarmHistory, btnNavHistoricalData, btnNavSettings };
            foreach (Button button in navButtons)
            {
                button.BackColor = button == navButton ? NavSelected : NavNormal;
            }

            RefreshAlarmHistory(true);
        }

        // Creates one tile for each sensor in the room
        private void BuildSensorTiles(Room room)
        {
            tlpSensors.SuspendLayout();
            tlpSensors.Controls.Clear();
            foreach (SensorTile oldTile in tiles)
            {
                oldTile.Dispose();
            }
            tiles.Clear();

            foreach (Sensor sensor in room.Sensors)
            {
                SensorTile tile = new SensorTile();
                tiles.Add(tile);
                tlpSensors.Controls.Add(tile);
            }
            tlpSensors.ResumeLayout(true);
        }

        private void RefreshDashboard(bool forceHistory)
        {
            Room room = SelectedRoom;

            for (int i = 0; i < tiles.Count && i < room.Sensors.Count; i++)
            {
                tiles[i].ShowSensor(room.Sensors[i]);
            }

            UpdateAlertList();
            UpdateCriticalBanner();
            RefreshAlarmHistory(forceHistory);
            UpdateStatusBar();
        }

        // Lists all unacknowledged alerts: Critical first, then Failed, then Warning
        private void UpdateAlertList()
        {
            // Keep the user's selection when the list is rebuilt
            HashSet<SensorAlert> selected = new HashSet<SensorAlert>();
            foreach (ListViewItem item in lvAlerts.SelectedItems)
            {
                if (item.Tag is SensorAlert alert)
                    selected.Add(alert);
            }

            List<SensorAlert> ordered = monitor.ActiveAlerts
                .OrderBy(a => StatusStyle.For(a.Level).Rank)
                .ThenByDescending(a => a.Timestamp)
                .ToList();

            lvAlerts.BeginUpdate();
            lvAlerts.Items.Clear();
            foreach (SensorAlert alert in ordered)
            {
                ListViewItem item = new ListViewItem(new[]
                {
                    alert.Timestamp.ToString("HH:mm:ss"),
                    alert.RoomID,
                    alert.SensorName,
                    alert.ValueText,
                    StatusStyle.For(alert.Level).Label
                });
                item.Tag = alert;
                lvAlerts.Items.Add(item);
                item.Selected = selected.Contains(alert);
            }
            lvAlerts.EndUpdate();

            btnAcknowledge.Enabled = ordered.Count > 0;
        }

        // Shows critical alerts in red at the top of the screen (UR-02)
        private void UpdateCriticalBanner()
        {
            SensorAlert? newest = monitor.ActiveAlerts
                .Where(a => a.Level == "Critical")
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefault();

            if (newest == null)
            {
                lblCriticalBanner.Visible = false;
                return;
            }

            lblCriticalBanner.Text =
                $"CRITICAL:  {newest.RoomID} – {newest.SensorName} {newest.ValueText} at {newest.Timestamp:HH:mm:ss}";
            lblCriticalBanner.Visible = true;
        }

        // Shows the saved alarms for the selected room (read-only, NFR-05)
        private void RefreshAlarmHistory(bool force)
        {
            if (!dgvAlarms.Visible)
                return;

            string roomId = SelectedRoom.RoomID;
            List<AlarmRecord> alarms = database.GetAlarms(roomId, 300);
            int acknowledged = alarms.Count(a => a.Acknowledged);
            string key = $"{roomId}|{alarms.FirstOrDefault()?.AlarmID}|{alarms.Count}|{acknowledged}";

            if (force || key != lastAlarmsKey)
            {
                dgvAlarms.DataSource = alarms;
                lastAlarmsKey = key;
            }
        }

        // Shows whether the database is working, in plain language (UR-05)
        private void UpdateStatusBar()
        {
            if (database.PendingWriteCount > 0)
            {
                lblDatabase.ForeColor = Color.DarkRed;
                lblDatabase.Text =
                    $"Database unavailable – {database.PendingWriteCount} record(s) are kept in memory " +
                    "and will be saved automatically when it is available again.";
            }
            else if (database.LastError != null)
            {
                lblDatabase.ForeColor = Color.DarkRed;
                lblDatabase.Text = $"Database problem: {database.LastError}";
            }
            else
            {
                lblDatabase.ForeColor = SystemColors.ControlText;
                lblDatabase.Text = $"Saving to {database.DatabasePath}";
            }
        }
    }
}
