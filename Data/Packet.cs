using System;
using System.IO;

namespace SocketsComplete.Data {
    internal class Packet : IPacket {
        public long Imei;
        public ushort Length;
        public Gps Gps;
        public byte Checksum;
    }
}
