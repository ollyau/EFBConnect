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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BeatlesBlog.SimConnect;

namespace ClientDataTest
{
    /// <summary>
    /// Interaction logic for ClientDataHost.xaml
    /// </summary>
    public partial class ClientDataHost : UserControl
    {
        SimConnect sc;

        enum Requests
        {
            HostAPI0,
            HostAPI1,
        }

        public ClientDataHost()
        {
            InitializeComponent();

            sc = new SimConnect(this.Dispatcher);

            sc.OnRecvException += new SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(sc_OnRecvClientData);
        }

        public void ConnectLocal()
        {
            sc.Open("ClientDataHost");
        }

        public void Connect(string server, int port, bool bIsIPV6)
        {
            sc.Open("ClientDataHost", server, port, bIsIPV6);
        }

        public void Disconnect()
        {
            sc.Close();
        }

        void sc_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            switch ((Requests)data.dwRequestID)
            {
                case Requests.HostAPI0:
                    {
                        HostAPI0 api0 = (HostAPI0)data.dwData;
                    }
                    break;

                case Requests.HostAPI1:
                    {
                        HostAPI1 api1 = (HostAPI1)data.dwData;
                        if (api1.RequestedDataIndex < 0 ||
                            api1.RequestedDataIndex > 4)
                        {
                            HostException1 exc = new HostException1();
                            exc.RequestorID = api1.RequestorID;
                            exc.ReqeustedDataIndex = api1.RequestedDataIndex;
                            exc.szErrorMessage = "HostAPI1: RequestedDataIndex out of range";
                            sender.SetClientData(exc);
                            return;
                        }

                        HostResponse1 resp = new HostResponse1();

                        resp.RequestorID = api1.RequestorID;
                        resp.RequestedDataIndex = api1.RequestedDataIndex;
                        switch (api1.RequestedDataIndex)
                        {
                            case 0:
                                // return the highest valid value for RequestDataIndex
                                resp.RequestedDataValue = 4;
                                break;

                            case 1:
                                // return var 1
                                resp.RequestedDataValue = double.Parse(txtVal1.Text);
                                break;

                            case 2:
                                // return var 2
                                resp.RequestedDataValue = double.Parse(txtVal2.Text);
                                break;

                            case 3:
                                // return var 3
                                resp.RequestedDataValue = double.Parse(txtVal3.Text);
                                break;

                            case 4:
                                // return var 4
                                resp.RequestedDataValue = double.Parse(txtVal4.Text);
                                break;
                        }
                        sender.SetClientData(resp);
                    }
                    break;
            }
        }

        void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            // setup incoming request area for API 1 and setup recurring Request
            sender.CreateClientData(typeof(HostAPI1));
            sender.RequestClientData(Requests.HostAPI1, SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, typeof(HostAPI1));

            // setup regular response area for API1
            sender.CreateClientData(typeof(HostResponse1));
            // setup exception response area for API1
            sender.CreateClientData(typeof(HostException1));

            // setup read-only data structure for API 2
            sender.CreateClientData(typeof(HostAPI2));
        }

        void sc_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            HostAPI2 api = new HostAPI2();

            api.StringValue = txtVal21.Text;
            api.BooleanValue = chkVal22.IsChecked.Value;
            api.IntValue = Int32.Parse(txtVal23.Text);
            api.DoubleValue = Double.Parse(txtVal24.Text);

            sc.SetClientData(api);
        }

        [ClientDataStruct("ClientDataHost.HostAPI0")]
        public class HostAPI0
        {
            [ClientDataItem(0)]
            public Byte Int8Item;

            [ClientDataItem(1)]
            public Int16 Int16Item;

            [ClientDataItem(2)]
            public Int32 Int32Item;

            [ClientDataItem(3)]
            public Int64 Int64Item;

            [ClientDataItem(4)]
            public Single Float32Item;

            [ClientDataItem(5)]
            public Double Float64Item;
        }

        [ClientDataStruct("ClientDataHost.HostResponse0", true)]
        public class HostResponse0
        {
        }

        //
        // Host API 1 uses a request / response / exception style interface
        //

        [ClientDataStruct("ClientDataHost.HostAPI1")]
        public class HostAPI1
        {
            [ClientDataItem(0)]
            public Guid RequestorID;

            [ClientDataItem(1)]
            public Int32 RequestedDataIndex;
        }

        [ClientDataStruct("ClientDataHost.HostResponse1", true)]
        public class HostResponse1
        {
            [ClientDataItem(0)]
            public Guid RequestorID;

            [ClientDataItem(1)]
            public Int32 RequestedDataIndex;

            [ClientDataItem(2)]
            public Double RequestedDataValue;
        }

        [ClientDataStruct("ClientDataHost.HostException1", true)]
        public class HostException1
        {
            [ClientDataItem(0)]
            public Guid RequestorID;

            [ClientDataItem(1)]
            public Int32 ReqeustedDataIndex;

            [ClientDataItem(2, 256)]
            public string szErrorMessage;
        }

        //
        // Host API 2 exposes a set of read-only data
        //

        [ClientDataStruct("ClientDataHost.HostAPI2", true)]
        public class HostAPI2
        {
            [ClientDataItem(0, 64)]
            public string StringValue;

            [ClientDataItem(1)]
            public Boolean BooleanValue;

            [ClientDataItem(2)]
            public Int32 IntValue;

            [ClientDataItem(3)]
            public Double DoubleValue;
        }
    }
}
