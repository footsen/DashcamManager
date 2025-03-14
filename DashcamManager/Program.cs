using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace DashcamManager
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string dashcamIPAddress = "192.168.5.36";   // reasonable default
            string dashcamDownloadFolder = @"d:\localdashcam";   // reasonable default

            using (GetDefaultValuesForm defaultValues = new GetDefaultValuesForm(dashcamIPAddress, dashcamDownloadFolder))
            {
                if (defaultValues.ShowDialog() == DialogResult.OK)
                {
                    dashcamIPAddress = defaultValues.dashcamIPAddress;
                    dashcamDownloadFolder = defaultValues.dashcamDownloadFolder;
                }
                else
                {
                    return; // Exit if user cancels
                }
            }

            {
                Application.Run(new DashcamManagerForm(dashcamIPAddress, dashcamDownloadFolder));
            }
        }
    }
}