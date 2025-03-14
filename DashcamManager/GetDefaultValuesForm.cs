using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DashcamManager
{
    public partial class GetDefaultValuesForm: Form
    {
        public string dashcamIPAddress { get; private set; }
        public string dashcamDownloadFolder { get; private set; }

        public GetDefaultValuesForm(string dashcamIPAddress, string dashcamDownloadFolder)
        {
            this.dashcamIPAddress = dashcamIPAddress;
            this.dashcamDownloadFolder = dashcamDownloadFolder;
            InitializeComponent();
            txtDashcamIP.Text = dashcamIPAddress;
            txtDownloadFolder.Text = dashcamDownloadFolder;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                // todo: folder picker button

                dashcamIPAddress = txtDashcamIP.Text;
                dashcamDownloadFolder = txtDownloadFolder.Text;
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Close application if user cancels
        }

        private bool ValidateInputs()
        {
            // check that IP address is valid
            if (! IPAddress.TryParse(txtDashcamIP.Text, out _))
            {
                MessageBox.Show(String.Format("Invalid IP address {0}. Please try again.", txtDashcamIP.Text), "Invalid entry", MessageBoxButtons.OK);
                {
                    return false;
                }
            }

            // check that we can ping it
            bool pingOK = false;
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(txtDashcamIP.Text, 1000); // 1s timeout
                    pingOK = (reply.Status == IPStatus.Success);
                }
            }
            catch
            {
                // fall through
            }
            if (!pingOK)
            {
                MessageBox.Show(String.Format("Cannot ping IP address {0}.\nPlease make sure that your dashcam is powered on and Wi-Fi Station Mode is enabled, then try again.", txtDashcamIP.Text), "Invalid entry", MessageBoxButtons.OK);
                return false;
            }

            bool directoryOK = false;
            try
            {
                // create the download folder if it does not exist
                if (Directory.Exists(txtDownloadFolder.Text))
                {
                    directoryOK = true;
                }
                else
                {
                    Directory.CreateDirectory(txtDownloadFolder.Text);

                    // confirm that the download folder exists
                    if (Directory.Exists(txtDownloadFolder.Text))
                    {
                        directoryOK = true;
                    }
                }
            }
            catch
            {
                // fall through
            }
            if (!directoryOK)
            {
                MessageBox.Show(String.Format("Invalid folder {0}, please try again.", txtDownloadFolder.Text), "Invalid entry", MessageBoxButtons.OK);
                return false;
            }


            // all good
            return true;
        }
    }
}
