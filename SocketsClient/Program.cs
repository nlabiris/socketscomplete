using System;
using System.IO;
using System.Threading;
using SocketsComplete;

namespace SocketsClient {
    internal class Program {
        private static void Main(string[] args) {
            Packet packet = new Packet {
                Imei = 351777042773935,
                Length = 12,
                Gps = new Gps {
                    Lat = 38.03039F,
                    Lng = 23.70840F,
                    Speed = 25,
                    Altitude = 100
                }
            };

            byte[] data = packet.Serialize();

            for (int i = 0; i < data.Length; i++) {
                Console.Write("{0:X} ", data[i]);
            }

            Client client = new Client("127.0.0.1");
            client.Transmit(data);
        }
    }

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
