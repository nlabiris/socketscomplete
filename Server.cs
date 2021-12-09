namespace SocketsComplete {
    /// <summary>
    /// Base server to manipulate TCP and UDP servers
    /// </summary>
    public class Server {
        private TCPServer TCP;
        //private UDPServer UDP;

        /// <summary>
        /// Constructor for server
        /// </summary>
        public Server() {
            TCP = new TCPServer();
            //UDP = new UDPServer();
        }

        /// <summary>
        /// Starts all servers
        /// </summary>
        public void Start() {
            TCP.Start();
            //UDP.Start();
        }

        /// <summary>
        /// Checks if any of the servers is running
        /// </summary>
        /// <returns>Any server is running</returns>
        public bool IsActive() {
            return TCP.Active;
        }

        /// <summary>
        /// Stops all servers
        /// </summary>
        public void Stop() {
            TCP.Stop();
            //UDP.Stop();
        }
    }
}
