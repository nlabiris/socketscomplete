using System.Net.Sockets;

namespace SocketsComplete {
    public class StateObject {
        /// <summary>
        /// Client socket
        /// </summary>
        public Socket workSocket = null;

        /// <summary>
        /// Size of receive buffer
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// Receive buffer
        /// </summary>
        public byte[] buffer = new byte[BufferSize];

        /// <summary>
        /// Bytes recieved
        /// </summary>
        public int BytesReceived = 0;
    }
}
