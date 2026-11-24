
using System;
using System.Windows.Forms;

namespace HCRMS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // =====================================
            // P17 - Launch the SCADA dashboard
            // =====================================

            // The app opens straight to the dashboard, without test pop-ups
            Application.Run(new SCADADashboard());
        }
    }
}
