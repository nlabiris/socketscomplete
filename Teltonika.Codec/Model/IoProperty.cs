using System;
using System.Linq;

namespace Teltonika.Codec.Model
{
    public struct IoProperty
    {
        public int Id { get; private set; }
        public long? Value { get; private set; }
        public byte[] ArrayValue { get; private set; }
        public string RawValue
        {
            get
            {
                return string.Join(
                    string.Empty,
                    Convert.ToString(Value.Value, 2)
                    .Select(
                        (x, i) => i > 0 && i % 4 == 0 ? string.Format(" {0}", x) : x.ToString()
                    )
                );
            }
        }

        /// <summary>
        /// Creates IoProperty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IoProperty Create(int id, long value)
        {
            return new IoProperty
            {
                Id = id,
                Value = value
            };
        }

        public static IoProperty Create(int id, byte[] value)
        {
            return new IoProperty()
            {
                Id = id,
                ArrayValue = value
            };
        }
    }
}
