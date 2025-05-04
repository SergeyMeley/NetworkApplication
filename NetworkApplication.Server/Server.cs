using NetworkApplication.ChatCommon;
using NetworkApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApplication.Server
{
    public class Server<T>
    {
        Dictionary<String, T> clients = new Dictionary<string, T>();
        IMessageSource<T> messageSource;
        public Server(IMessageSource<T> source)
        {
            messageSource = source;
        }
        void Register(ChatMessage message, T fromep)
        {
            Console.WriteLine("Message Register, name = " + message.FromName);
            clients.Add(message.FromName, fromep);


            using (var ctx = new TestContext())
            {
                if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName) != null) return;
                ctx.Add(new User { Name = message.FromName });
                ctx.SaveChanges();
            }
        }
        void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);
            using (var ctx = new TestContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);
                if (msg != null)
                {
                    msg.Received = true;
                    ctx.SaveChanges();
                }
            }
        }
        void RelyMessage(ChatMessage message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName, out T ep))
            {
                using (var ctx = new TestContext())
                {
                    var fromUser = ctx.Users.First(x => x.Name == message.FromName);
                    var toUser = ctx.Users.First(x => x.Name == message.ToName);
                    var msg = new ChatMessage
                    {
                        FromName = fromUser.Name,
                        ToName = toUser.Name,
                        Received = false,
                        Text = message.Text
                    };
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                    id = msg.Id;
                }
                var forwardMessage = new ChatMessage()
                {
                    Id = id,
                    Command = Command.Message,
                    ToName = message.ToName,
                    FromName = message.FromName,
                    Text = message.Text
                };
                messageSource.Send(forwardMessage, ep);
                Console.WriteLine($"Message Relied, from = {message.FromName} to = { message.ToName}");
 }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }
        void ProcessMessage(ChatMessage message, T fromep)
        {
            Console.WriteLine($"Получено сообщение от {message.FromName} для {message.ToName} с командой { message.Command}:");
        Console.WriteLine(message.Text);
            if (message.Command == Command.Register)
            {
                Register(message, messageSource.CopyT(fromep));

            }
            if (message.Command == Command.Confirmation)
            {
                Console.WriteLine("Confirmation receiver");
                ConfirmMessageReceived(message.Id);
            }
            if (message.Command == Command.Message)
            {
                RelyMessage(message);
            }
        }
        bool work = true;
        public void Stop()
        {
            work = false;
        }

        public void Work()
        {
            Console.WriteLine("UDP Клиент ожидает сообщений...");
            while (work)
            {
                try
                {
                    T remoteEndPoint = messageSource.CreateNewT();
                    var message = messageSource.Receive(ref remoteEndPoint);
                    if (message == null)
                        return;
                    ProcessMessage(message, remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }
    }

}
