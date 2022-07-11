using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketsComplete.Properties;

namespace SocketsComplete
{
    internal class TCPOutputControl
    {
        private Socket tcpListener;
        private OutputControlData OutputControlData;
        private bool active;
        private UDPServer UDPServer;
        private Logger Logger = new Logger("OutputControl");

        /// <summary>
        /// Constructor for TCP output control server
        /// </summary>
        /// <param name="ocd">Output control data storage</param>
        /// <param name="udp">UDP server to send outputs</param>
        public TCPOutputControl(OutputControlData ocd, UDPServer udp = null)
        {
            OutputControlData = ocd;
            UDPServer = udp;
        }

        public bool Active
        {
            get { return active; }
        }

        /// <summary>
        /// Starts the TCP output control server.
        /// </summary>
        public void Start()
        {
            if (Settings.Default.OutputControlPort != 0)
            {
                active = true;

                // Starting a sender thread to check for pending outputs and send them
                var Sender = new Thread(SendOutputs);
                Sender.Start();

                // Establish the local endpoint for the socket.
                var localTCPEndPoint = new IPEndPoint(IPAddress.Any, Settings.Default.OutputControlPort);

                // Create a TCP/IP socket.
                tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    tcpListener.Bind(localTCPEndPoint);
                    tcpListener.Listen(100);
                    Logger.Console($"Started listening at port {Settings.Default.OutputControlPort}");

                    // Start an asynchronous socket to listen for connections.
                    tcpListener.BeginAccept(new AsyncCallback(AcceptTCPCallback), tcpListener);
                }
                catch (Exception e)
                {
                    Logger.Console(e);
                }
            }
            else
                Logger.Console("TCP output control port can't be set to 0. Server start terminated.");
        }

        /// <summary>
        /// Stop the TCP output control server
        /// </summary>
        public void Stop()
        {
            Logger.Console("TCP output server manually stopped.");
            active = false;
            try
            {
                tcpListener.Close();
            }
            catch (Exception e)
            {
                Logger.Console(e);
            }
        }

        private void AcceptTCPCallback(IAsyncResult ar)
        {
            if (!active) return;
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            Logger.Console("Got new connection: " + handler.RemoteEndPoint);
            // Reinitiate socket listening
            try
            {
                tcpListener.BeginAccept(new AsyncCallback(AcceptTCPCallback), tcpListener);
            }
            catch
            {
                Logger.Console("Error in tcpListener.BeginAccept()");
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.socket = handler;

            // Begin recieving packet.
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.socket;

            // Try reading data from the client socket.
            try
            {
                state.BytesReceived = handler.EndReceive(ar);
            }
            catch
            {
                Logger.Console("Connection broke");
            }

            if (state.BytesReceived > 0)
            {
                Logger.Console($"Received {state.BytesReceived} bytes from socket.");
                ParseData(state);
                try
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                catch
                {
                    Logger.Console("Connection closed");
                }
            }
        }

        /// <summary>
        /// Parse and add output control packet to stack
        /// IMEI             OUTPUT DATA
        /// 0000000000000000 A0860100050203040506
        /// </summary>
        /// <param name="state">Socket state</param>
        private void ParseData(StateObject state)
        {
            var result = new OutputControl();
            try
            {
                result.ImeiBuffer = Helper.ReadBlock(state.buffer, 0, 15);
                var imeiString = Encoding.ASCII.GetString(result.ImeiBuffer, 0, 15);
                long.TryParse(imeiString, out long imei);
                result.Imei = imei;
                result.OutputData = Helper.ReadBlock(state.buffer, 15, 39);
                OutputControlData.AddToStack(result);
            }
            catch
            {
                Logger.Console("Error parsing data.");
            }
        }

        /// <summary>
        /// Output sender
        /// </summary>
        private void SendOutputs()
        {
            try
            {
                while (active)
                {
                    //Logger.Console($"Pending output stack: {this.OutputControlData.StackCount}");
                    foreach (var packet in OutputControlData.GetPacketsFromStack())
                    {
                        SendOutput(packet);
                    }
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                Logger.Console("Error in output sender.");
            }
        }

        /// <summary>
        /// Send output through available connection
        /// </summary>
        /// <param name="packet">Output control packet object</param>
        private void SendOutput(OutputControl packet)
        {
            Logger.Console($"Connection for imei {packet.Imei} found, trying to send output signal.");
            try
            {
                var response = new byte[packet.OutputData.Length];
                Array.Copy(packet.OutputData, response, packet.OutputData.Length);
                if (SendOutput(packet.Imei, response, OutputControlData.GetConnection(packet.Imei)))
                {
                    OutputControlData.RemoveFromStack(packet);
                }
            }
            catch
            {
                Logger.Console("Failed to send output.");
            }
        }

        /// <summary>
        /// Send output to the specified connection
        /// </summary>
        /// <param name="id">Output control packet id</param>
        /// <param name="imei">Device IMEI</param>
        /// <param name="response">Packet buffer for sending</param>
        /// <param name="connection">Device connection object</param>
        private bool SendOutput(long imei, byte[] response, object connection)
        {
            if (connection.GetType() == typeof(Socket))
            { // TCP
                try
                {
                    ((Socket)connection).Send(response, SocketFlags.None);
                }
                catch
                {
                    Logger.Console($"Can't reach TCP socket to send packet with IMEI {imei}");
                    OutputControlData.RemoveSocket(imei);
                    return false;
                }
                return true;
            }
            else if (connection.GetType() == typeof(IPEndPoint))
            { // UDP
                UDPServer.Send(response, (EndPoint)connection);
                return true;
            }

            Logger.Console("Unable to get connection type for output sending.");
            return false;
        }
    }
}
