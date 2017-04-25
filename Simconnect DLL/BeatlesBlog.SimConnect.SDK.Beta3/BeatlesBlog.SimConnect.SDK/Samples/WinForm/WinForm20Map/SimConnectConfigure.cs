using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormMap
{
    public partial class SimConnectConfigure : Form
    {
        public string ServerName
        {
            get
            {
                return txtServerName.Text;
            }
            set
            {
                txtServerName.Text = value;
            }
        }

        public string ServerPort
        {
            get
            {
                return txtServerPort.Text;
            }
            set
            {
                txtServerPort.Text = value;
            }
        }

        public int ServerPortInt
        {
            get
            {
                return Int32.Parse(txtServerPort.Text);
            }
        }

        public SimConnectConfigure()
        {
            InitializeComponent();
        }

        public SimConnectConfigure(string serverName, string serverPort)
        {
            InitializeComponent();

            ServerName = serverName;
            ServerPort = serverPort;
        }
    }
}
