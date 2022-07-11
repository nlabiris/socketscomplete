using System;
using System.Net;
using System.Net.Sockets;
using SocketsComplete.Properties;

namespace SocketsComplete
{
    /// <summary>
    /// UDP server for listening and sending packets
    /// </summary>
    internal class UDPServer
    {
        private byte[] buffer = new byte[1024];
        private Socket UDPSocket;
        private OutputControlData OutputControlData;
        private bool active;
        private Logger Logger = new Logger("UDP");

        /// <summary>
        /// Constructor for UDP server
        /// </summary>
        /// <param name="ocd">Output control data storage</param>
        public UDPServer(OutputControlData ocd)
        {
            OutputControlData = ocd;
        }

        public bool Active
        {
            get { return active; }
        }

        /// <summary>
        /// Starts the UDP server.
        /// </summary>
        public void Start()
        {
            if (Settings.Default.UDPPort != 0)
            {
                active = true;
                // Setup the socket and buffer.
                UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                UDPSocket.ExclusiveAddressUse = false;
                UDPSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                const int SIO_UDP_CONNRESET = -1744830452;
                byte[] inValue = { 0, 0, 0, 0 };     // == false
                byte[] outValue = { 0, 0, 0, 0 };    // initialize to 0
                UDPSocket.IOControl(SIO_UDP_CONNRESET, inValue, outValue);
                UDPSocket.Bind(new IPEndPoint(IPAddress.Any, Settings.Default.UDPPort));
                buffer = new byte[1024];

                // Start listening for a new packet.
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                Logger.Console($"Started listening at port {Settings.Default.UDPPort}");
                UDPSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, UDPSocket);
            }
            else
            {
                Logger.Console("UDP port can't be set to 0. Server start terminated.");
            }
        }

        /// <summary>
        /// Send a packet to the given endpoint
        /// </summary>
        /// <param name="response">Packet buffer</param>
        /// <param name="clientEP">Client endpoint</param>
        public void Send(byte[] response, EndPoint clientEP)
        {
            UDPSocket.SendTo(response, clientEP);
            Logger.Console($"Output sent to: {((IPEndPoint)clientEP).Address}:{((IPEndPoint)clientEP).Port}");
        }

        /// <summary>
        /// Stop the UDP server
        /// </summary>
        public void Stop()
        {
            Logger.Console("UDP server manually stopped.");
            active = false;
            try
            {
                UDPSocket.Shutdown(SocketShutdown.Both);
                UDPSocket.Close();
            }
            catch (Exception)
            {
            }
        }

        private void DoReceiveFrom(IAsyncResult iar)
        {
            try
            {
                if (!active) return;
                // Get the received packet.
                var recvSock = (Socket)iar.AsyncState;
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int packetLength = recvSock.EndReceiveFrom(iar, ref clientEP);
                Logger.Console($"Received packet from: {((IPEndPoint)clientEP).Address}:{((IPEndPoint)clientEP).Port}");

                // Copy revieved packet to local buffer
                byte[] localBuffer = new byte[packetLength];
                Array.Copy(buffer, localBuffer, packetLength);

                //Start listening for a new packet.
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                UDPSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, UDPSocket);

                Logger.Console("Parsing recieved data.");
                Logger.Console("---- Parser log start ----");

                var parseResults = Helper.ParseData(localBuffer);
                if (parseResults != null)
                {
                    Logger.Console(parseResults);
                    OutputControlData.InsertConnection(parseResults.Imei, clientEP);

                    Logger.Console("---- Parser log end ----");
                    if (parseResults.Response != null)
                    {
                        UDPSocket.SendTo(parseResults.Response, clientEP);
                    }
                }
            }
            catch
            {
                Logger.Console("Unable to recieve UDP packet.");
            }
        }
    }
}