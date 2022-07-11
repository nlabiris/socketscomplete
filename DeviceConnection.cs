using System.Net;
using System.Net.Sockets;

namespace SocketsComplete
{
    public class DeviceConnection
    {
        /// <summary>
        /// Constructor for device connection
        /// </summary>
        /// <param name="socket">TCP socket which the device is using</param>
        public DeviceConnection(Socket socket)
        {
            TCPSocket = socket;
        }

        /// <summary>
        /// Constructor for device connection
        /// </summary>
        /// <param name="ep">UDP endpoint which the device is using</param>
        public DeviceConnection(EndPoint ep)
        {
            UDPEndPoint = ep;
        }

        /// <summary>
        /// Empty constructor for device connection
        /// </summary>
        public DeviceConnection()
        {
        }

        /// <summary>
        /// Stored TCP connection to the device
        /// </summary>
        public Socket TCPSocket { get; private set; }

        /// <summary>
        /// Stored UDP connection to the device
        /// </summary>
        public EndPoint UDPEndPoint { get; private set; }

        /// <summary>
        /// Checks if the device has a TCP connection
        /// </summary>
        /// <returns>Does the device have a TCP connection</returns>
        public bool HasTCP
        {
            get { return TCPSocket != null; }
        }

        /// <summary>
        /// Sets a TCP socket to the device connection
        /// </summary>
        /// <param name="socket">TCP socket which the device is using</param>
        public void Set(Socket socket)
        {
            TCPSocket = socket;
            if (socket != null)
                UDPEndPoint = null;
        }

        /// <summary>
        /// Sets a UDP endpoint to the device connection
        /// </summary>
        /// <param name="ep">UDP endpoint which the device is using</param>
        public void Set(EndPoint ep)
        {
            UDPEndPoint = ep;
            if (ep != null)
                TCPSocket = null;
        }
    }
}
