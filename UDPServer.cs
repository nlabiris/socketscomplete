using System;
using System.Net;
using System.Net.Sockets;
using SocketsComplete.Properties;

namespace SocketsComplete {
    /// <summary>
    /// UDP server for listening and sending packets
    /// </summary>
    internal class UDPServer {
        private static bool? isMono;
        private byte[] buffer = new byte[1024];
        private Socket UDPSocket;
        private bool active;

        /// <summary>
        /// Constructor for UDP server
        /// </summary>
        public UDPServer() {

        }

        public bool Active {
            get { return active; }
        }

        /// <summary>
        /// Starts the UDP server.
        /// </summary>
        public void Start() {
            if (Settings.Default.UDPPort != 0) {
                this.active = true;
                // Setup the socket and buffer.
                UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                if (!IsMono()) {
                    UDPSocket.ExclusiveAddressUse = false;
                    UDPSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    const int SIO_UDP_CONNRESET = -1744830452;
                    byte[] inValue = { 0, 0, 0, 0 };     // == false
                    byte[] outValue = { 0, 0, 0, 0 };    // initialize to 0
                    UDPSocket.IOControl(SIO_UDP_CONNRESET, inValue, outValue);
                }
                UDPSocket.Bind(new IPEndPoint(IPAddress.Any, Settings.Default.UDPPort));
                buffer = new byte[1024];

                // Start listening for a new packet.
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                Console.WriteLine("Started listening at port {0}", Settings.Default.UDPPort);
                UDPSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, UDPSocket);
            } else {
                Console.WriteLine("UDP port can't be set to 0. Server start terminated.");
            }
        }

        /// <summary>
        /// Send a packet to the given endpoint
        /// </summary>
        /// <param name="response">Packet buffer</param>
        /// <param name="clientEP">Client endpoint</param>
        public void Send(byte[] response, EndPoint clientEP) {
            UDPSocket.SendTo(response, clientEP);
            Console.WriteLine("Output sent to: {0}:{1}", ((IPEndPoint)clientEP).Address, ((IPEndPoint)clientEP).Port);
        }

        /// <summary>
        /// Stop the UDP server
        /// </summary>
        public void Stop() {
            Console.WriteLine("UDP server manually stopped.");
            this.active = false;
            try {
                this.UDPSocket.Shutdown(SocketShutdown.Both);
                this.UDPSocket.Close();
            } catch (Exception) {
            }
        }

        private static bool IsMono() {
            if (isMono == null) {
                Type t = Type.GetType("Mono.Runtime");
                isMono = (t != null);
            }
            return isMono.Value;
        }

        private void DoReceiveFrom(IAsyncResult iar) {
            try {
                if (!this.active) return;
                // Get the received packet.
                var recvSock = (Socket)iar.AsyncState;
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int packetLength = recvSock.EndReceiveFrom(iar, ref clientEP);
                Console.WriteLine("Recieved packet from: {0}:{1}", ((IPEndPoint)clientEP).Address, ((IPEndPoint)clientEP).Port);

                // Copy revieved packet to local buffer
                byte[] localBuffer = new byte[packetLength];
                Array.Copy(buffer, localBuffer, packetLength);

                //Start listening for a new packet.
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                UDPSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, UDPSocket);

                Console.WriteLine("Parsing recieved data.");
                Console.WriteLine("---- Parser log start ----");

                var parseResults = Helper.ParseData(localBuffer);
                if (parseResults != null) {
                    Console.WriteLine(parseResults);

                    Console.WriteLine("---- Parser log end ----");
                    if (parseResults.Response != null) {
                        UDPSocket.SendTo(parseResults.Response, clientEP);
                    }
                }
            } catch {
                Console.WriteLine("Unable to recieve UDP packet.");
            }
        }
    }
}