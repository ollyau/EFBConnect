using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

namespace EFBConnect
{
    enum ConnectionType
    {
        Broadcast,
        IPAddress
    }

    class MainViewModel : NotifyPropertyChanged
    {
        private SimConnectInstance sc;
        private bool scConnected = false;
        private Config cfg;
        private DispatcherTimer timer;

        public MainViewModel()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                cfg = new Config("EFBConnect.xml", new List<Type> { typeof(ConnectionType) });

                _connection = cfg.Get<ConnectionType>("ConnectionType", ConnectionType.Broadcast, true);
                _deviceIp = cfg.Get<string>("IPAddress", null, true);

                if (!string.IsNullOrEmpty(_deviceIp))
                {
                    SetIP(_deviceIp);
                }

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += timer_Tick;

                sc = new SimConnectInstance();
                sc.OpenEvent += sc_OnRecvOpenEvent;
                sc.DisconnectEvent += sc_OnRecvDisconnectEvent;

                if (SimConnectHelpers.IsLocalRunning)
                {
                    sc.Connect();
                }
                else
                {
                    ConnectedString = "Disconnected from Flight Simulator.";
                    timer.Start();
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (SimConnectHelpers.IsLocalRunning)
            {
                sc.Connect();
            }
        }

        private ConnectionType _connection = ConnectionType.Broadcast;
        public ConnectionType Connection
        {
            get { return _connection; }
            set
            {
                SetProperty(ref _connection, value);
                switch (value)
                {
                    case ConnectionType.Broadcast:
                        SetIP(null);
                        break;
                    case ConnectionType.IPAddress:
                        SetIpDialogEnabled = true;
                        break;
                }
            }
        }

        private string _currentIpSetting = "IP Address";
        public string CurrentIpSetting
        {
            get { return _currentIpSetting; }
            set { SetProperty(ref _currentIpSetting, value); }
        }

        private string _connectedString;
        public string ConnectedString
        {
            get { return _connectedString; }
            set { SetProperty(ref _connectedString, value); }
        }

        private bool SetIP(string value)
        {
            System.Net.IPAddress ipValue;
            if (!string.IsNullOrEmpty(value) && System.Net.IPAddress.TryParse(value, out ipValue))
            {
                CurrentIpSetting = string.Format("IP Address ({0})", value);
                ForeFlightUdp.Instance.SetDestination(ipValue);
                return true;
            }
            else
            {
                ForeFlightUdp.Instance.SetDestination(System.Net.IPAddress.Broadcast);
                return false;
            }
        }

        private void sc_OnRecvOpenEvent(object sender, OpenEventArgs e)
        {
            timer.Stop();
            scConnected = true;
            ConnectedString = string.Format("Connected to {0}.", e.SimulatorName);
        }

        private void sc_OnRecvDisconnectEvent(object sender, EventArgs e)
        {
            timer.Start();
            scConnected = false;
            ConnectedString = "Disconnected from Flight Simulator.";
        }

        internal void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cfg.Set("ConnectionType", _connection);
            cfg.Set("IPAddress", _deviceIp);
            cfg.Save();
            if (scConnected)
            {
                sc.Disconnect();
            }
        }

        #region Message Dialog Bindings

        private void MessageDialogShow(string content, string title = null)
        {
            MessageDialogTitle = title;
            MessageDialogContent = content;
            MessageDialogEnabled = true;
        }

        private bool _overlayEnabled = false;
        public bool OverlayEnabled
        {
            get { return _overlayEnabled; }
            set { SetProperty(ref _overlayEnabled, value); }
        }

        private bool _messageDialogEnabled = false;
        public bool MessageDialogEnabled
        {
            get { return _messageDialogEnabled; }
            set { SetProperty(ref _messageDialogEnabled, value); OverlayEnabled = value; }
        }

        private string _messageDialogTitle;
        public string MessageDialogTitle
        {
            get { return _messageDialogTitle; }
            set { SetProperty(ref _messageDialogTitle, value); }
        }

        private string _messageDialogContent;
        public string MessageDialogContent
        {
            get { return _messageDialogContent; }
            set { SetProperty(ref _messageDialogContent, value); }
        }

        private ICommand _messageDialogClose;
        public ICommand MessageDialogClose
        {
            get
            {
                if (_messageDialogClose == null)
                {
                    _messageDialogClose = new RelayCommand((param) => MessageDialogEnabled = false);
                }
                return _messageDialogClose;
            }
        }

        private bool _ipSetDialogEnabled = false;
        public bool SetIpDialogEnabled
        {
            get { return _ipSetDialogEnabled; }
            set { SetProperty(ref _ipSetDialogEnabled, value); OverlayEnabled = value; }
        }

        private string _deviceIp;
        public string DeviceIp
        {
            get { return _deviceIp; }
            set
            {
                SetProperty(ref _deviceIp, value);
            }
        }

        private ICommand _setIpCommand;
        public ICommand SetIpCommand
        {
            get
            {
                if (_setIpCommand == null)
                {
                    _setIpCommand = new RelayCommand(param =>
                    {
                        if (SetIP(_deviceIp))
                        {
                            SetIpDialogEnabled = false;
                        }
                        else
                        {
                            _connection = ConnectionType.Broadcast;
                            OnPropertyChanged("Connection");
                            CurrentIpSetting = "IP Address";
                            SetIpDialogEnabled = false;
                            MessageDialogShow("EFB Connect will fall back to an all device broadcast.", "IP address invalid");
                            DeviceIp = null;
                        }
                    });
                }
                return _setIpCommand;
            }
        }

        #endregion
    }
}
