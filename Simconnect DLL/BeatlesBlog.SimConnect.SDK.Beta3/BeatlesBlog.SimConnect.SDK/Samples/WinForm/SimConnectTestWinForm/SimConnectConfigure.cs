using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimConnectTestWinForm
{
    public partial class SimConnectConfigure : Form
    {
        public enum NetworkingMode
        {
            IPv4,
            IPv6,
            Pipe,
        }

        public NetworkingMode NetworkMode
        {
            get
            {
                return (NetworkingMode)cboNetworkMode.SelectedIndex;
            }
            set
            {
                cboNetworkMode.SelectedIndex = (int)value;
            }
        }

        public string NetworkServer
        {
            get
            {
                return txtServerComputer.Text;
            }
            set
            {
                txtServerComputer.Text = value;
            }
        }

        public string NetworkPort
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

        public int NetworkPortInt
        {
            get
            {
                return Convert.ToInt32(NetworkPort);
            }
            set
            {
                NetworkPort = value.ToString();
            }
        }

        public SimConnectConfigure()
        {
            InitializeComponent();
        }
    }
}
