using System;
using System.IO;

namespace SocketsComplete.Data {
    internal class Gps {
        public float Lat;
        public float Lng;
        public short Speed;
        public short Altitude;

        public byte[] Serialize() {
            using (MemoryStream m = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(m)) {
                    writer.Write(Lat);
                    writer.Write(Lng);
                    writer.Write(Speed);
                    writer.Write(Altitude);
                }
                return m.ToArray();
            }
        }
    }
}
