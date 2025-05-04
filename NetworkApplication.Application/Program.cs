using NetworkApplication.Client;
using NetworkApplication.Server;
using System.Net;


//if (args.Length == 0)
//{
//    var s = new Server<IPEndPoint>(new UdpMessageSource());
//    s.Work();
//}
//else
//    if (args.Length == 3)
//{
//    var c = new Client<IPEndPoint>(args[0], new UdpMessageSourceClient(int.Parse(args[2]), args[1], 12345));
//    c.Start();
//}
//else
//{
//    Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
//    Console.WriteLine("Для запуска клиента введите ник-нейм и IP сервера как параметры запуска приложения");
//}


if (args.Length == 0)
{
    // Запуск сервера
    Console.WriteLine("Запуск сервера NetMQ...");
    //var messageSource = new NetMQMessageSource(); // Реализация IMessageSource<string>
    //var server = new NetMQServer(messageSource);
    //server.Start();

    using (var messageSource = new NetMQMessageSource())
    using (var server = new NetMQServer(messageSource))
    {
        server.Start();
        Console.ReadKey();
    }
}
else if (args.Length == 2)
{
    // Запуск клиента
    Console.WriteLine($"Запуск клиента {args[0]}...");
    var messageSource = new NetMQMessageSourceClient($"tcp://{args[1]}:5555"); // Адрес сервера
    var client = new NetMQClient(args[0], messageSource);
    client.Start();
}
else
{
    Console.WriteLine("Использование:");
    Console.WriteLine("  Сервер: NetworkApplication.exe");
    Console.WriteLine("  Клиент: NetworkApplication.exe <никнейм> <IP_сервера>");
    Console.WriteLine("Пример клиента: NetworkApplication.exe Alice 127.0.0.1");
}