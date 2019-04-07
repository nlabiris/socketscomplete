using System;
using System.Collections.Generic;
using SocketsComplete.Data;

namespace SocketsComplete {
    internal class Parser {
        private List<IPacket> parsedPackets;

        /// <summary>
        /// Constructor for parser
        /// </summary>
        public Parser() {
            this.parsedPackets = new List<IPacket>();
        }

        public List<IPacket> GetPackets() {
            return this.parsedPackets;
        }

        /// <summary>
        /// Analyses packets
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        public void ParsePacket(byte[] buffer, int offset = 0) {
            if (buffer == null) {
                throw new ArgumentNullException("buffer");
            }

            parsedPackets.Clear();

            byte packetSize = buffer[offset];
            Packet data = new Packet();
            Gps gps = new Gps();
            gps.Lat = BitConverter.ToSingle(buffer, offset + 0);
            gps.Lng = BitConverter.ToSingle(buffer, offset + 4);
            gps.Speed = BitConverter.ToInt16(buffer, offset + 8);
            gps.Altitude = BitConverter.ToInt16(buffer, offset + 10);
            data.Gps = gps;

            parsedPackets.Add(data);
        }

        /// <summary>
        /// Checks is the time valid
        /// </summary>
        /// <param name="time">time</param>
        /// <returns>Is time valid</returns>
        private static bool IsValid(DateTime time) {
            DateTime now = DateTime.UtcNow;
            if (now > time) {
                return true;
            } else {
                return time.Subtract(now).TotalDays < 1;
            }
        }

        /// <summary>
        /// Parses time
        /// </summary>
        /// <param name="buffer">Data buffer</param>
        /// <param name="offset">offset</param>
        /// <returns>time</returns>
        private static DateTime ParseTime(byte[] buffer, int offset) {
            uint dif = BitConverter.ToUInt32(buffer, offset) >> 4;
            return new DateTime(2008, 1, 1).AddSeconds(dif * 2);
        }

        /// <summary>
        /// Displays detailed data to log
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length</param>
        private void DisplayDetailedData(byte[] buffer, int startIndex, int length) {
            if (buffer.Length <= startIndex) {
                startIndex = buffer.Length - 1;
            }
            if (startIndex + length > buffer.Length) {
                length = buffer.Length - startIndex;
            }
            Console.WriteLine(HexConverter.ByteArrayToHex(buffer, startIndex, length));
        }
    }
}