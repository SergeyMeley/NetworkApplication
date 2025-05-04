using NetworkApplication.ChatCommon;

public class NetMQClient : IDisposable
{
    private readonly string _name;
    private readonly IMessageSourceClient<string> _messageSource;
    private bool _isRunning;

    public NetMQClient(string name, IMessageSourceClient<string> messageSource)
    {
        _name = name;
        _messageSource = messageSource;
    }

    public void Start()
    {
        _isRunning = true;
        Register();

        var listenerThread = new Thread(ListenForMessages);
        listenerThread.Start();

        SendMessages();
    }

    private void ListenForMessages()
    {
        while (_isRunning)
        {
            try
            {
                string serverAddress = string.Empty;
                var message = _messageSource.Receive(ref serverAddress);

                Console.WriteLine($"\n[От {message.FromName}]: {message.Text}");

                if (message.Command == Command.Message)
                {
                    Confirm(message, serverAddress);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private void Register()
    {
        var message = new ChatMessage
        {
            Command = Command.Register,
            FromName = _name
        };
        _messageSource.Send(message, _messageSource.GetServer());
    }

    private void Confirm(ChatMessage message, string endpoint)
    {
        var confirmMsg = new ChatMessage
        {
            Command = Command.Confirmation,
            FromName = _name,
            Id = message.Id
        };
        _messageSource.Send(confirmMsg, endpoint);
    }

    private void SendMessages()
    {
        while (_isRunning)
        {
            Console.WriteLine("\nКому и что отправить? (Имя Текст):");
            var input = Console.ReadLine()?.Split(' ', 2);

            if (input?.Length != 2)
            {
                Console.WriteLine("Формат: Имя Текст");
                continue;
            }

            var message = new ChatMessage
            {
                Command = Command.Message,
                FromName = _name,
                ToName = input[0],
                Text = input[1]
            };

            _messageSource.Send(message, _messageSource.GetServer());
        }
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Dispose()
    {
        Stop();
        (_messageSource as IDisposable)?.Dispose();
    }
}