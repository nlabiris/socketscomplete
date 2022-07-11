using SocketsComplete.Properties;
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
            var state = TCP.CreateSocket(Settings.Default.ServerIP, Settings.Default.TCPPort);
            TCP.Connect(state.endPoint, state.socket);
            return state.socket;
        }

        public Socket OutputConnect()
        {
            var state = TCP.CreateSocket(Settings.Default.ServerIP, Settings.Default.OutputControlPort);
            TCP.Connect(state.endPoint, state.socket);
            return state.socket;
        }

        public void Transmit(Socket client, byte[] packet)
        {
            TCP.Transmit(client, packet);
        }

        public void Receive(Socket client)
        {
            TCP.Retrieve(client);
        }

        public void Close(Socket client)
        {
            TCP.Close(client);
        }
    }
}
