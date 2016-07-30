using BeatlesBlog.SimConnect;
using System;

namespace EFBConnect
{
    class SimConnectHelpers
    {
        public static bool IsLocalRunning
        {
            get { return LookupDefaultPortNumber("SimConnect_Port_IPv4") != 0 || LookupDefaultPortNumber("SimConnect_Port_IPv6") != 0; }
        }

        public static int LookupDefaultPortNumber(string strValueName)
        {
            string[] simulators = {
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft Games\Flight Simulator",
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft ESP",
                                      @"HKEY_CURRENT_USER\Software\LockheedMartin\Prepar3D",
                                      @"HKEY_CURRENT_USER\Software\Lockheed Martin\Prepar3D v2",
                                      @"HKEY_CURRENT_USER\Software\Lockheed Martin\Prepar3D v3",
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft Games\Flight Simulator - Steam Edition"
                                  };
            foreach (var sim in simulators)
            {
                var value = (string)Microsoft.Win32.Registry.GetValue(sim, strValueName, null);
                if (!string.IsNullOrEmpty(value))
                {
                    int port = int.Parse(value);
                    if (port != 0) { return port; }
                }
            }
            return 0;
        }
    }

    public class OpenEventArgs : EventArgs
    {
        public string SimulatorName { get; private set; }
        public OpenEventArgs(string SimulatorName)
        {
            this.SimulatorName = SimulatorName;
        }
    }

    class SimConnectInstance
    {
        private SimConnect sc = null;
        private ForeFlightUdp ffUdp;
        private Log log;

        public EventHandler<OpenEventArgs> OpenEvent;
        public EventHandler DisconnectEvent;

        private const string appName = "EFBConnect";

        protected virtual void OnRaiseOpenEvent(OpenEventArgs e)
        {
            EventHandler<OpenEventArgs> handler = OpenEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseDisconnectEvent(EventArgs e)
        {
            EventHandler handler = DisconnectEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public SimConnectInstance()
        {
            log = Log.Instance;
            sc = new SimConnect(null);

            sc.OnRecvOpen += sc_OnRecvOpen;
            sc.OnRecvException += sc_OnRecvException;
            sc.OnRecvQuit += sc_OnRecvQuit;

            sc.OnRecvEvent += sc_OnRecvEvent;
            sc.OnRecvEventObjectAddremove += sc_OnRecvEventObjectAddremove;
            sc.OnRecvSimobjectData += sc_OnRecvSimobjectData;
            sc.OnRecvSimobjectDataBytype += sc_OnRecvSimobjectDataBytype;
        }

        public void Connect()
        {
            if (SimConnectHelpers.IsLocalRunning)
            {
                try
                {
                    log.Info("Opening SimConnect connection.");
                    sc.Open(appName);
                }
                catch (SimConnect.SimConnectException ex)
                {
                    log.Warning(string.Format("Local connection failed.\r\n{0}", ex.ToString()));
                    try
                    {
                        bool ipv6support = System.Net.Sockets.Socket.OSSupportsIPv6;
                        log.Info("Opening SimConnect connection " + (ipv6support ? "(IPv6)." : "(IPv4)."));
                        int scPort = ipv6support ? SimConnectHelpers.LookupDefaultPortNumber("SimConnect_Port_IPv6") : SimConnectHelpers.LookupDefaultPortNumber("SimConnect_Port_IPv4");
                        if (scPort == 0) { throw new SimConnect.SimConnectException("Invalid port."); }
                        sc.Open(appName, null, scPort, ipv6support);
                    }
                    catch (SimConnect.SimConnectException innerEx)
                    {
                        log.Error(string.Format("Local connection failed.\r\n{0}", innerEx.ToString()));
                    }
                }
            }
            else
            {
                log.Warning("Flight Simulator must be running in order to connect to SimConnect.");
            }
        }

        public void Disconnect()
        {
            log.Info("Closing SimConnect connection.");
            sc.Close();
            OnRaiseDisconnectEvent(EventArgs.Empty);
        }

        void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            log.Info(string.Format("Connected to {0}\r\n    Simulator Version:\t{1}.{2}.{3}.{4}\r\n    SimConnect Version:\t{5}.{6}.{7}.{8}",
                data.szApplicationName, data.dwApplicationVersionMajor, data.dwApplicationVersionMinor, data.dwApplicationBuildMajor, data.dwApplicationBuildMinor,
                data.dwSimConnectVersionMajor, data.dwSimConnectVersionMinor, data.dwSimConnectBuildMajor, data.dwSimConnectBuildMinor));

            OnRaiseOpenEvent(new OpenEventArgs(data.szApplicationName));

            string simIdent = "MSFS";

            if (data.szApplicationName.Contains("Flight Simulator X"))
            {
                simIdent = "FSX";
            }
            else if (data.szApplicationName.Contains("ESP"))
            {
                simIdent = "ESP";
            }
            else if (data.szApplicationName.Contains("Prepar3D"))
            {
                simIdent = "P3D";
            }

            ffUdp = ForeFlightUdp.Instance;
            ffUdp.SetSimulator(simIdent);

            sc.RequestDataOnUserSimObject(Requests.UserPosition, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(Position));
            sc.RequestDataOnSimObjectType(Requests.TrafficEnumerate, 200000, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT & SIMCONNECT_SIMOBJECT_TYPE.HELICOPTER, typeof(TrafficInfo));

            sc.SubscribeToSystemEvent(Events.ObjectAdded, "ObjectAdded");
            sc.SubscribeToSystemEvent(Events.SixHz, "6Hz");
        }

        void sc_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            log.Warning(string.Format("OnRecvException: {0} ({1}) {2} {3}", data.dwException.ToString(), Enum.GetName(typeof(SIMCONNECT_EXCEPTION), data.dwException), data.dwSendID.ToString(), data.dwIndex.ToString()));
            sc.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 10.0f, Requests.DisplayText, string.Format("{0} SimConnect Exception: {1} ({2})", appName, data.dwException.ToString(), Enum.GetName(typeof(SIMCONNECT_EXCEPTION), data.dwException)));
        }

