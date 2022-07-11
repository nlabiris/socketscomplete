using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SocketsComplete
{
    /// <summary>
    /// Output control data storage
    /// </summary>
    internal class OutputControlData
    {
        private List<OutputControl> stack;
        private Dictionary<long, DeviceConnection> connections;

        /// <summary>
        /// Constructor
        /// </summary>
        public OutputControlData()
        {
            stack = new List<OutputControl>();
            connections = new Dictionary<long, DeviceConnection>();
        }

        /// <summary>
        /// Gets the number of pending outputs
        /// </summary>
        /// <returns>Number of pending outputs</returns>
        public int StackCount
        {
            get { return stack.Count; }
        }

        /// <summary>
        /// Inserts a device UDP endpoint to connections
        /// </summary>
        public void InsertConnection(long imei, EndPoint clientEP)
        {
            lock (connections)
            {
                if (connections.TryGetValue(imei, out DeviceConnection connection))
                {
                    connection.Set(clientEP);
                }
                else
                {
                    connections.Add(imei, new DeviceConnection(clientEP));
                }
            }
        }

        /// <summary>
        /// Inserts a device TCP socket to connections
        /// </summary>
        public void InsertConnection(long imei, Socket socket)
        {
            lock (connections)
            {
                if (connections.TryGetValue(imei, out DeviceConnection connection))
                {
                    connection.Set(socket);
                }
                else
                {
                    connections.Add(imei, new DeviceConnection(socket));
                }
            }
        }

        /// <summary>
        /// Removes TCP socket for specified imei from connections
        /// </summary>
        /// <param name="imei">Device IMEI</param>
        public void RemoveSocket(long imei)
        {
            lock (connections)
            {
                if (connections.TryGetValue(imei, out DeviceConnection connection))
                {
                    connection.Set((Socket)null);
                }
            }
        }

        /// <summary>
        /// Gets socket or endpoint of the provided device IMEI
        /// </summary>
        /// <param name="imei">Device IMEI</param>
        /// <returns>Device connection socket or endpoint as object</returns>
        public object GetConnection(long imei)
        {
            lock (connections)
            {
                if (connections.TryGetValue(imei, out DeviceConnection connection))
                {
                    if (connection.TCPSocket != null)
                        return connection.TCPSocket;
                    else
                        return connection.UDPEndPoint;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets pending outputs from stack
        /// </summary>
        /// <returns>List of pending output packets</returns>
        public List<OutputControl> GetPacketsFromStack()
        {
            var result = new List<OutputControl>();
            if (stack.Count > 0)
            {
                lock (stack)
                {
                    for (var i = 0; i < stack.Count; i++)
                    {
                        var packet = stack[i];
                        lock (connections)
                        {
                            if (connections.ContainsKey(packet.Imei))
                                result.Add(packet);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Closes TCP sockets in device connections
        /// </summary>
        public void Stop()
        {
            lock (connections)
            {
                foreach (var conn in connections.Values)
                {
                    var socket = conn.TCPSocket;
                    if (socket != null)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{nameof(Stop)}: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds output packet to pending stack
        /// </summary>
        /// <param name="packet">Output packet</param>
        public void AddToStack(OutputControl packet)
        {
            lock (stack)
            {
                stack.Add(packet);
            }
        }

        public void RemoveFromStack(OutputControl packet)
        {
            lock (stack)
            {
                stack.Remove(packet);
            }
        }
    }
}
