namespace SocketsComplete {
    /// <summary>
    /// Base server to manipulate TCP and UDP servers
    /// </summary>
    public class Server {
        private TCPServer TCP;
        private UDPServer UDP;

        /// <summary>
        /// Constructor for server
        /// </summary>
        public Server() {
            UDP = new UDPServer();
            TCP = new TCPServer();
        }

        /// <summary>
        /// Starts all servers
        /// </summary>
        public void Start() {
            UDP.Start();
            TCP.Start();
        }

        /// <summary>
        /// Checks if any of the servers is running
        /// </summary>
        /// <returns>Any server is running</returns>
        public bool IsActive() {
            return UDP.Active || TCP.Active;
        }

        /// <summary>
        /// Stops all servers
        /// </summary>
        public void Stop() {
            UDP.Stop();
            TCP.Stop();
        }
    }
}
