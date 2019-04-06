using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketsComplete.Properties;

namespace SocketsComplete {
    internal class TCPClient {
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private string ServerIp { get; set; }

        public TCPClient(string serverIp) {
            this.ServerIp = serverIp;
        }

        public void Transmit(byte[] packet) {
            // Connect to a remote device.
            try {
                // Establish the remote endpoint for the socket.
                // The name of the
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.GetHostEntry(this.ServerIp);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Settings.Default.TCPPortClient);

                // Create a TCP/IP socket.
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Console.WriteLine("Connection acquired...");

                // Send test data to the remote device.
                Send(client, packet);
                sendDone.WaitOne();

                Console.WriteLine("Message sent...");

                Receive(client);
                receiveDone.WaitOne();

                //Console.WriteLine("Read {0} bytes from socket. \n Data : [{1:X}]", response.BytesRecieved, BitConverter.ToString(response.buffer));

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar) {
            Console.WriteLine("Connecting...");
            try {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client) {
            try {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar) {
            Console.WriteLine("Receiving...");
            try {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
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
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, byte[] data) {
            // Begin sending the data to the remote device.
            client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar) {
            Console.WriteLine("Sending...");
            try {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                sendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
