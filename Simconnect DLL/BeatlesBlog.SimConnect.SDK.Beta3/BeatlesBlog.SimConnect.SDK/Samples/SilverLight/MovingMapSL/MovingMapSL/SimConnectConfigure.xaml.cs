using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MovingMapSL
{
    public partial class SimConnectConfigure : UserControl
    {
        public SimConnectConfigure()
        {
            InitializeComponent();
        }

        public event EventHandler ConnectClicked;

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
                return Convert.ToInt32(txtServerPort.Text);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectClicked != null)
            {
                ConnectClicked(this, EventArgs.Empty);
            }
        }
    }
}
