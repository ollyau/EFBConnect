using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EFBConnect
{
    class ForeFlightUdp
    {
        private static readonly Lazy<ForeFlightUdp> _instance = new Lazy<ForeFlightUdp>(() => new ForeFlightUdp());
        public static ForeFlightUdp Instance { get { return _instance.Value; } }

        private Object _lock = new Object();
        private Socket udpSocket;
        private IPEndPoint ipEndPoint;
        private Log log;
        private string simIdent;
        private const int foreFlightPort = 49000;

        private ForeFlightUdp()
        {
            log = Log.Instance;
        }

        public void SetDestination(IPAddress ip)
        {
            lock (_lock)
            {
                ipEndPoint = new IPEndPoint(ip, foreFlightPort);

                if (udpSocket != null)
                {
                    udpSocket.Close();
                    udpSocket.Dispose();
                }

                udpSocket = new Socket(ipEndPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                if (ipEndPoint.Address == IPAddress.Broadcast)
                {
                    udpSocket.EnableBroadcast = true;
                }
            }
        }

        public void SetSimulator(string simIdent)
        {
            this.simIdent = simIdent;
        }

        public void Send(Position p)
        {
            lock (_lock)
            {
                if (udpSocket != null)
                {
                    var posDatagram = string.Format(CultureInfo.InvariantCulture,
                        "XGPS{0},{1:0.#####},{2:0.#####},{3:0.#},{4:0.###},{5:0.#}",
                        simIdent, p.Longitude, p.Latitude, p.Altitude, p.GroundTrack, p.GroundSpeed
                        );
                    udpSocket.SendTo(Encoding.ASCII.GetBytes(posDatagram), ipEndPoint);
                    //log.Info(posDatagram);
                }
            }
        }

        public void Send(Attitude a)
        {
            lock (_lock)
            {
                if (udpSocket != null)
                {
                    var attDatagram = string.Format(CultureInfo.InvariantCulture,
                        "XATT{0},{1:0.#},{2:0.#},{3:0.#}",
                        simIdent, a.TrueHeading, -a.Pitch, -a.Bank
                        );
                    udpSocket.SendTo(Encoding.ASCII.GetBytes(attDatagram), ipEndPoint);
                    //log.Info(attDatagram);
                }
            }
        }

        public void Send(TrafficInfo t, uint dwObjectID)
        {
            lock (_lock)
            {
                if (udpSocket != null)
                {
                    var trafficDatagram = string.Format(CultureInfo.InvariantCulture,
                        "XTRAFFIC{0},{1},{2:0.#####},{3:0.#####},{4:0.#},{5:0.#},{6},{7:0.###},{8:0.#},{9}",
                        simIdent, dwObjectID, t.Latitude, t.Longitude, t.Altitude, t.VerticalSpeed,
                        t.OnGround ? 0 : 1, t.TrueHeading, t.GroundVelocity,
                        (string.IsNullOrEmpty(t.Airline) || string.IsNullOrEmpty(t.FlightNumber)) ? t.TailNumber : t.Airline + " " + t.FlightNumber
                        );
                    udpSocket.SendTo(Encoding.ASCII.GetBytes(trafficDatagram), ipEndPoint);
                    //log.Info(trafficDatagram);
                }
            }
        }
    }
}
