using System;
using System.IO;
using System.Threading;
using SocketsComplete;

namespace SocketsClient
{
    internal class Program {
        private static void Main(string[] args) {
            Packet packet = new Packet {
                Imei = 351777042773935, // 8 bytes
                Length = 12,            // 2 bytes
                Data = new Data {
                    Lat = 38.03039F,    // 4 bytes
                    Lng = 23.70840F,    // 4 bytes
                    Speed = 25,         // 2 bytes
                    Altitude = 100      // 2 bytes
                }
            };

            byte[] data = packet.CreatePacket();

            //for (int i = 0; i < data.Length; i++) {
            //    Console.Write("{0:X} ", data[i]);
            //}

            Client client = new Client();
            var socket = client.Connect();
            for (int j = 0; j < 20; j++)
            {
                client.Transmit(socket, data);
                Thread.Sleep(500);
            }
            client.Close(socket);
            Console.ReadLine();
        }
    }

    internal class Packet {
        public long Imei;
        public ushort Length;
        public Data Data;
        public byte Checksum;

        public byte[] CreatePacket() {
            byte[] dataWithoutCs = this.Serialize();
            this.Checksum = this.CountCheckSum(dataWithoutCs, dataWithoutCs.Length - 1);
            return this.Serialize();
        }

        private byte[] Serialize() {
            using (MemoryStream m = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(m)) {
                    writer.Write(Imei);
                    writer.Write(Length);
                    writer.Write(Data.Serialize());
                    writer.Write(Checksum);
                }
                return m.ToArray();
            }
        }

        /// <summary>
        /// Counts the checksum of the provided buffer
        /// </summary>
        /// <param name="buffer">Packet buffer</param>
        /// <param name="length">Bytes to count from start</param>
        /// <returns>Checksum</returns>
        public byte CountCheckSum(byte[] buffer, int length) {
            int sum = 0;
            for (var i = 0; i < length; i++) {
                sum += buffer[i];
            }
            return (byte)(sum & 0xFF);
        }
    }

    internal class Data {
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
