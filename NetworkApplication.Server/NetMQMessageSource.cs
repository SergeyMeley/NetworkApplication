using NetMQ;
using NetMQ.Sockets;
using NetworkApplication.ChatCommon;

public class NetMQMessageSource : IMessageSource<string>, IDisposable
{
    private readonly ResponseSocket _serverSocket;
    private readonly string _bindAddress;

    public NetMQMessageSource(string bindAddress = "tcp://*:5555")
    {
        _bindAddress = bindAddress;
        _serverSocket = new ResponseSocket();
        _serverSocket.Bind(_bindAddress); // Сервер слушает порт 5555
    }

    public string CopyT(string endpoint)
    {
        return endpoint; // NetMQ не требует глубокого копирования
    }

    public string CreateNewT()
    {
        return string.Empty; // Не используется в NetMQ
    }

    public ChatMessage Receive(ref string endpoint)
    {
        var messageJson = _serverSocket.ReceiveFrameString();
        return ChatMessage.FromJson(messageJson);
    }

    public void Send(ChatMessage message, string endpoint)
    {
        var json = message.ToJson();
        _serverSocket.SendFrame(json); // Ответ клиенту
    }

    public void Dispose()
    {
        _serverSocket?.Dispose();
    }
}