using System;
using System.Collections.Generic;

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

            while (offset < buffer.Length) {
                byte packetSize = buffer[offset];
                offset++;
                if (buffer.Length >= offset + packetSize) {
                    offset += packetSize;
                } else {
                    return;
                }
            }
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