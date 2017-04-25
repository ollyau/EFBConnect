using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BeatlesBlog.SimConnect;

namespace SimConnectTestWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            sc = new SimConnect(this);

            sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvException += new SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvQuit += new SimConnect.RecvQuitEventHandler(sc_OnRecvQuit);
            sc.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(sc_OnRecvSimobjectData);
            sc.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(sc_OnRecvSimobjectDataBytype);
            sc.OnRecvEventObjectAddremove += new SimConnect.RecvEventObjectAddremoveEventHandler(sc_OnRecvEventObjectAddremove);
        }

        #region Private Data

        private SimConnect sc = null;

        SimConnectConfigure.NetworkingMode LastNetworkMode = SimConnectConfigure.NetworkingMode.IPv4;
        string LastNetworkServer = "localhost";
        string LastNetworkPort = "4504";

        private System.Collections.Generic.Dictionary<UInt32, UserPosition> Objects = new Dictionary<UInt32, UserPosition>();

        #endregion

        void sc_OnRecvEventObjectAddremove(SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data)
        {
            switch ((EventIDs)data.uEventID)
            {
                case EventIDs.AddObject:
                    if (data.dwData != 0 && !Objects.ContainsKey(data.dwData))
                    {
                        UserPosition pos = new UserPosition();
                        sc.RequestDataOnSimObject(
                            (RequestID)((int)RequestID.AIDataBase + data.dwData),
                            data.dwData,
                            SIMCONNECT_PERIOD.VISUAL_FRAME,
                            SIMCONNECT_DATA_REQUEST_FLAG.CHANGED,
                            pos);
                    }
                    break;

                case EventIDs.RemoveObject:
                    if (data.dwData != 0 && Objects.ContainsKey(data.dwData))
                    {
                        lstViewer.Items.Remove(Objects[data.dwData].ListItem);
                        Objects.Remove(data.dwData);
                    }
                    break;
            }
        }

        void sc_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            UserPosition pos = (UserPosition)data.dwData;
            if (!pos.IsUser && !Objects.ContainsKey(data.dwObjectID))
            {
                pos.ListItem.SubItems[colheaderLatitude.Index].Text = pos.LatitudeDegrees.ToString();
                pos.ListItem.SubItems[colheaderLongitude.Index].Text = pos.LongitudeDegrees.ToString();
                pos.ListItem.SubItems[colheaderAltitude.Index].Text = pos.AltitudeFeet.ToString();
                pos.ListItem.SubItems[colheaderPitch.Index].Text = pos.PitchDegrees.ToString();
                pos.ListItem.SubItems[colheaderBank.Index].Text = pos.BankDegrees.ToString();
                pos.ListItem.SubItems[colheaderHeading.Index].Text = pos.HeadingDegrees.ToString();

                Objects.Add(data.dwObjectID, pos);
                lstViewer.Items.Add(pos.ListItem);

                sc.RequestDataOnSimObject(
                    (RequestID)((int)RequestID.AIDataBase + data.dwObjectID),
                    data.dwObjectID,
                    SIMCONNECT_PERIOD.VISUAL_FRAME,
                    SIMCONNECT_DATA_REQUEST_FLAG.CHANGED,
                    pos);
            }
        }

        void sc_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            UserPosition pos = (UserPosition)data.dwData;
            if (!Objects.ContainsKey(data.dwObjectID))
            {
                Objects.Add(data.dwObjectID, pos);
                lstViewer.Items.Add(pos.ListItem);
            }

            pos.ListItem.SubItems[colheaderLatitude.Index].Text = pos.LatitudeDegrees.ToString();
            pos.ListItem.SubItems[colheaderLongitude.Index].Text = pos.LongitudeDegrees.ToString();
            pos.ListItem.SubItems[colheaderAltitude.Index].Text = pos.AltitudeFeet.ToString();
            pos.ListItem.SubItems[colheaderPitch.Index].Text = pos.PitchDegrees.ToString();
            pos.ListItem.SubItems[colheaderBank.Index].Text = pos.BankDegrees.ToString();
            pos.ListItem.SubItems[colheaderHeading.Index].Text = pos.HeadingDegrees.ToString();
        }

        void sc_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            menuDisconnect_Click(null, null);
        }

        void sc_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            lstViewer.Items.Clear();
            UserPosition pos = new UserPosition();
            sc.RequestDataOnUserSimObject(
                RequestID.UserPosition,
                SIMCONNECT_PERIOD.VISUAL_FRAME,
                pos);
            sc.SubscribeToSystemEvent(EventIDs.AddObject, "ObjectAdded");
            sc.SubscribeToSystemEvent(EventIDs.RemoveObject, "ObjectRemoved");
            sc.RequestDataOnSimObjectType(RequestID.AIEnumerate, 200000, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT, typeof(UserPosition));
        }

        private static string strAppName = "SimConnectTestWinForm";

        private void menuConnectLocal_Click(object sender, EventArgs e)
        {
            try
            {
                sc.Open(strAppName);
            }
            catch (SimConnect.SimConnectException)
            {
                MessageBox.Show(this, "Local connection failed", strAppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            menuConnect.Visible = false;
            menuDisconnect.Visible = true;
        }

        private void menuConnectCustom_Click(object sender, EventArgs e)
        {
            SimConnectConfigure scConfig = new SimConnectConfigure();

            scConfig.NetworkMode = LastNetworkMode;
            scConfig.NetworkServer = LastNetworkServer;
            scConfig.NetworkPort = LastNetworkPort;

            if (scConfig.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    if (scConfig.NetworkMode == SimConnectConfigure.NetworkingMode.Pipe)
                    {
                        sc.Open(strAppName, scConfig.NetworkServer, scConfig.NetworkPort);
                    }
                    else if (scConfig.NetworkMode == SimConnectConfigure.NetworkingMode.IPv4)
                    {
                        sc.Open(strAppName, scConfig.NetworkServer, scConfig.NetworkPortInt, false);
                    }
                    else
                    {
                        sc.Open(strAppName, scConfig.NetworkServer, scConfig.NetworkPortInt, true);
                    }
                }
                catch (SimConnect.SimConnectException)
                {
                    MessageBox.Show(this, "Custom connection failed", strAppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LastNetworkMode = scConfig.NetworkMode;
                LastNetworkServer = scConfig.NetworkServer;
                LastNetworkPort = scConfig.NetworkPort;

                menuConnect.Visible = false;
                menuDisconnect.Visible = true;
            }
        }

        private void menuDisconnect_Click(object sender, EventArgs e)
        {
            lstViewer.Items.Clear();
            Objects.Clear();

            sc.Close();

            menuConnect.Visible = true;
            menuDisconnect.Visible = false;
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            lstViewer.Items.Clear();
            Objects.Clear();

            sc.Close();

            sc = null;
        }

        private void menuConnect_DropDownOpening(object sender, EventArgs e)
        {
            menuConnectLocal.Enabled = SimConnect.IsLocalRunning();
        }
    }

    enum RequestID
    {
        UserPosition,
        AIEnumerate,

        AIDataBase = 0x01000000,
    }

    enum EventIDs
    {
        AddObject,
        RemoveObject,
    }

    [DataStruct()]
    public class UserPosition
    {
        public UserPosition()
        {
        }

        #region Private Data

        private bool bIsUser = false;
        private double latitude1 = 10.0;
        private double longitude1 = -105.37;
        private double altitude1 = 250.3;
        private double pitch1 = -12.3;
        private double bank1 = 0.02;
        private double heading1 = 325.78;
        private string category = "category";

        private ListViewItem lvi = new ListViewItem(new string[] {"", "", "", "", "", ""});

        #endregion

        #region Public Accessors and SimConnect Registration

        [DataItem("Is User Sim", "bool")]
        public bool IsUser
        {
            get
            {
                return bIsUser;
            }
            set
            {
                bIsUser = value;
                RaisePropertyChanged("IsUser");
            }
        }

        [DataItem("Plane Latitude", "degrees")]
        public double LatitudeDegrees
        {
            get
            {
                return latitude1;
            }
            set
            {
                latitude1 = value;
                RaisePropertyChanged("LatitudeDegrees");
            }
        }

        [DataItem("Plane Longitude", "degrees")]
        public double LongitudeDegrees
        {
            get
            {
                return longitude1;
            }
            set
            {
                longitude1 = value;
                RaisePropertyChanged("LongitudeDegrees");
            }
        }

        [DataItem("Plane Alt Above Ground", "feet")]
        public double AltitudeFeet
        {
            get
            {
                return altitude1;
            }
            set
            {
                altitude1 = value;
                RaisePropertyChanged("AltitudeFeet");
            }
        }

        [DataItem("Plane Pitch Degrees", "degrees")]
        public double PitchDegrees
        {
            get
            {
                return pitch1;
            }
            set
            {
                pitch1 = value;
                RaisePropertyChanged("PitchDegrees");
            }
        }

        [DataItem("Plane Bank Degrees", "degrees")]
        public double BankDegrees
        {
            get
            {
                return bank1;
            }
            set
            {
                bank1 = value;
                RaisePropertyChanged("BankDegrees");
            }
        }

        [DataItem("Plane Heading Degrees True", "degrees")]
        public double HeadingDegrees
        {
            get
            {
                return heading1;
            }
            set
            {
                heading1 = value;
                RaisePropertyChanged("HeadingDegrees");
            }
        }

        [DataItem("CATEGORY", 64)]
        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                RaisePropertyChanged("Category");
            }
        }

        public ListViewItem ListItem
        {
            get
            {
                return lvi;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members
        // not actually inheriting from INotifyPropertyChanged, just leave here for simplicity

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
