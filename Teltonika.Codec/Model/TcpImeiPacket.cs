namespace Teltonika.Codec.Model
{
    public class TcpImeiPacket : IPacket
    {
        public byte Length { get; private set; }
        public long Imei { get; private set; }

        public static TcpImeiPacket Create(byte length, long imei)
        {
            return new TcpImeiPacket
            {
                Length = length,
                Imei = imei
            };
        }
    }
}
