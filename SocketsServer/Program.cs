using System;
using System.Threading;
using SocketsComplete;

namespace SocketsServer {
    internal class Program {
        private static void Main(string[] args) {
            Server server = new Server();
            server.Start();
            while (server.IsActive()) {
                var stopString = Console.ReadLine();

                if (stopString != null && stopString.ToLower() == "stop") {
                    server.Stop();
                }

                Thread.Sleep(500);
            }
        }
    }
}
