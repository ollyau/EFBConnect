using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BeatlesBlog.SimConnect;

namespace SimConnectTestSilverLight
{
    public partial class Page : UserControl
    {
        public delegate void MyDelegate();

        enum RequestIDs
        {
            AIEnumerate,

            AIDataBase = 0x01000000,
        }

        enum EventIDs
        {
            AddObject,
            RemoveObject,
        }

        public Page()
        {
            InitializeComponent();

            lstViewer.Items.Clear();
            lstViewer.ItemsSource = Objects;
        }

        #region Private Data

        private class ObjectList : ObservableKeyedCollection<UInt32, VehiclePosition>
        {
            protected override UInt32 GetKeyForItem(VehiclePosition item)
            {
                if (item != null)
                    return item.ObjectID;
                else
                    return 0;
            }
        }
        ObjectList Objects = new ObjectList();

        private SimConnect sc = null;

        #endregion

        private void SimConnectConnect(object sender, RoutedEventArgs e)
        {
            string strAppName = "SimConnectTestSilverLight";

            sc = new SimConnect(this.Dispatcher);

            sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvException += new SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvQuit += new SimConnect.RecvQuitEventHandler(sc_OnRecvQuit);
            sc.OnRecvEventObjectAddremove += new SimConnect.RecvEventObjectAddremoveEventHandler(sc_OnRecvEventObjectAddremove);
            sc.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(sc_OnRecvSimobjectDataBytype);

            sc.Open(strAppName, "localhost", 4504);

            Connect.Visibility = Visibility.Collapsed;
            Disconnect.Visibility = Visibility.Visible;
        }

        private void SimConnectDisconnect(object sender, RoutedEventArgs e)
        {
            sc.Close();
            sc = null;

            Disconnect.Visibility = Visibility.Collapsed;
            Connect.Visibility = Visibility.Visible;

            Objects.Clear();
        }

        #region SimConnect OnRecvXxx handlers

        private void sc_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        private void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            sender.SubscribeToSystemEvent(EventIDs.AddObject, "ObjectAdded");
            sender.SubscribeToSystemEvent(EventIDs.RemoveObject, "ObjectRemoved");

            sender.RequestDataOnSimObjectType(RequestIDs.AIEnumerate, 200000, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT, typeof(VehiclePosition));
        }

        private void sc_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            SimConnectDisconnect(null, null);
        }

        private void sc_OnRecvEventObjectAddremove(SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data)
        {
            switch ((EventIDs)data.uEventID)
            {
                case EventIDs.AddObject:
                    if (data.dwData != 0 && !Objects.Contains(data.dwData))
                    {
                        VehiclePosition pos = new VehiclePosition(data.dwData);
                        Objects.Add(pos);
                        sender.RequestDataOnSimObject(
                            (RequestIDs)((int)RequestIDs.AIDataBase + (int)pos.ObjectID),
                            pos.ObjectID,
                            SIMCONNECT_PERIOD.VISUAL_FRAME,
                            SIMCONNECT_DATA_REQUEST_FLAG.CHANGED,
                            pos);
                    }
                    break;

                case EventIDs.RemoveObject:
                    if (data.dwData != 0 && Objects.Contains(data.dwData))
                    {
                        Objects.Remove(data.dwData);
                    }
                    break;
            }
        }

        private void sc_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (!Objects.Contains(data.dwObjectID))
            {
                VehiclePosition pos = (VehiclePosition)data.dwData;
                pos.ObjectID = data.dwObjectID;
                Objects.Add(pos);
                sender.RequestDataOnSimObject(
                    (RequestIDs)((int)RequestIDs.AIDataBase + (int)pos.ObjectID),
                    pos.ObjectID,
                    SIMCONNECT_PERIOD.VISUAL_FRAME,
                    SIMCONNECT_DATA_REQUEST_FLAG.CHANGED,
                    pos);
            }
        }

        #endregion
    }

    [DataStruct()]
    public class VehiclePosition : System.ComponentModel.INotifyPropertyChanged
    {
        public VehiclePosition(UInt32 eObjectID)
        {
            this.eObjectID = eObjectID;
        }

        public VehiclePosition()
        {
        }

        #region Private Data

        private UInt32 eObjectID;

        private bool bIsUser = false;
        private double latitude1 = 10.0;
        private double latitude2 = 10.0;
        private double longitude1 = -105.37;
        private double longitude2 = -105.37;
        private double altitude1 = 250.3;
        private double altitude2 = 250.3;
        private double pitch1 = -12.3;
        private double pitch2 = -12.3;
        private double bank1 = 0.02;
        private double bank2 = 0.02;
        private double heading1 = 325.78;
        private double heading2 = 325.78;
        private string category = "category";

        #endregion

        #region Public Accessors and SimConnect Registration

        public UInt32 ObjectID
        {
            get
            {
                return eObjectID;
            }
            set
            {
                eObjectID = value;
            }
        }

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

        [DataItem("Plane Latitude", "radians")]
        public double LatitudeRadians
        {
            get
            {
                return latitude2;
            }
            set
            {
                latitude2 = value;
                RaisePropertyChanged("LatitudeRadians");
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

        [DataItem("Plane Longitude", "radians")]
        public double LongitudeRadians
        {
            get
            {
                return longitude2;
            }
            set
            {
                longitude2 = value;
                RaisePropertyChanged("LongitudeRadians");
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

        [DataItem("Plane Alt Above Ground", "meters")]
        public double AltitudeMeters
        {
            get
            {
                return altitude2;
            }
            set
            {
                altitude2 = value;
                RaisePropertyChanged("AltitudeMeters");
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

        [DataItem("Plane Pitch Degrees", "radians")]
        public double PitchRadians
        {
            get
            {
                return pitch2;
            }
            set
            {
                pitch2 = value;
                RaisePropertyChanged("PitchRadians");
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

        [DataItem("Plane Bank Degrees", "radians")]
        public double BankRadians
        {
            get
            {
                return bank2;
            }
            set
            {
                bank2 = value;
                RaisePropertyChanged("BankRadians");
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

        [DataItem("Plane Heading Degrees True", "radians")]
        public double HeadingRadians
        {
            get
            {
                return heading2;
            }
            set
            {
                heading2 = value;
                RaisePropertyChanged("HeadingRadians");
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

        #endregion

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

    public abstract class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        #region Override Clear/Insert/Remove/Set commands to add a call to RaiseCollectionChanged to fire the CollectionChanged event

        protected override void ClearItems()
        {
            base.ClearItems();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            object item = this.ElementAt(index);
            base.RemoveItem(index);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        #endregion

        #region INotifyCollectionChanged Members - implements CollectionChanged event and RaiseCollectionChanged function

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        #endregion
    }
}
