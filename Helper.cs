using System;
using System.Collections.Generic;

namespace SocketsComplete {
    internal static class Helper {
        /// <summary>
        /// Retrieves the data section of a packet
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="index">Start index</param>
        /// <returns>Object with data information of a package</returns>
        public static PacketData GetData(byte[] buffer, int index) {
            var packet = new PacketData();
            packet.Length = BitConverter.ToUInt16(ReadBlock(buffer, index, 2), 0);
            var data = ReadBlock(buffer, index + 2, packet.Length);
            packet.Data = ReadBlock(data, 0, packet.Length);

            return packet;
        }

        /// <summary>
        /// Retrieves a specific portion of the provided buffer
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="offset">Byte offset</param>
        /// <param name="length">Portion length</param>
        /// <returns>Buffer portion</returns>
        public static byte[] ReadBlock(byte[] buffer, int offset, int length) {
            var block = new byte[length];
            var index = 0;
            for (var i = offset; i < offset + length; i++) {
                block[index++] = buffer[i];
            }
            return block;
        }

        /// <summary>
        /// Counts the checksum of the provided buffer
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="length">Bytes to count from start</param>
        /// <returns>Checksum</returns>
        public static byte CountCheckSum(byte[] buffer, int length) {
            int sum = 0;
            for (var i = 0; i < length; i++) {
                sum += buffer[i];
            }
            return (byte)(sum & 0xFF);
        }

        /// <summary>
        /// Parse the data from packet
        /// </summary>
        /// <param name="localBuffer">Packet buffer</param>
        /// <returns>Parse result object</returns>
        public static ParseResults ParseData(byte[] localBuffer) {
            ParseResults parseResults = new ParseResults();
            parseResults.ImeiBuffer = Helper.ReadBlock(localBuffer, 0, 8);
            var cs = localBuffer[localBuffer.Length - 1];

            var packets = new List<PacketData>();

            if (cs == Helper.CountCheckSum(localBuffer, localBuffer.Length - 1)) {
                var lengthIndex = 8;
                var parser = new Parser();
                var packetData = Helper.GetData(localBuffer, lengthIndex);
                packets.Add(packetData);
                parser.ParsePacket(packets[0].Data);
                parseResults.Packets.AddRange(parser.GetPackets());
            }
            return parseResults;
        }
    }
}