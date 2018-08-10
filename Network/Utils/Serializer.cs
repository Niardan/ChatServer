namespace Network.Utils
{
    public abstract class Serializer : ISerializer
    {
        public abstract byte[] Serialize(object value);
        public abstract T Deserialize<T>(byte[] bytes);
    }
}