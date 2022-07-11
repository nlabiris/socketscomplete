using System;
using System.IO;
using System.Text;
using Teltonika.Codec;
using Teltonika.Codec.Model;

namespace SocketsComplete
{
    internal class Parser
    {
        /// <summary>
        /// Parses the IMEI.
        /// </summary>
        /// <param name="localBuffer">The local buffer.</param>
        /// <returns></returns>
        public IPacket ParseImei(byte[] localBuffer)
        {
            if (localBuffer == null)
            {
                throw new ArgumentNullException("buffer is null");
            }

            var imeiLengthBlock = Helper.ReadBlock(localBuffer, 0, 2);
            var imeiLength = imeiLengthBlock[1];
            var imeiBlock = Helper.ReadBlock(localBuffer, 2, imeiLength);
            var imeiString = Encoding.ASCII.GetString(imeiBlock, 0, imeiLength);
            if (!long.TryParse(imeiString, out long imei))
            {
                return null;
            }

            return TcpImeiPacket.Create(imeiLength, imei);
        }

        /// <summary>
        /// Analyses packets
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        public IPacket ParsePacket(byte[] buffer, int offset = 0)
        {
            TcpDataPacket packet = null;
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer is null");
            }

            var firstByte = Helper.ReadBlock(buffer, 0, 1);
            if (firstByte[0] == 0xFF) // ping
            {
                return null;
            }

            var preambleLengthBlock = Helper.ReadBlock(buffer, 0, 8);

            var preamble = BytesSwapper.Swap(BitConverter.ToInt32(preambleLengthBlock, 0));
            var length = BytesSwapper.Swap(BitConverter.ToInt32(preambleLengthBlock, 4)) + 4; // + 4 crc bytes

            if (preamble != 0)
            {
                throw new NotSupportedException();
            }

            using (var reader = new ReverseBinaryReader(new MemoryStream(buffer)))
            {
                var decoder = new DataDecoder(reader);
                packet = decoder.DecodeTcpData();
            }

            if (packet.codecId == 12)
            {
                return null;
            }

            return packet;
        }
    }
}