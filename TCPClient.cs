using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketsComplete
{
    internal class TCPClient
    {
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private Logger Logger = new Logger("Client");

        public void Transmit(Socket client, byte[] packet)
        {
            try
            {
                Send(client, packet);
            }
            catch (Exception e)
            {
                Logger.Console(e);
            }
        }

        public void Retrieve(Socket client)
        {
            try
            {
                Receive(client);
            }
            catch (Exception e)
            {
                Logger.Console(e);
            }
        }

        public StateObject CreateSocket(string ip, int port)
        {
            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            return new StateObject
            {
                socket = client,
                endPoint = remoteEP
            };
        }

        public void Connect(EndPoint remoteEP, Socket client)
        {
            try
            {
                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                Logger.Console(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Logger.Console("Connecting...");

                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Logger.Console($"Socket connected to {client.RemoteEndPoint.ToString()}");

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Logger.Console(e.ToString());
            }
        }

        public void Close(Socket client)
        {
            // Close socket
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            Logger.Console("Socket closed");

        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.socket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                receiveDone.WaitOne();
            }
            catch (Exception e)
            {
                Logger.Console(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Logger.Console("Receiving...");
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.socket;
                byte[] localBuffer = null;
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Logger.Console("Message received...");
                    state.BytesReceived = bytesRead;
                    localBuffer = new byte[bytesRead];
                    Array.Copy(state.buffer, localBuffer, state.BytesReceived);
                    Logger.Console($"Read {state.BytesReceived} bytes from socket. \n Data : [{BitConverter.ToString(localBuffer):X}]");
                    //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    receiveDone.Set();
                }
                else
                {
                    // it enters here if we shutdown the socket
                    Logger.Console("Message done...");
                    Logger.Console($"Read {state.BytesReceived} bytes from socket. \n Data : [{BitConverter.ToString(state.buffer):X}]");
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Logger.Console(e.ToString());
            }
        }

        private void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.
            client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
            sendDone.WaitOne();
        }

        private void SendCallback(IAsyncResult ar)
        {
            Logger.Console("Sending...");
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Logger.Console($"Sent {bytesSent} bytes to server.");
                sendDone.Set();
            }
            catch (Exception e)
            {
                Logger.Console(e.ToString());
            }
        }
    }
}
