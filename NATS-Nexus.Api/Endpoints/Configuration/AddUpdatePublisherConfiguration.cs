namespace NATS_Nexus.Api.Endpoints.Configuration;

public class AddUpdatePublisherConfiguration
{
    private readonly ILogger<AddUpdatePublisherConfiguration> _logger;
    private readonly INatsConnection _natsConnection;
    public AddUpdatePublisherConfiguration(ILogger<AddUpdatePublisherConfiguration> logger, INatsConnection connection)
    {
        _logger = logger;
        _natsConnection = connection;
    }

    public async Task<IResult> HandleAsync(int _publishRate)
    {

        // TODO: store the value in the NATS KV store
        _logger.LogInformation("Publish rate set to {PublishRate}", _publishRate);
        await UpdatePublishRateInKvStore(_publishRate);
        return Results.Ok($"Publish rate set to {_publishRate}");
    }

    public async Task UpdatePublishRateInKvStore(int _publishRate)
    {
        var kv = _natsConnection.CreateKeyValueStoreContext();
        var bucketName = "publisher-bkt";
        var publisherKvConfig = new NatsKVConfig(bucket: bucketName)
        {
            Description = "Publish rate",
        };
        _logger.LogInformation("Creating or updating the publisher configuration in the KV store");
        var publisher = await kv.CreateOrUpdateStoreAsync(publisherKvConfig);
        _logger.LogInformation("Putting the publish rate in the KV store");
        await publisher.PutAsync<int>("publishRate", _publishRate);
    }
}
