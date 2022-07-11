using System;
using System.Globalization;
using System.Text;

namespace SocketsComplete
{
    public static class HexConverter
    {
        /// <summary>
        /// Converts HEX to byte array
        /// </summary>
        /// <param name="hex">HEX</param>
        /// <returns>Byte array</returns>
        public static byte[] HexToByteArray(string hex)
        {
            return HexToByteArray(Encoding.Default.GetBytes(hex));
        }

        /// <summary>
        /// Converts HEX to byte array
        /// </summary>
        /// <param name="hex">HEX array</param>
        /// <returns>byte array</returns>
        public static byte[] HexToByteArray(byte[] hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException("hex");
            }
            return HexToByteArray(hex, 0, hex.Length);
        }

        /// <summary>
        /// Converts HEX to byte array
        /// </summary>
        /// <param name="hex">HEX array</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Element count</param>
        /// <returns>Byte array</returns>
        public static byte[] HexToByteArray(byte[] hex, int offset, int count)
        {
            if (hex == null)
            {
                throw new ArgumentNullException("hex");
            }
            byte[] buffer = new byte[count / 2];

            for (int i = offset; i < offset + count; i += 2)
            {
                buffer[(i - offset) / 2] = (byte)(GetDigitFromHex((char)hex[i]) * 16 + HexConverter.GetDigitFromHex((char)hex[i + 1]));
            }
            return buffer;
        }

        /// <summary>
        /// Converts byte array to HEX
        /// </summary>
        /// <param name="buffer">byte array</param>
        /// <returns>HEX string</returns>
        public static string ByteArrayToHex(byte[] buffer)
        {
            if (buffer != null)
            {
                return ByteArrayToHex(buffer, 0, buffer.Length);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts byte array to HEX
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Element count</param>
        /// <returns>HEX string</returns>
        public static string ByteArrayToHex(byte[] buffer, int offset, int count)
        {
            if (buffer != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = offset; i < offset + count; i++)
                {
                    sb.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts byte array to HEX
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <returns>HEX string</returns>
        public static string ByteArrayToHexWithSpaces(byte[] buffer)
        {
            if (buffer != null)
            {
                return ByteArrayToHexWithSpaces(buffer, 0, buffer.Length);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts byte array to HEX
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <param name="offset">offset</param>
        /// <param name="count">Element count</param>
        /// <returns>HEX string</returns>
        public static string ByteArrayToHexWithSpaces(byte[] buffer, int offset, int count)
        {
            if (buffer != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = offset; i < offset + count; i++)
                {
                    if (i > offset)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Determine decimal value from HEX value
        /// </summary>
        /// <param name="hexDigit">HEX value</param>
        /// <returns>decimal value</returns>
        private static byte GetDigitFromHex(char hexDigit)
        {
            switch (hexDigit)
            {
                case '0':
                    return 0;

                case '1':
                    return 1;

                case '2':
                    return 2;

                case '3':
                    return 3;

                case '4':
                    return 4;

                case '5':
                    return 5;

                case '6':
                    return 6;

                case '7':
                    return 7;

                case '8':
                    return 8;

                case '9':
                    return 9;

                case 'A':
                    return 10;

                case 'B':
                    return 11;

                case 'C':
                    return 12;

                case 'D':
                    return 13;

                case 'E':
                    return 14;

                case 'F':
                    return 15;

                default:
                    throw new ArgumentException(new string(hexDigit, 1), "hexDigit");
            }
        }
    }
}