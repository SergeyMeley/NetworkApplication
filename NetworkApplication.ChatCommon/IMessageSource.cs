using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApplication.ChatCommon
{
    public interface IMessageSourceClient
    {
        void Send(ChatMessage message, IPEndPoint ep);
        ChatMessage Receive(ref IPEndPoint ep);
    }
}
