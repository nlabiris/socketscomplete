using System.Net.Sockets;

namespace SocketsComplete {
    public class Client
    {
        private TCPClient TCP;

        public Client()
        {
            TCP = new TCPClient();
        }

        public Socket Connect()
        {
            var state = TCP.CreateSocket();
            TCP.Connect(state.endPoint, state.socket);
            return state.socket;
        }

        public void Transmit(Socket client, byte[] packet)
        {
            TCP.Transmit(client, packet);
        }

        public void Close(Socket client)
        {
            TCP.Close(client);
        }
    }
}
