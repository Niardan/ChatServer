﻿using System.IO;
using System.Xml.Serialization;

namespace ChatServer
{
    public class Parametrs
    {
        private readonly XmlSerializer _serializer;
        private readonly string _pathParametrs;

        private ParametrsModel _parametrs;

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
            catch
            {
                _parametrs = new ParametrsModel();
                _parametrs.MaxConnection = 10;
                _parametrs.KeyConnection = "1234";
                _parametrs.MaxMessageSize = 1000;
                _parametrs.MaxMessageLength = 10;
                _parametrs.Port = 41200;
                _parametrs.Timeout = 5000;
                SaveParametrs();
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
        public int Port { get { return _parametrs.Port; } }
        public int Timeout { get { return _parametrs.Timeout; } }
    }

    public class ParametrsModel
    {
        public int MaxConnection { set; get; }
        public string KeyConnection { set; get; }
        public int MaxMessageSize { set; get; }
        public int MaxMessageLength { set; get; }
        public int Port { set; get; }
        public int Timeout { set; get; }
    }
}