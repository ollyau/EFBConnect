using BeatlesBlog.SimConnect;
using System;

namespace EFBConnect
{
    class EFBConnectClient : SimConnectClient
    {
        private ForeFlightUdp ffUdp;

        public EFBConnectClient() : base("EFBConnect")
        {
            Client.OnRecvException += OnRecvException;
            Client.OnRecvEvent += OnRecvEvent;
            Client.OnRecvEventObjectAddremove += OnRecvEventObjectAddremove;
            Client.OnRecvSimobjectData += OnRecvSimobjectData;
            Client.OnRecvSimobjectDataBytype += OnRecvSimobjectDataBytype;
        }

        protected override void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            // call parent class for default behavior
            base.OnRecvOpen(sender, data);

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

            Client.RequestDataOnUserSimObject(Requests.UserPosition, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(Position));
            Client.RequestDataOnSimObjectType(Requests.TrafficEnumerate, 200000, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT & SIMCONNECT_SIMOBJECT_TYPE.HELICOPTER, typeof(TrafficInfo));
            Client.SubscribeToSystemEvent(Events.ObjectAdded, "ObjectAdded");
            Client.SubscribeToSystemEvent(Events.SixHz, "6Hz");
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var exceptionName = Enum.GetName(typeof(SIMCONNECT_EXCEPTION), data.dwException);
            var message = $"{exceptionName} (Exception = {data.dwException}, SendID = {data.dwSendID}, Index = {data.dwIndex})";
            Log.Instance.Warning(message);
            Client.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 10.0f, Requests.DisplayText, $"{ApplicationName} SimConnect Exception: {message}");
        }

        private void OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            if ((Events)data.uEventID == Events.SixHz)
            {
                Client.RequestDataOnUserSimObject(Requests.UserAttitude, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(Attitude));
            }
        }

        private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
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

        private void OnRecvEventObjectAddremove(SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data)
        {
            if ((Events)data.uEventID == Events.ObjectAdded &&
                (data.eObjType == SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT || data.eObjType == SIMCONNECT_SIMOBJECT_TYPE.HELICOPTER) &&
                (data.dwData != SimConnect.USER_SIMOBJECT))
            {
                Client.RequestDataOnSimObject(
                    (Requests)((uint)Requests.TrafficInfoBase + (uint)data.dwData),
                    data.dwData, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(TrafficInfo));
            }
        }

        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if ((Requests)data.dwRequestID == Requests.TrafficEnumerate && (data.dwObjectID != SimConnect.USER_SIMOBJECT))
            {
                Client.RequestDataOnSimObject(
                    (Requests)((uint)Requests.TrafficInfoBase + (uint)data.dwObjectID),
                    data.dwObjectID, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, typeof(TrafficInfo));
            }
        }
    }
}