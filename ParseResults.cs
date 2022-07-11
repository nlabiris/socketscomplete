using System.Text;
using Teltonika.Codec.Model;

namespace SocketsComplete
{
    /// <summary>
    /// Results from parsing recieved data
    /// </summary>
    internal class ParseResults
    {
        /// <summary>
        /// Parsed packet
        /// </summary>
        public IPacket Packet { get; set; }

        /// <summary>
        /// Device IMEI
        /// </summary>
        public long Imei { get; set; }

        /// <summary>
        /// Packet confirmation response
        /// </summary>
        public byte[] Response { get; set; }

        /// <summary>
        /// Raw data
        /// </summary>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Raw data
        /// </summary>
        public string RawDataHex {
            get
            {
                return HexConverter.ByteArrayToHex(RawData);
            }
        }
    }
}