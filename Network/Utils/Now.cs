namespace Network.Utils
{
    public abstract class Now : INow
    {
        public abstract double Get { get; }
    }
}