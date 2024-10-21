using redispubsub.Publisher;
using StackExchange.Redis;
using System.Text.Json;

namespace redispubsub.Consumer
{
    public class Consume(ILogger<Consume> logger) : IHostedService
    {
        private static readonly string ConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer Connection = ConnectionMultiplexer.Connect(ConnectionString);

        private const string Channel = "messages";
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ConsumeJob, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async void ConsumeJob(object? state)
        {
            var subscriber = Connection.GetSubscriber();

            await subscriber.SubscribeAsync(Channel, (channel, message) =>
            {
                var json = JsonSerializer.Deserialize<Message>(message);

                logger.LogInformation(
                    "Received message: {Channel} - {@Message}",
                    channel,
                    message);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
