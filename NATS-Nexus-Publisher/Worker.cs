using NATS.Client.Core;
using NATS.Client.KeyValueStore;
using NATS.Net;

namespace NATS_Nexus.Publisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly INatsConnection _natsConnection;
        private readonly string PublisherBucketKey = "publisher_bkt";
        private int _publishRate = 1; // Default: 1 message per second
        private int _intervalMs = 60000; // Default interval: 1000ms (1 second)

        public Worker(ILogger<Worker> logger, INatsConnection connection)
        {
            _logger = logger;
            _natsConnection = connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start listening for KV changes
            _ = ListenPublisherKv();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing {Rate} messages per second at {Time}", _publishRate, DateTimeOffset.Now);

                for (int i = 0; i < _publishRate; i++)
                {
                    await _natsConnection.PublishAsync("nats.topic", "Hello from Worker!");
                }

                await Task.Delay(_intervalMs, stoppingToken);
            }
        }

        public async Task ListenPublisherKv()
        {
            var kv = _natsConnection.CreateKeyValueStoreContext();
            var publisherBucketConfig = new NatsKVConfig(PublisherBucketKey)
            {
                Description = "Publisher bucket",
            };
            var publisherBucket = await kv.CreateOrUpdateStoreAsync(publisherBucketConfig);

            await foreach (var kvPair in publisherBucket.WatchAsync<int>())
            {
                if (kvPair.Key == "publish_rate")
                {
                    _publishRate = Math.Max(1, kvPair.Value); // Ensure at least 1 message per second
                    _intervalMs = 1000 / _publishRate; // Adjust interval based on rate

                    _logger.LogInformation("Publish rate changed: {PublishRate} messages per second", _publishRate);
                }
            }
        }
    }
}
