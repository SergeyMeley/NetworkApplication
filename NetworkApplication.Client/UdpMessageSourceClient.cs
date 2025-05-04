using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkApplication.ChatCommon;

namespace NetworkApplication.Client
{
    public class UdpMessageSourceClient : IMessageSourceClient<IPEndPoint>
    {
        private UdpClient client;
        private IPEndPoint ep;
        public UdpMessageSourceClient(int p, string ip, int r)
        {
            client = new UdpClient(p);
            ep = new IPEndPoint(IPAddress.Parse(ip), r);
        }
        public IPEndPoint CreateNewT()
        {
            return new IPEndPoint(IPAddress.Any, 0);
        }
        public IPEndPoint GetServer()
        {
            return ep;
        }
        public ChatMessage Receive(ref IPEndPoint iPEndPoint)
        {
            byte[] receiveBytes = client.Receive(ref iPEndPoint);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);
            var messageReceived = ChatMessage.FromJson(receivedData);
            return messageReceived;
        }
        public void Send(ChatMessage message, IPEndPoint iPEndPoint)
        {
            var json = message.ToJson();
            var b = Encoding.ASCII.GetBytes(json);
            client.Send(b, b.Length, iPEndPoint);
        }
    }
}
