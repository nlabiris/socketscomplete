using SocketsComplete;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketsOutputClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            byte[] locking = HexConverter.HexToByteArray("000000000000001A0C0105000000126C7663616E636C6F7365616C6C646F6F7273010000721C");
            byte[] unlocking = HexConverter.HexToByteArray("000000000000001B0C0105000000136C7663616E6F70656E616C6C646F6F72730D0A0100005D32");

            Client client = new Client();
            var socket = client.OutputConnect();

            if (args.Length == 0)
            {
                throw new ArgumentException("wrong arguments");
            }

            var imei = Encoding.ASCII.GetBytes(args[0]);

            if (args[1] == "lock")
            {
                client.Transmit(socket, imei.Concat(locking).ToArray());
            }
            else if (args[1] == "unlock")
            {
                client.Transmit(socket, imei.Concat(unlocking).ToArray());
            }

            Thread.Sleep(2000);
            client.Close(socket);

            Console.ReadLine();
        }
    }
}
