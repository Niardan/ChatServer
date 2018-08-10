namespace Network.Utils
{
    public interface ISerializer
    {
        byte[] Serialize(object value);
        T Deserialize<T>(byte[] bytes);
    }
}