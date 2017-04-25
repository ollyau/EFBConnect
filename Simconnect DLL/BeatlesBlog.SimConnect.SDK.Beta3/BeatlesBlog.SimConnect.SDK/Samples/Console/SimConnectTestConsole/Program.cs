using System;
using System.Text;

namespace SimConnectTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SimConnectTest sct = new SimConnectTest();
            sct.Test();
            sct = null;
        }
    }

    // my request IDs
    enum Requests
    {
        DisplayWeather,
        DisplayEvent,
        DisplaySetData,

        DisplayMenuMain,
        DisplayMenuWeather,
        DisplayMenuEvent,
        DisplayMenuSetData,

        CreateAI,

        WeatherAtLocation,
        WeatherNearestLocation,
        WeatherObservation,
        WeatherObservationStation,
        WeatherObservationNearestStation,
    }

    // my event IDs
    enum Events
    {
        AilerCenter,
        AilerRight,
        AilerLeft,
    }

    // data structures used to send/receive SimObject data

    [BeatlesBlog.SimConnect.DataStruct()]
    public struct VehiclePosition
    {
        [BeatlesBlog.SimConnect.DataItem("Plane Latitude", "degrees")]
        public float latitude;

        [BeatlesBlog.SimConnect.DataItem("Plane Longitude", "degrees")]
        public float longitude;

        [BeatlesBlog.SimConnect.DataItem("Plane Altitude", "feet")]
        public float altitude;
    }

    [BeatlesBlog.SimConnect.DataStruct()]
    public struct VehicleOrientation
    {
        [BeatlesBlog.SimConnect.DataItem("Plane Pitch Degrees", "degrees")]
        public float pitch;

        [BeatlesBlog.SimConnect.DataItem("Plane Bank Degrees", "degrees")]
        public float bank;

        [BeatlesBlog.SimConnect.DataItem("Plane Heading Degrees Magnetic", "degrees")]
        public float heading;
    }

    [BeatlesBlog.SimConnect.DataStruct()]
    public struct VehicleData
    {
        [BeatlesBlog.SimConnect.DataItem()]
        public VehiclePosition position;

        [BeatlesBlog.SimConnect.DataItem()]
        public VehicleOrientation orientation;
    }

    class SimConnectTest
    {
        BeatlesBlog.SimConnect.SimConnect sc = null;

        System.Threading.AutoResetEvent exitEvent = new System.Threading.AutoResetEvent(false);

        // main selection menu
        string[] strMainMenu = {
                                   "SimConnect Test",                           // window title
                                   "Choose an item:",                           // selection list header
                                   "Test Weather Functions",                    // selection list item 1
                                   "Test Event Functions",                      // selection list item 2
                                   "Test Set Data Functions",                   // selection list item 3
                                   "Exit AddOn"                                 // selection list item 4
                               };

        // weather selection menu
        string[] strWeatherMenu = {   
                                      "SimConnect Test - Weather Functions",    // window title
                                      "Choose an item:",                        // selection list header
                                      "Weather at Current LLA",                 // selection list item 1
                                      "Weather at KSEA",                        // selection list item 2
                                      "Weather at Nearest Station",             // selection list item 3
                                      "Return"                                  // selection list item 4
                                  };

        // event selection menu
        string[] strEventMenu = {
                                    "SimConnect Test - Event Functions",        // window title
                                    "Choose an item:",                          // selection list header
                                    "Send Aileron Right",                       // selection list item 1
                                    "Send Aileron Left",                        // selection list item 2
                                    "Send Aileron Center",                      // selection list item 3
                                    "Return"                                    // selection list item 4
                                };

        // set data selection menu
        string[] strSetDataMenu = {
                                      "SimConnect Test - Set Data Functions",                       // window title
                                      "Choose an item:",                                            // selection list header
                                      "Set User Vehicle to KEZF using INITPOSITION structure",      // selection list item 1
                                      "Set User Vehicle to 0,0,3000 using custom structure",        // selection list item 2
                                      "Create AI Aircraft at KEZF",                                 // selection list item 3
                                      "Return"                                                      // selection list item 4
                                  };

        public SimConnectTest()
        {
            sc = new BeatlesBlog.SimConnect.SimConnect(null);
        }

        ~SimConnectTest()
        {
            sc = null;
        }

        public void Test()
        {
            // hook needed events
            sc.OnRecvOpen += new BeatlesBlog.SimConnect.SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);
            sc.OnRecvException += new BeatlesBlog.SimConnect.SimConnect.RecvExceptionEventHandler(sc_OnRecvException);
            sc.OnRecvEvent += new BeatlesBlog.SimConnect.SimConnect.RecvEventEventHandler(sc_OnRecvEvent);
            sc.OnRecvWeatherObservation += new BeatlesBlog.SimConnect.SimConnect.RecvWeatherObservationEventHandler(sc_OnRecvWeatherObservation);
            sc.OnRecvSimobjectData += new BeatlesBlog.SimConnect.SimConnect.RecvSimobjectDataEventHandler(sc_OnRecvSimobjectData);
            sc.OnRecvQuit += new BeatlesBlog.SimConnect.SimConnect.RecvQuitEventHandler(sc_OnRecvQuit);
            sc.OnRecvAssignedObjectId += new BeatlesBlog.SimConnect.SimConnect.RecvAssignedObjectIdEventHandler(sc_OnRecvAssignedObjectId);

            // only uncomment one of the sc.Open lines below

            // comment this if and all the local versions of the sc.Open if using a test remote connection
            if (BeatlesBlog.SimConnect.SimConnect.IsLocalRunning())
            {   // allow attempt a local connection if one appears to be running

                // make local pipe connection (default local mode)
                sc.Open("SimConnectTest");

                // make local IPv4 connection (any of the below are valid)
                //sc.Open("SimConnectTest", null, 0, System.Net.Sockets.AddressFamily.InterNetwork);
                //sc.Open("SimConnectTest", "", 0, System.Net.Sockets.AddressFamily.InterNetwork);
                //sc.Open("SimConnectTest", "localhost", 0, System.Net.Sockets.AddressFamily.InterNetwork);
                //sc.Open("SimConnectTest", "127.0.0.1", 0, System.Net.Sockets.AddressFamily.InterNetwork);

                // make local IPv6 connection (any of the below are valid)
                //sc.Open("SimConnectTest", null, 0, System.Net.Sockets.AddressFamily.InterNetworkV6);
                //sc.Open("SimConnectTest", "", 0, System.Net.Sockets.AddressFamily.InterNetworkV6);
                //sc.Open("SimConnectTest", "localhost", 0, System.Net.Sockets.AddressFamily.InterNetworkV6);
                //sc.Open("SimConnectTest", "::0", 0, System.Net.Sockets.AddressFamily.InterNetworkV6);
            }
            else
            {
                // make remote IPv4 connection
                //sc.Open("SimConnectTest", "winxpsp3dt", 4504, System.Net.Sockets.AddressFamily.InterNetwork);

                // make remote IPv6 connection
                //sc.Open("SimConnectTest", "vista64vm", 4506, System.Net.Sockets.AddressFamily.InterNetworkV6);

                // comment these 3 lines out if using a test remote connection
                Console.WriteLine("No Local SimConnect Instance available.  Press return to exit.");
                Console.ReadLine();
                return;
            }

            // cause mainline of program to wait until exit option is chosen from in-game menu, or game exits
            exitEvent.WaitOne();
        }

        void sc_OnRecvAssignedObjectId(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data)
        {
            System.Collections.Generic.List<BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT> waypoints = new System.Collections.Generic.List<BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT>(9);

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.267192578,
                -77.447785951,
                0,
                BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.ALTITUDE_IS_AGL | BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.ON_GROUND | BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.SPEED_REQUESTED,
                40));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.263456641,
                -77.452560283,
                100,
                BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.ALTITUDE_IS_AGL | BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.SPEED_REQUESTED,
                80));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.22580833,
                -77.371365646,
                1000,
                BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.ALTITUDE_IS_AGL | BeatlesBlog.SimConnect.SIMCONNECT_WAYPOINT_FLAGS.SPEED_REQUESTED,
                135));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.259513879,
                -77.208287338,
                2000));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.225231046,
                -76.863052369,
                4000));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                38.131311768,
                -76.5526886,
                6000));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                37.995076241,
                -76.301376344,
                10000));

            waypoints.Add(new BeatlesBlog.SimConnect.SIMCONNECT_DATA_WAYPOINT(
                37.622929368,
                -76.007492066,
                10000));

            sc.SetDataOnSimObject(data.dwObjectID, waypoints);
        }

        void sc_OnRecvQuit(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV data)
        {
            // cause mainline thread to exit
            exitEvent.Set();
        }

        void sc_OnRecvSimobjectData(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            Console.WriteLine("OnRecvSimobjectData");
            switch ((Requests)data.dwRequestID)
            {
                case Requests.WeatherAtLocation:
                    // SimObjectData related to Weather At User's Location, request interpolated observation
                    {
                        VehiclePosition ap = (VehiclePosition)data.dwData;
                        sc.WeatherRequestInterpolatedObservation(Requests.WeatherObservation, ap.latitude, ap.longitude, ap.altitude);
                    }
                    break;

                case Requests.WeatherNearestLocation:
                    // SimObjectData related to Weather Station Nearest User's Location, request observation at nearest station
                    {
                        VehiclePosition ap = (VehiclePosition)data.dwData;
                        sc.WeatherRequestObservationAtNearestStation(Requests.WeatherObservationNearestStation, ap.latitude, ap.longitude);
                    }
                    break;
            }
        }

        void sc_OnRecvWeatherObservation(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_WEATHER_OBSERVATION data)
        {
            // display the returned weather observation
            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.SCROLL_RED, 20.0f, Requests.DisplayWeather, data.szMetar);
            Console.WriteLine("OnRecvWeatherObservation");
        }

        void sc_OnRecvEvent(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_EVENT data)
        {
            switch ((Requests)data.uEventID)
            {
                case Requests.DisplayMenuMain:
                    // event from main menu, see which menu item was selected
                    switch ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData)
                    {
                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_1:
                            // display weather selection menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuWeather, strWeatherMenu);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_2:
                            // display event selection menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuEvent, strEventMenu);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_3:
                            // display set data selection menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuSetData, strSetDataMenu);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_4:
                            // cause mainline thread to exit
                            exitEvent.Set();
                            break;

                    }
                    break;

                case Requests.DisplayMenuWeather:
                    // event from weather selection menu, see which menu item was selected
                    switch ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData)
                    {
                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_1:
                            // requests the users current location to use in weather request (actual weather request made in OnRecvSimObjectData)
                            sc.RequestDataOnUserSimObject(Requests.WeatherAtLocation, typeof(VehiclePosition));
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_2:
                            // request weather at SeaTac airport
                            sc.WeatherRequestObservationAtStation(Requests.WeatherObservationStation, "KSEA");
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_3:
                            // request the users current location to use in the weather request (actual weather request made in OnRecvSimObjectData)
                            sc.RequestDataOnUserSimObject(Requests.WeatherNearestLocation, typeof(VehiclePosition));
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_4:
                            // display main menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuMain, strMainMenu);
                            break;

                    }
                    break;

                case Requests.DisplayMenuEvent:
                    // event from event selection menu, see which menu item was selected
                    switch((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData)
                    {
                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_1:
                            // Send the Aileron Right event and show status message
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplayEvent, "Sent Aileron Right");
                            sc.TransmitClientEventToUser(Events.AilerRight, BeatlesBlog.SimConnect.SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_2:
                            // Send the Aileron Left event and show status message
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplayEvent, "Sent Aileron Left");
                            sc.TransmitClientEventToUser(Events.AilerLeft, BeatlesBlog.SimConnect.SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_3:
                            // Send the Aileron Center event and show status message
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplayEvent, "Sent Aileron Center");
                            sc.TransmitClientEventToUser(Events.AilerCenter, BeatlesBlog.SimConnect.SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_4:
                            // display main menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuMain, strMainMenu);
                            break;
                    }
                    break;

                case Requests.DisplayMenuSetData:
                    // event from set data menu, see which menu item was selected
                    switch ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData)
                    {
                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_1:
                            // Set the user's current location to (0,0,3000) using the SIMCONNECT_DATA_INITPOSIITON structure
                            sc.SetDataOnUserSimObject(
                                new BeatlesBlog.SimConnect.SIMCONNECT_DATA_INITPOSITION(
                                    38.268351,
                                    -77.445360,
                                    0,
                                    0, 
                                    0, 
                                    225, 
                                    true, 
                                    0
                                )
                            );
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplaySetData, "SetDataOnSimObject called with INITPOSITION structure");
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_2:
                            // set the user's current location to (0,0,3000) using a program defined structure
                            {
                                VehicleData vehdata = new VehicleData();
                                vehdata.position.latitude = 0;
                                vehdata.position.longitude = 0;
                                vehdata.position.altitude = 3000;
                                vehdata.orientation.pitch = 0;
                                vehdata.orientation.bank = 0;
                                vehdata.orientation.heading = 120;

                                sc.SetDataOnUserSimObject(vehdata);
                                sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplaySetData, "SetDataOnSimObject called with custom structure");
                            }
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_3:
                            // create an AI aircraft and place the user near it
                            sc.AICreateNonATCAircraft("Beech Baron 58 Paint1", "N09TG",
                                new BeatlesBlog.SimConnect.SIMCONNECT_DATA_INITPOSITION(
                                    38.268351,
                                    -77.445360,
                                    0,
                                    0,
                                    0,
                                    225,
                                    true,
                                    0
                                ), 
                                Requests.CreateAI
                            );
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.PRINT_RED, 3.0f, Requests.DisplaySetData, "AI Aircraft Created");
                            break;

                        case BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.MENU_SELECT_4:
                            // display main menu
                            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuMain, strMainMenu);
                            break;
                    }
                    break;

                case Requests.DisplayWeather:
                    // event from display of current weather observation, if timed out, redisplay weather selection menu
                    if ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData == BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.TIMEOUT)
                    {
                        sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuWeather, strWeatherMenu);
                    }
                    break;

                case Requests.DisplayEvent:
                    // event from display of event status message, if timed out, redisplay event selection menu
                    if ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData == BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.TIMEOUT)
                    {
                        sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuEvent, strEventMenu);
                    }
                    break;

                case Requests.DisplaySetData:
                    // event from display of set data status message, if timed out, redisplay set data selection menu
                    if ((BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT)data.dwData == BeatlesBlog.SimConnect.SIMCONNECT_TEXT_RESULT.TIMEOUT)
                    {
                        sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuSetData, strSetDataMenu);
                    }
                    break;
            }

            Console.WriteLine("OnRecvEvent");
        }

        void sc_OnRecvException(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("OnRecvException: " + data.dwException.ToString() + "  " + data.dwSendID.ToString() + "  " + data.dwIndex.ToString());
            switch ((BeatlesBlog.SimConnect.SIMCONNECT_EXCEPTION)data.dwException)
            {
                case BeatlesBlog.SimConnect.SIMCONNECT_EXCEPTION.WEATHER_UNABLE_TO_GET_OBSERVATION:
                    sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.SCROLL_RED, 20.0f, Requests.DisplayWeather, "Unable to obtain a weather observation");
                    break;
            }
        }

        void sc_OnRecvOpen(BeatlesBlog.SimConnect.SimConnect sender, BeatlesBlog.SimConnect.SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("OnRecvOpen");

            // map some SimConnect events for event testing
            sender.MapClientEventToSimEvent(Events.AilerCenter, "CENTER_AILER_RUDDER");
            sender.MapClientEventToSimEvent(Events.AilerRight, "AILERONS_RIGHT");
            sender.MapClientEventToSimEvent(Events.AilerLeft, "AILERONS_LEFT");

            // display the main selection menu
            sc.Text(BeatlesBlog.SimConnect.SIMCONNECT_TEXT_TYPE.MENU, 0, Requests.DisplayMenuMain, strMainMenu);
        }
    }
}
