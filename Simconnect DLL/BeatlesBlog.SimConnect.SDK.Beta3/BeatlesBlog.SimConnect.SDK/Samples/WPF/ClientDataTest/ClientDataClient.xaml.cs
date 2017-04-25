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
    /// Interaction logic for ClientDataClient.xaml
    /// </summary>
    public partial class ClientDataClient : UserControl
    {
        Guid guidClient = Guid.NewGuid();
        SimConnect sc;

        enum Requests
        {
            HostResponse1,
            HostException1,

            HostAPI2,
        }

        public ClientDataClient()
        {
            InitializeComponent();

            sc = new SimConnect(this.Dispatcher);

            sc.OnRecvException += new SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(sc_OnRecvClientData);
        }

        public void ConnectLocal()
        {
            sc.Open("ClientDataClient");
        }

        public void Connect(string server, int port, bool bIsIPV6)
        {
            sc.Open("ClientDataClient", server, port, bIsIPV6);
        }

        public void Disconnect()
        {
            sc.Close();
        }

        void sc_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            switch ((Requests)data.dwRequestID)
            {
                case Requests.HostResponse1:
                    {
                        HostResponse1 resp = (HostResponse1)data.dwData;
                        if (resp.RequestorID == guidClient)
                        {   // only process replies meant for us
                            switch (resp.RequestedDataIndex)
                            {
                                case 1:
                                    txtVal1.Text = resp.RequestedDataValue.ToString();
                                    break;

                                case 2:
                                    txtVal2.Text = resp.RequestedDataValue.ToString();
                                    break;

                                case 3:
                                    txtVal3.Text = resp.RequestedDataValue.ToString();
                                    break;

                                case 4:
                                    txtVal4.Text = resp.RequestedDataValue.ToString();
                                    break;

                                case 5:
                                    txtVal5.Text = resp.RequestedDataValue.ToString();
                                    break;
                            }
                        }
                    }
                    break;

                case Requests.HostException1:
                    {
                        HostException1 exc = (HostException1)data.dwData;
                        if (exc.RequestorID == guidClient)
                        {
                            switch (exc.ReqeustedDataIndex)
                            {
                                case 1:
                                    txtVal1.Text = exc.szErrorMessage;
                                    break;

                                case 2:
                                    txtVal2.Text = exc.szErrorMessage;
                                    break;

                                case 3:
                                    txtVal3.Text = exc.szErrorMessage;
                                    break;

                                case 4:
                                    txtVal4.Text = exc.szErrorMessage;
                                    break;

                                case 5:
                                    txtVal5.Text = exc.szErrorMessage;
                                    break;
                            }
                        }
                    }
                    break;

                case Requests.HostAPI2:
                    {
                        HostAPI2 api = (HostAPI2)data.dwData;

                        txtVal21.Text = api.StringValue;
                        txtVal22.Text = api.BooleanValue ? "True" : "False";
                        txtVal23.Text = api.IntValue.ToString();
                        txtVal24.Text = api.DoubleValue.ToString();
                    }
                    break;
            }
        }

        void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            sender.RequestClientData(Requests.HostResponse1, SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, typeof(HostResponse1));
            sender.RequestClientData(Requests.HostException1, SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, typeof(HostException1));
            sender.RequestClientData(Requests.HostAPI2, SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, typeof(HostAPI2));
        }

        void sc_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        //
        // Host API 1 uses a request / response style interface
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

            [ClientDataItem(2, ItemSize = 256)]
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

        private void GetDataFromHost(int index)
        {
            HostAPI1 api = new HostAPI1();

            api.RequestorID = guidClient;
            api.RequestedDataIndex = index;

            sc.SetClientData(api);
        }

        private void btnGetVal1_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromHost(1);
        }

        private void btnGetVal2_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromHost(2);
        }

        private void btnGetVal3_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromHost(3);
        }

        private void btnGetVal4_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromHost(4);
        }

        private void btnGetVal5_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromHost(5);
        }
    }
}
