using System;
using System.IO;
using System.Threading;
using SocketsComplete;

namespace SocketsClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Packet packet = new Packet
            {
                Imei = 351777042773935, // 8 bytes
                Length = 12,            // 2 bytes
                Data = new Data
                {
                    Lat = 38.03039F,    // 4 bytes
                    Lng = 23.70840F,    // 4 bytes
                    Speed = 25,         // 2 bytes
                    Altitude = 100      // 2 bytes
                }
            };

            //byte[] data = packet.CreatePacket();

            byte[] imei = new byte[] {
                //0x00, 0x0F, 0x33, 0x35, 0x36, 0x33, 0x30, 0x37, 0x30, 0x34, 0x32, 0x34, 0x34, 0x31, 0x30, 0x31, 0x33
                0x00, 0x0F, 0x33, 0x35, 0x37, 0x30, 0x37, 0x33, 0x32, 0x39, 0x32, 0x38, 0x33, 0x33, 0x36, 0x34, 0x33
            };

            byte[] data = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x08, 0x01, 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD8, 0xEA, 0x30, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x05, 0x02, 0x15, 0x03, 0x01, 0x01, 0x01, 0x42, 0x5E, 0x0F, 0x01, 0xF1, 0x00, 0x00, 0x60, 0x1A, 0x01, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xC7, 0xCF
            };

            byte[] locking = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x0C, 0x01, 0x05, 0x00, 0x00, 0x00, 0x12, 0x6C, 0x76, 0x63, 0x61, 0x6E, 0x63, 0x6C, 0x6F, 0x73, 0x65, 0x61, 0x6C, 0x6C, 0x64, 0x6F, 0x6F, 0x72, 0x73, 0x01, 0x00, 0x00, 0x72, 0x1C
            };

            byte[] unlocking = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1B, 0x0C, 0x01, 0x05, 0x00, 0x00, 0x00, 0x13, 0x6C, 0x76, 0x63, 0x61, 0x6E, 0x6F, 0x70, 0x65, 0x6E, 0x61, 0x6C, 0x6C, 0x64, 0x6F, 0x6F, 0x72, 0x73, 0x0D, 0x0A, 0x01, 0x00, 0x00, 0x5D, 0x32
            };

            Client client = new Client();
            var socket = client.Connect();
            Console.WriteLine("Sending IMEI...");
            client.Transmit(socket, imei);
            client.Receive(socket);
            Thread.Sleep(2000);
            Console.WriteLine("Sending data...");
            client.Transmit(socket, data);
            client.Receive(socket);
            Thread.Sleep(500);
            //client.Close(socket);

            Console.ReadLine();
        }
    }

    internal class Packet
    {
        public long Imei;
        public ushort Length;
        public Data Data;
        public byte Checksum;

        public byte[] CreatePacket()
        {
            byte[] dataWithoutCs = this.Serialize();
            this.Checksum = this.CountCheckSum(dataWithoutCs, dataWithoutCs.Length - 1);
            return this.Serialize();
        }

        private byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
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
        public byte CountCheckSum(byte[] buffer, int length)
        {
            int sum = 0;
            for (var i = 0; i < length; i++)
            {
                sum += buffer[i];
            }
            return (byte)(sum & 0xFF);
        }
    }

    internal class Data
    {
        public float Lat;
        public float Lng;
        public short Speed;
        public short Altitude;

        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
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
