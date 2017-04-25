using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimConnectTestWPF
{
    /// <summary>
    /// Interaction logic for SimConnectConfigure.xaml
    /// </summary>
    public partial class SimConnectConfigure : Window, System.ComponentModel.INotifyPropertyChanged
    {
        public enum NetworkingMode
        {
            IPv4,
            IPv6,
            Pipe,
        }

        private bool bOKPressed = false;
        public bool OKPressed
        {
            get
            {
                return bOKPressed;
            }
        }

        public NetworkingMode NetworkMode
        {
            get
            {
                return (NetworkingMode)cbNetworkMode.SelectedIndex; ;
            }
            set
            {
                cbNetworkMode.SelectedIndex = (int)value;
                RaisePropertyChanged("NetworkMode");
            }
        }

        public string NetworkServer
        {
            get
            {
                return txtServerName.Text;
            }
            set
            {
                txtServerName.Text = value;
                RaisePropertyChanged("NetworkServer");
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
                RaisePropertyChanged("NetworkPort");
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            bOKPressed = false;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bOKPressed = true;
            this.Close();
        }

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
