using BeatlesBlog.SimConnect;

namespace EFBConnect
{
    [DataStruct()]
    public struct Position
    {
        [DataItem("PLANE LATITUDE", "degrees")]
        public double Latitude;

        [DataItem("PLANE LONGITUDE", "degrees")]
        public double Longitude;

        [DataItem("PLANE ALTITUDE", "meters")]
        public double Altitude;

        [DataItem("GPS GROUND TRUE TRACK", "degrees")]
        public double GroundTrack;

        [DataItem("GPS GROUND SPEED", "Meters per second")]
        public double GroundSpeed;
    }

    [DataStruct()]
    public struct TrafficInfo
    {
        [DataItem("PLANE LATITUDE", "degrees")]
        public double Latitude;

        [DataItem("PLANE LONGITUDE", "degrees")]
        public double Longitude;

        [DataItem("PLANE ALTITUDE", "feet")]
        public double Altitude;

        [DataItem("VELOCITY WORLD Y", "ft/min")]
        public double VerticalSpeed;

        [DataItem("SIM ON GROUND", "bool")]
        public bool OnGround;

        [DataItem("PLANE HEADING DEGREES TRUE", "degrees")]
        public double TrueHeading;

        [DataItem("GROUND VELOCITY", "Knots")]
        public double GroundVelocity;

        [DataItem("ATC ID", 32)]
        public string TailNumber;

        [DataItem("ATC AIRLINE", 64)]
        public string Airline;

        [DataItem("ATC FLIGHT NUMBER", 8)]
        public string FlightNumber;

        [DataItem("IS USER SIM", "bool")]
        public bool UserSim;
    }

    [DataStruct()]
    public struct Attitude
    {
        [DataItem("PLANE PITCH DEGREES", "degrees")]
        public double Pitch;

        [DataItem("PLANE BANK DEGREES", "degrees")]
        public double Bank;

        [DataItem("PLANE HEADING DEGREES TRUE", "degrees")]
        public double TrueHeading;
    }

    enum Requests
    {
        DisplayText,
        UserPosition,
        UserAttitude,
        TrafficEnumerate,
        TrafficInfoBase = 0x01000000,
    }

    enum Events
    {
        ObjectAdded,
        SixHz
    }
}