        void sc_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            log.Info("OnRecvQuit - simulator has closed.");
            Disconnect();
        }

        void sc_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            if ((Events)data.uEventID == Events.SixHz)
            {
                sc.RequestDataOnUserSimObject(Requests.UserAttitude, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(Attitude));
            }
        }

        void sc_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((Requests)data.dwRequestID)
            {
                case Requests.UserAttitude:
                    ffUdp.Send((Attitude)data.dwData);
                    return;
                case Requests.UserPosition:
                    ffUdp.Send((Position)data.dwData);
                    return;
                default:
                    if ((Requests)data.dwRequestID == (Requests)((int)Requests.TrafficInfoBase + data.dwObjectID))
                    {
                        ffUdp.Send((TrafficInfo)data.dwData, data.dwObjectID);
                    }
                    return;
            }
        }

        void sc_OnRecvEventObjectAddremove(SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data)
        {
            if ((Events)data.uEventID == Events.ObjectAdded &&
                (data.eObjType == SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT || data.eObjType == SIMCONNECT_SIMOBJECT_TYPE.HELICOPTER) &&
                (data.dwData != SimConnect.USER_SIMOBJECT))
            {
                sc.RequestDataOnSimObject(
                    (Requests)((uint)Requests.TrafficInfoBase + (uint)data.dwData),
                    data.dwData, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(TrafficInfo));
            }
        }

        void sc_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if ((Requests)data.dwRequestID == Requests.TrafficEnumerate && (data.dwObjectID != SimConnect.USER_SIMOBJECT))
            {
                sc.RequestDataOnSimObject(
                    (Requests)((uint)Requests.TrafficInfoBase + (uint)data.dwObjectID),
                    data.dwObjectID, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(TrafficInfo));
            }
        }
    }
}