using System;
using System.Net;
using System.Net.Sockets;
using SocketsComplete.Properties;

namespace SocketsComplete {
    /// <summary>
    /// TCP server for listening and sending packets
    /// </summary>
    internal class TCPServer {
        private const int BACKLOG = 100;

        /// <summary>
        /// Server's listener socket
        /// </summary>
        private Socket tcpListener;

        /// <summary>
        /// Active flag
        /// </summary>
        private bool active;

        /// <summary>
        /// Constructor for TCP server
        /// </summary>
        public TCPServer() {

        }

        /// <summary>
        /// Active flag
        /// </summary>
        public bool Active {
            get { return active; }
        }

        /// <summary>
        /// Starts the TCP server.
        /// </summary>
        public void Start() {
            if (Settings.Default.TCPPort <= 0) {
                throw new Exception("Invalid port");
            }

            this.active = true;
            // Establish the local endpoint for the socket.
            var localTCPEndPoint = new IPEndPoint(IPAddress.Any, Settings.Default.TCPPort);

            // Create a TCP/IP socket.
            tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try {
                tcpListener.Bind(localTCPEndPoint);
                tcpListener.Listen(BACKLOG);
                Console.WriteLine("Started listening at port {0}", Settings.Default.TCPPort);

                // Start an asynchronous socket to listen for connections.
                tcpListener.BeginAccept(new AsyncCallback(AcceptTCPCallback), tcpListener);
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Stop the TCP server
        /// </summary>
        public void Stop() {
            this.active = false;
            try {
                this.tcpListener.Close();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptTCPCallback(IAsyncResult ar) {
            if (!this.active) return;
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            // Reinitiate socket listening
            try {
                tcpListener.BeginAccept(new AsyncCallback(AcceptTCPCallback), tcpListener);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            // Begin recieving packet.
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar) {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            byte[] localBuffer = null;

            // Try reading data from the client socket.
            try {
                state.BytesReceived = handler.EndReceive(ar);
                // Copy retieved packet buffer to a local buffer for handling
                localBuffer = new byte[state.BytesReceived];
                Array.Copy(state.buffer, localBuffer, state.BytesReceived);
            } catch (Exception e) {
                Console.WriteLine("{0} - Connection broke", e);
            }

            if (state.BytesReceived > 0 && localBuffer != null) {
                Console.WriteLine("Recieved {0} bytes from socket.", state.BytesReceived);

                var parseResults = Helper.ParseData(localBuffer);
                if (parseResults.Response != null) {
                    handler.BeginSend(parseResults.Response, 0, parseResults.Response.Length, 0, new AsyncCallback(SendCallback), handler);
                }

                try {
                    // Listening for more packets
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                } catch {
                    Console.WriteLine("Connection closed.");
                }
            }
        }

        /// <summary>
        /// Complete response sending to the socket
        /// </summary>
        /// <param name="ar">Asynchronous operation state</param>
        private void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                handler.EndSend(ar);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}