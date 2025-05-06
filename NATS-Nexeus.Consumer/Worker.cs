using NATS.Client.Core;
using NATS.Client.JetStream.Models;
using NATS.Client.JetStream;
using NATS.Net;
using System.Collections.Concurrent;
using NATS.Client.KeyValueStore;

namespace NATS_Nexeus_Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly INatsConnection _natsConnection;
    private readonly string ConsumerBucketKey = "consumer_bkt";
    private readonly ConcurrentDictionary<int, Task> _consumerTasks = new();
    private int _no_of_consumers = 1;
    private readonly string _streamName = "NATS";
    private readonly string _subject = "nats.topic";

    public Worker(ILogger<Worker> logger, INatsConnection natsConnection)
    {
        _logger = logger;
        _natsConnection = natsConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var js = new NatsJSContext(_natsConnection);
        var existingStreams = await ListStreamNamesAsync(js, stoppingToken);
        await CreateOrUpdateStream(js, _streamName, _subject, existingStreams, stoppingToken);

        if (_consumerTasks.IsEmpty)
        {
            await AdjustConsumers(1, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await ListenPublisherKv();
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
        }
    }

    public async Task ListenPublisherKv()
    {
        var kv = _natsConnection.CreateKeyValueStoreContext();
        var consumerBucketConfig = new NatsKVConfig(ConsumerBucketKey)
        {
            Description = "Consumer bucket",
        };
        var consumerBucket = await kv.CreateOrUpdateStoreAsync(consumerBucketConfig);

        await foreach (var kvPair in consumerBucket.WatchAsync<int>())
        {
            if (kvPair.Key == "no_of_consumers")
            {
                int newConsumerCount = Math.Max(1, kvPair.Value);
                _logger.LogInformation("Consumer count changed - Current: {CurrentConsumers}, New Target: {NewConsumers}", _consumerTasks.Count, newConsumerCount);
                await AdjustConsumers(newConsumerCount, CancellationToken.None);
            }
        }
    }

    private async Task AdjustConsumers(int newCount, CancellationToken stoppingToken)
    {
        int currentCount = _consumerTasks.Count;

        // Add new consumers
        if (newCount > currentCount)
        {
            for (int i = currentCount + 1; i <= newCount; i++)
            {
                // Start the consumer task only after the consumer count is updated
                _consumerTasks[i] = Task.Run(() => StartConsumer(i, stoppingToken), stoppingToken);
                _logger.LogInformation("Consumer {Name} added.", $"consumer-{i}");
            }
        }
        // Remove extra consumers
        else if (newCount < currentCount)
        {
            foreach (var key in _consumerTasks.Keys.Where(k => k > newCount).ToList())
            {
                if (_consumerTasks.TryRemove(key, out var consumerTask))
                {
                    // Cancel the task gracefully
                    stoppingToken.ThrowIfCancellationRequested(); // Ensure cancellation
                    consumerTask.ContinueWith(t => _logger.LogInformation("Consumer {Name} stopped.", $"consumer-{key}"));
                }
            }
        }
    }



    private async Task StartConsumer(int consumerId, CancellationToken stoppingToken)
    {
        string consumerName = $"consumer-{consumerId}";
        try
        {
            var js = new NatsJSContext(_natsConnection);
            var consumerConfig = new ConsumerConfig
            {
                Name = consumerName,
                AckPolicy = ConsumerConfigAckPolicy.Explicit
            };

            var consumer = await js.CreateOrUpdateConsumerAsync(_streamName, consumerConfig, stoppingToken);

            await foreach (var msg in consumer.ConsumeAsync<string>(cancellationToken: stoppingToken))
            {
                _logger.LogInformation("Consumer {Consumer} received message: {Message}", consumerName, msg.Data);
                await msg.AckAsync(cancellationToken: stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in consumer {Consumer}.", consumerName);
        }
    }

    private async Task CreateOrUpdateStream(NatsJSContext js, string streamName, string subject, List<string> existingNames, CancellationToken stoppingToken)
    {
        try
        {
            var streamConfig = new StreamConfig(name: streamName, subjects: [subject])
            {
                Storage = StreamConfigStorage.File,
                MaxAge = TimeSpan.FromDays(7),
                DenyDelete = true,
                DenyPurge = true
            };

            if (existingNames.Contains(streamName))
            {
                _logger.LogWarning("Stream {Name} already exists. Updating...", streamName);
                await js.UpdateStreamAsync(streamConfig, stoppingToken);
            }
            else
            {
                _logger.LogWarning("Stream {Name} does not exist. Creating...", streamName);
                await js.CreateStreamAsync(streamConfig, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stream {Name} failed to create or configure.", streamName);
        }
    }

    private static async Task<List<string>> ListStreamNamesAsync(NatsJSContext js, CancellationToken stoppingToken)
    {
        var streams = new List<string>();
        await foreach (var name in js.ListStreamNamesAsync(null, stoppingToken))
        {
            streams.Add(name);
        }
        return streams;
    }
}
