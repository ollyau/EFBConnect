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
    public partial class Page : UserControl
    {
        BeatlesBlog.SimConnect.SimConnect sc = null;

        System.Collections.ObjectModel.ObservableCollection<SampleData> Samples = new System.Collections.ObjectModel.ObservableCollection<SampleData>();
        Microsoft.VirtualEarth.MapControl.MapPolyline polyline = new Microsoft.VirtualEarth.MapControl.MapPolyline();

        enum Requests
        {
            UserVehicle,
        }
        
        public Page()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            polyline.Stroke = new SolidColorBrush(Colors.Green);
            polyline.StrokeThickness = 3.0;
            polyline.Opacity = 0.8;
            polyline.Locations = new Microsoft.VirtualEarth.MapControl.LocationCollection();
        }

        private void scConfig_ConnectClicked(object sender, EventArgs e)
        {
            scConfig.Visibility = Visibility.Collapsed;

            sc = new BeatlesBlog.SimConnect.SimConnect(this.Dispatcher);

            sc.OnRecvException += new BeatlesBlog.SimConnect.SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvOpen += new BeatlesBlog.SimConnect.SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvSimobjectData += new BeatlesBlog.SimConnect.SimConnect.RecvSimobjectDataEventHandler(sc_OnRecvSimobjectData);

            sc.Open("MovingMapSL", scConfig.ServerName, scConfig.ServerPortInt);
        }

        void sc_OnRecvException(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        void sc_OnRecvOpen(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_OPEN data)
        {
            sc.RequestDataOnUserSimObject(Requests.UserVehicle, BeatlesBlog.SimConnect.SIMCONNECT_PERIOD.SECOND, BeatlesBlog.SimConnect.SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, typeof(SampleData));

            MainUI.Visibility = Visibility.Visible;
            lstSamples.ItemsSource = Samples;
            mapMain.Children.Add(polyline);
        }

        void sc_OnRecvSimobjectData(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            SampleData sample = (SampleData)data.dwData;
            Samples.Insert(0, sample);  // store this sample in the main collection

            if (polyline.Locations.Count == 0)
            {   // stick the first point in twice, so we can start updating the zeroeth position right now
                polyline.Locations.Add(sample.Location);
                polyline.Locations.Add(sample.Location);
            }

            if ((Samples.Count % 6) == 0)
            {   // create a new polyline point every 6 samples
                polyline.Locations.Insert(0, sample.Location);
            }

            polyline.Locations[0] = sample.Location;    // update zeroeth sample in polyline to current location
            mapMain.Center = sample.Location;           // update map cener to current location
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Samples.Clear();
            polyline.Locations.Clear();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            sc.Close();

            sc = null;

            Samples.Clear();
            polyline.Locations.Clear();

            mapMain.Children.Remove(polyline);
            lstSamples.ItemsSource = null;
            MainUI.Visibility = Visibility.Collapsed;
            scConfig.Visibility = Visibility.Visible;
        }

        private void btnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Host.Content.IsFullScreen = !App.Current.Host.Content.IsFullScreen;

            if (App.Current.Host.Content.IsFullScreen)
            {
                btnFullscreen.Content = "Windowed";
            }
            else
            {
                btnFullscreen.Content = "Fullscreen";
            }
        }
    }

    [BeatlesBlog.SimConnect.DataStruct()]
    public class SampleData
    {
        private Microsoft.VirtualEarth.MapControl.Location loc = new Microsoft.VirtualEarth.MapControl.Location();
        public Microsoft.VirtualEarth.MapControl.Location Location
        {
            get
            {
                return loc;
            }
        }

        [BeatlesBlog.SimConnect.DataItem("Plane Latitude", "degrees")]
        public double Latitude
        {
            get
            {
                return loc.Latitude;
            }
            set
            {
                loc.Latitude = value;
            }
        }

        [BeatlesBlog.SimConnect.DataItem("Plane Longitude", "degrees")]
        public double Longitude
        {
            get
            {
                return loc.Longitude;
            }
            set
            {
                loc.Longitude = value;
            }
        }

        [BeatlesBlog.SimConnect.DataItem("Plane Altitude", "meters")]
        public double Altitude
        {
            get
            {
                return loc.Altitude;
            }
            set
            {
                loc.Altitude = value;
            }
        }
    }

    public class Trunc6 : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value).ToString("F6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
