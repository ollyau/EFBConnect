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

    class MainViewModel : ObservableObject
    {
        private EFBConnectClient efbConnect;
        private Config cfg;
        private DispatcherTimer timer;

        public MainViewModel()
        {
#if DEBUG
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
#endif
                cfg = new Config("EFBConnect.xml", new List<Type> { typeof(ConnectionType) });

                _connection = cfg.Get<ConnectionType>("ConnectionType", ConnectionType.Broadcast, true);
                _deviceIp = cfg.Get<string>("IPAddress", null, true);

                if (!string.IsNullOrEmpty(_deviceIp))
                {
                    SetIP(_deviceIp);
                }

                ConnectionStatus = "Disconnected from flight simulator.";

                efbConnect = new EFBConnectClient();
                efbConnect.PropertyChanged += Simulator_PropertyChanged;
                efbConnect.Initialize();

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += timer_Tick;
                timer.Start();
#if DEBUG
            }
#endif
        }

        private void Simulator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (efbConnect.Connected)
            {
                timer.Stop();
                ConnectionStatus = $"Connected to {efbConnect.SimulatorName}.";
            }
            else
            {
                timer.Start();
                ConnectionStatus = "Disconnected from flight simulator.";
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            efbConnect.Initialize();
        }

        private ConnectionType _connection = ConnectionType.Broadcast;
        public ConnectionType Connection
        {
            get { return _connection; }
            set
            {
                SetField(ref _connection, value);
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
            set { SetField(ref _currentIpSetting, value); }
        }

        private string _connectedString;
        public string ConnectionStatus
        {
            get { return _connectedString; }
            set { SetField(ref _connectedString, value); }
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

        internal void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cfg.Set("ConnectionType", _connection);
            cfg.Set("IPAddress", _deviceIp);
            cfg.Save();
            efbConnect.Uninitialize();
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
            set { SetField(ref _overlayEnabled, value); }
        }

        private bool _messageDialogEnabled = false;
        public bool MessageDialogEnabled
        {
            get { return _messageDialogEnabled; }
            set { SetField(ref _messageDialogEnabled, value); OverlayEnabled = value; }
        }

        private string _messageDialogTitle;
        public string MessageDialogTitle
        {
            get { return _messageDialogTitle; }
            set { SetField(ref _messageDialogTitle, value); }
        }

        private string _messageDialogContent;
        public string MessageDialogContent
        {
            get { return _messageDialogContent; }
            set { SetField(ref _messageDialogContent, value); }
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
            set { SetField(ref _ipSetDialogEnabled, value); OverlayEnabled = value; }
        }

        private string _deviceIp;
        public string DeviceIp
        {
            get { return _deviceIp; }
            set
            {
                SetField(ref _deviceIp, value);
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
