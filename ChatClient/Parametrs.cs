using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ChatClient
{
    public class Parametrs
    {
        private ParametrsModel _parametrs;
        private XmlSerializer _serializer;
        private string _pathParametrs;

        public Parametrs(string pathParametrs)
        {
            _serializer = new XmlSerializer(typeof(ParametrsModel));
            _pathParametrs = pathParametrs;
        }

        public void LoadParametrs()
        {
            try
            {
                using (Stream stream = new FileStream(_pathParametrs, FileMode.Open))
                {
                    _parametrs = (ParametrsModel)_serializer.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                _parametrs = new ParametrsModel();
                _parametrs.MaxConnection = 1;
                _parametrs.KeyConnection = "1234";
                _parametrs.MaxMessageSize = 1000;
                _parametrs.MaxMessageLength = 10;
            }
        }

        public void SaveParametrs()
        {
            using (Stream stream = new FileStream(_pathParametrs, FileMode.Create))
            {
                _serializer.Serialize(stream, _parametrs);
            }
        }
        public int MaxConnection { get { return _parametrs.MaxConnection; } }
        public string KeyConnection { get { return _parametrs.KeyConnection; } }
        public int MaxMessageSize { get { return _parametrs.MaxMessageSize; } }
        public int MaxMessageLength { get { return _parametrs.MaxMessageLength; } }
    }

    public class ParametrsModel
    {
        public int MaxConnection { set; get; }
        public string KeyConnection { set; get; }
        public int MaxMessageSize { set; get; }
        public int MaxMessageLength { set; get; }
    }
}