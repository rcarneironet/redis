
using StackExchange.Redis;
using System.Text.Json;

namespace redispubsub.Publisher
{
    public class Producer(ILogger<Producer> logger) : IHostedService
    {
        private static readonly string ConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer Connection = ConnectionMultiplexer.Connect(ConnectionString);

        private const string Channel = "messages";
        private const string Message = "Sending message: {Channel} - {@Message}";
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Produce, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async void Produce(object? state)
        {
            var subscriber = Connection.GetSubscriber();
            var message = new Message
            {
                Id = Guid.NewGuid(),
                MessageValue = DateTime.UtcNow.ToString(),
            };

            var json = JsonSerializer.Serialize(message);

            await subscriber.PublishAsync(Channel, json);

            logger.LogInformation(
                "Published message: {Channel} - {@Message}",
                json,
                message);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
