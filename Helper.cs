using Teltonika.Codec.Model;

namespace SocketsComplete
{
    internal static class Helper
    {
        /// <summary>
        /// Retrieves a specific portion of the provided buffer
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="offset">Byte offset</param>
        /// <param name="length">Portion length</param>
        /// <returns>Buffer portion</returns>
        public static byte[] ReadBlock(byte[] buffer, int offset, int length)
        {
            var block = new byte[length];
            var index = 0;
            for (var i = offset; i < offset + length; i++)
            {
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
        public static byte CountCheckSum(byte[] buffer, int length)
        {
            int sum = 0;
            for (var i = 0; i < length; i++)
            {
                sum += buffer[i];
            }
            return (byte)(sum & 0xFF);
        }

        public static ParseResults ParseImei(byte[] localBuffer)
        {
            var parseResults = new ParseResults();
            var parser = new Parser();
            var packet = parser.ParseImei(localBuffer);
            if (packet == null)
            {
                return null;
            }
            parseResults.Imei = ((TcpImeiPacket)packet).Imei;
            parseResults.Packet = packet;
            parseResults.Response = new byte[] { 0x01 };
            return parseResults;
        }

        /// <summary>
        /// Parse the data from packet
        /// </summary>
        /// <param name="localBuffer">Packet buffer</param>
        /// <returns>Parse result object</returns>
        public static ParseResults ParseData(byte[] localBuffer)
        {
            var parseResults = new ParseResults();

            var parser = new Parser();
            var packet = parser.ParsePacket(localBuffer);
            parseResults.Packet = packet;
            parseResults.RawData = localBuffer;
            if (parseResults.RawDataHex == "000000000000002E0C01060000002643414E2D434F4E54524F4C20636D64206578656375746564207375636365737366756C6C792E010000346F")
            {
                parseResults.Response = new byte[] { 0x01 };
            }

            return parseResults;
        }
    }
}