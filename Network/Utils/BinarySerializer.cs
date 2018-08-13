using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network.Utils
{
    public class BinarySerializer : Serializer
    {
        private readonly BinaryFormatter _formatter = new BinaryFormatter();
        public override byte[] Serialize(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                _formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public override T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                T value = (T)_formatter.Deserialize(stream);
                return value;
            }
        }
    }
}