namespace ChatServer.Owner
{
    public class Owner : IOwner
    {
        private readonly string _id;
        public Owner(string id)
        {
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }
    }
}