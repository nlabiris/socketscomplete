using System;
using System.Collections.Generic;

namespace SocketsComplete {
    /// <summary>
    /// Results from parsing recieved data
    /// </summary>
    internal class ParseResults {
        /// <summary>
        /// Constructor for ParseResults
        /// </summary>
        public ParseResults()
        {
            Packets = new List<IPacket>();
        }

        /// <summary>
        /// List of parsed packets
        /// </summary>
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// Device IMEI buffer
        /// </summary>
        public byte[] ImeiBuffer { get; set; }

        /// <summary>
        /// Packet confirmation response
        /// </summary>
        public byte[] Response { get; set; }

        /// <summary>
        /// Returns device imei as long type
        /// </summary>
        /// <returns>Device imei</returns>
        public long Imei
        {
            get
            {
                return BitConverter.ToInt64(this.ImeiBuffer, 0);
            }
        }
    }
}