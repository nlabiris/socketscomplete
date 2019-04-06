using System;
using System.IO;

namespace SocketsComplete.Data {
    internal class Packet {
        public long Imei;
        public ushort Length;
        public Gps Gps;

        public byte[] Serialize() {
            using (MemoryStream m = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(m)) {
                    writer.Write(Imei);
                    writer.Write(Length);
                    writer.Write(Gps.Serialize());
                }
                return m.ToArray();
            }
        }
    }
}
