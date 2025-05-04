using NetMQ;
using NetMQ.Sockets;
using NetworkApplication.ChatCommon;

public class NetMQMessageSourceClient : IMessageSourceClient<string>
{
    private readonly RequestSocket _clientSocket;
    private readonly string _serverAddress;

    public NetMQMessageSourceClient(string serverAddress = "tcp://127.0.0.1:5555")
    {
        _serverAddress = serverAddress;
        _clientSocket = new RequestSocket();
        _clientSocket.Connect(_serverAddress);
    }

    public string CreateNewT()
    {
        return string.Empty; // NetMQ не требует явного endpoint для клиента
    }

    public string GetServer()
    {
        return _serverAddress;
    }

    public ChatMessage Receive(ref string endpoint)
    {
        var messageJson = _clientSocket.ReceiveFrameString();
        return ChatMessage.FromJson(messageJson);
    }

    public void Send(ChatMessage message, string endpoint)
    {
        var json = message.ToJson();
        _clientSocket.SendFrame(json);
        _clientSocket.ReceiveFrameString(); // Ждём ответ (Request-Reply паттерн)
    }

    public void Dispose()
    {
        _clientSocket?.Dispose();
    }
}