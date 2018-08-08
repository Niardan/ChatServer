using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Messages;
using MessagePack;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //IMessage message = new RequestMessage("5", "test", "recive");

           // message.TypeMessage = TypeMessage.Receive;
          
           // var bytes = MessagePackSerializer.Serialize(message);
            //var rezutMessage = MessagePackSerializer.Deserialize<IMessage>(bytes);
            //switch (rezutMessage)
            //{
            //    case RequestMessage test:
            //        break;

            //}
            //var Testmessage = (RequestMessage) rezutMessage;
         //   Console.WriteLine(rezutMessage.Id);
        }
    }
}
