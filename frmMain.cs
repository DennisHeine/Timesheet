using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NativeWifi;
using System.IO;

namespace WorksheetLog
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
        }


        public delegate void ControlStringConsumer(Control control, string text, bool ConnectOnWIFI);  // defines a delegate type

        public void SetText(Control control, string text, bool ConnectOnWIFI)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new ControlStringConsumer(SetText), new object[] { control, text, ConnectOnWIFI });  // invoking itself
            }
            else
            {
                tbWiFi.Text = text;
                rbProgramm.Checked = !ConnectOnWIFI;
                rbWiFi.Checked = ConnectOnWIFI;
                //control.Text = text;      // the "functional part", executing only on the main thread
            }
        }

        cTimes times = new cTimes();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {


            if (File.Exists(".\\data.dat"))
                times = times.Load(".\\data.dat");
            SetText(tbWiFi, times.WLANID, times.connectOnWLAN);
            WlanClient client = new WlanClient();
            while (true)
            {

                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    // Lists all networks with WEP security
                    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                    foreach (Wlan.WlanAvailableNetwork network in networks)
                    {
                        String nam = network.profileName;
                        //if (nam == "wlan_lab_01")
                        if (nam == times.WLANID || rbProgramm.Checked)
                        {
                            times.addTimestamp();
                            times.Save(times, ".\\data.dat");
                        }
                    }
                }


                System.Threading.Thread.Sleep(1000);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            this.WindowState = FormWindowState.Minimized;
        }

        private void rbWiFi_CheckedChanged(object sender, EventArgs e)
        {
            times.connectOnWLAN = rbWiFi.Checked;
            if (rbWiFi.Checked)
                tbWiFi.Enabled = true;
            else
                tbWiFi.Enabled = false;
        }

        private void tbWiFi_TextChanged(object sender, EventArgs e)
        {
            times.WLANID = tbWiFi.Text;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.notifyIcon1.ContextMenu = contextMenu1;
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void bnExport_Click(object sender, EventArgs e)
        {
            times.Export(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);;
        }
    }
}
