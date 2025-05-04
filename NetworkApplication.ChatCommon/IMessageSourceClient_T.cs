using NetworkApplication.ChatCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApplication.ChatCommon
{
    public interface IMessageSourceClient<T>
    {
        public void Send(ChatMessage message, T toAddr);
        public ChatMessage Receive(ref T fromAddr);
        public T CreateNewT();
        public T GetServer();
    }
}
