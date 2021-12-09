using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketsComplete.Properties;

namespace SocketsComplete {
    internal class TCPClient
    {
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public void Transmit(Socket client, byte[] packet)
        {
            try
            {
                Send(client, packet);
                //Receive(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public StateObject CreateSocket()
        {
            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Settings.Default.ServerIP);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Settings.Default.TCPPort);

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
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("Connecting...");
                
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Close(Socket client)
        {
            // Close socket
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            Console.WriteLine("Socket closed");

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
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Console.WriteLine("Receiving...");
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.socket;
                byte[] localBuffer = null;
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    Console.WriteLine("Message received...");

                    state.BytesReceived = bytesRead;
                    localBuffer = new byte[bytesRead];
                    Array.Copy(state.buffer, localBuffer, state.BytesReceived);
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                } else {
                    Console.WriteLine("Message done...");

                    Console.WriteLine("Read {0} bytes from socket. \n Data : [{1:X}]", state.BytesReceived, BitConverter.ToString(state.buffer));
                    //response = state;
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
            Console.WriteLine("Sending...");
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
