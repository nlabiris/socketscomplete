namespace SocketsComplete {
    public class Client {
        private TCPClient TCP;

        public Client(string serverIp) {
            TCP = new TCPClient(serverIp);
        }

        public void Transmit(byte[] packet) {
            TCP.Transmit(packet);
        }
    }
}
