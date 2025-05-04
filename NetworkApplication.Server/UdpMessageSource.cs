using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkApplication.ChatCommon;

namespace NetworkApplication.Server
{
    public class UdpMessageSource : IMessageSource<IPEndPoint>
    {
        private UdpClient udpClient;
        public UdpMessageSource()
        {
            udpClient = new UdpClient(12345);
        }
        public IPEndPoint CopyT(IPEndPoint t)
        {
            return new IPEndPoint(t.Address, t.Port);
        }
        public IPEndPoint CreateNewT()
        {
            return new IPEndPoint(IPAddress.Any, 0);
        }
        public ChatMessage Receive(ref IPEndPoint ep)
        {
            byte[] receiveBytes = udpClient.Receive(ref ep);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);
            return ChatMessage.FromJson(receivedData);
        }
        public void Send(ChatMessage message, IPEndPoint ep)
        {
            byte[] forwardBytes = Encoding.ASCII.GetBytes(message.ToJson());
            udpClient.Send(forwardBytes, forwardBytes.Length, ep);
        }
    }
}
