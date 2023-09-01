using Grpc.Chat;
using Grpc.Core;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7173");

var client = new Chat.ChatClient(channel);

Console.Write("Name: ");
var name = Console.ReadLine();

using var stream = client.SendMessage();
var requestStream = stream.RequestStream;
var responseStream = stream.ResponseStream;

var writeThread = new Thread(async () =>
{
    while (true)
    {
        await foreach (var message in responseStream.ReadAllAsync())
        {
            Console.WriteLine(message.Message);
        }
    }
});

writeThread.Start();


while (true)
{
    var message = Console.ReadLine();

    Console.SetCursorPosition(0, Console.CursorTop - 1);
    ClearLine();

    await requestStream.WriteAsync(new ChatMessage { Message = $"{DateTime.UtcNow}, {name}: {message}"});
}

void ClearLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentLineCursor);
}

