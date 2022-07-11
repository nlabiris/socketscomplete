using System;

namespace SocketsComplete
{
    internal class OutputControl
    {
        public byte[] ImeiBuffer { get; set; }
        public byte[] OutputData { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long Imei { get; set; }
    }
}
