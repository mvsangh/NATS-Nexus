using NATS.Client.Core;
using NATS.Client.KeyValueStore;
using NATS.Net;

namespace NATS_Nexus.Émetteur
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly INatsConnection _natsConnection;
        private readonly string PublisherBucketKey = "publisher_bkt";

        public Worker(ILogger<Worker> logger, INatsConnection connection)
        {
            _logger = logger;
            _natsConnection = connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task ListenPublisherKv()
        {
            var kv = _natsConnection.CreateKeyValueStoreContext();
            var publisherBucket = await kv.GetStoreAsync(PublisherBucketKey);
            if (publisherBucket == null)
            {
                _logger.LogInformation("Publisher bucket not found");
                return;
            }
            await Task.Run(async () =>
            {
                await foreach (var kvPair in publisherBucket.WatchAsync<int>())
                {
                    if (kvPair.Key == "publish_rate")
                    {
                        _logger.LogInformation("Publish rate changed to {PublishRate}", kvPair.Value);
                    }
                }
            });
        }

    }
}
