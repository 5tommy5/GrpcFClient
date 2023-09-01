using Grpc.Core;

namespace Grpc.ChatServer.Services
{
    public class ChatService :Chat.ChatBase
    {
        private readonly ILogger<ChatService> _logger;
        private readonly MessageStreamingService _streamingService;
        public ChatService(ILogger<ChatService> logger, MessageStreamingService streamingService)
        {
            _logger = logger;
            _streamingService = streamingService;
        }
        public override async Task SendMessage(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            _streamingService.Subscribe(responseStream);

            await foreach(var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Received message: {message.Message}");
                await _streamingService.SendMessage(message);
            }
        }

        public void WaitForMessages()
        {
            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
