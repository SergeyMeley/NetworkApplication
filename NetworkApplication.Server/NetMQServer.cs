using NetworkApplication.ChatCommon;
using NetworkApplication.Infrastructure;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkApplication.Server
{
    public class NetMQServer : IDisposable
    {
        private readonly Dictionary<string, string> _clients = new Dictionary<string, string>();
        //private readonly ResponseSocket _serverSocket;
        private readonly IMessageSource<string> _messageSource;
        private bool _isRunning;

        public NetMQServer(IMessageSource<string> messageSource/*, string bindAddress = "tcp://*:5555"*/)
        {
            _messageSource = messageSource;
            //_serverSocket = new ResponseSocket();
            //_serverSocket.Bind(bindAddress);
            _isRunning = true;
        }

        public void Start()
        {
            Console.WriteLine("Сервер запущен и ожидает сообщений...");

            while (_isRunning)
            {
                try
                {
                    string clientAddress = string.Empty;
                    var message = _messageSource.Receive(ref clientAddress);

                    if (message == null)
                        continue;

                    ProcessMessage(message, clientAddress);
                }
                catch (NetMQException ex)
                {
                    Console.WriteLine($"NetMQ ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            //_serverSocket?.Dispose();
        }

        private void ProcessMessage(ChatMessage message, string clientAddress)
        {
            Console.WriteLine($"Получено сообщение от {message.FromName}: {message.Text} (Команда: {message.Command})");

            switch (message.Command)
            {
                case Command.Register:
                    RegisterClient(message, clientAddress);
                    break;

                case Command.Confirmation:
                    ConfirmMessageReceived(message.Id);
                    break;

                case Command.Message:
                    RelayMessage(message);
                    break;
            }
        }

        private void RegisterClient(ChatMessage message, string clientAddress)
        {
            Console.WriteLine($"Регистрация: {message.FromName}");
            _clients[message.FromName] = clientAddress;

            using (var ctx = new TestContext())
            {
                if (!ctx.Users.Any(u => u.Name == message.FromName))
                {
                    ctx.Users.Add(new User { Name = message.FromName });
                    ctx.SaveChanges();
                }
            }
        }

        private void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine($"Подтверждение получения сообщения ID={id}");

            using (var ctx = new TestContext())
            {
                var msg = ctx.Messages.FirstOrDefault(m => m.Id == id);
                if (msg != null)
                {
                    msg.Received = true;
                    ctx.SaveChanges();
                }
            }
        }

        private void RelayMessage(ChatMessage message)
        {
            if (_clients.TryGetValue(message.ToName, out string clientAddress))
            {
                using (var ctx = new TestContext())
                {
                    var fromUser = ctx.Users.First(u => u.Name == message.FromName);
                    var toUser = ctx.Users.First(u => u.Name == message.ToName);

                    var dbMessage = new ChatMessage
                    {
                        FromName = fromUser.Name,
                        ToName = toUser.Name,
                        Text = message.Text,
                        Received = false
                    };

                    ctx.Messages.Add(dbMessage);
                    ctx.SaveChanges();

                    var forwardMessage = new ChatMessage
                    {
                        Id = dbMessage.Id,
                        Command = Command.Message,
                        FromName = message.FromName,
                        ToName = message.ToName,
                        Text = message.Text
                    };

                    _messageSource.Send(forwardMessage, clientAddress);
                    Console.WriteLine($"Сообщение переслано от {message.FromName} к {message.ToName}");
                }
            }
            else
            {
                Console.WriteLine($"Ошибка: пользователь {message.ToName} не найден.");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}