using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkApplication.ChatCommon;

namespace NetworkApplication.ChatCommon
{
    public interface IMessageSource
    {
        void Send(ChatMessage message, IPEndPoint ep);
        ChatMessage Receive(ref IPEndPoint ep);
    }
}
