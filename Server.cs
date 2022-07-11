namespace SocketsComplete
{
    /// <summary>
    /// Base server to manipulate TCP and UDP servers
    /// </summary>
    public class Server
    {
        private TCPServer TCP;
        //private UDPServer UDP;
        private TCPOutputControl TCPOutput;

        /// <summary>
        /// Constructor for server
        /// </summary>
        public Server()
        {
            var OutputControlData = new OutputControlData();
            TCP = new TCPServer(OutputControlData);
            TCPOutput = new TCPOutputControl(OutputControlData);
            //UDP = new UDPServer();
        }

        /// <summary>
        /// Starts all servers
        /// </summary>
        public void Start()
        {
            TCP.Start();
            TCPOutput.Start();
            //UDP.Start();
        }

        /// <summary>
        /// Checks if any of the servers is running
        /// </summary>
        /// <returns>Any server is running</returns>
        public bool IsActive()
        {
            return TCP.Active;
        }

        /// <summary>
        /// Stops all servers
        /// </summary>
        public void Stop()
        {
            TCP.Stop();
            TCPOutput.Stop();
            //UDP.Stop();
        }
    }
}
