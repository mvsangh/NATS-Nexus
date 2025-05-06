namespace NATS_Nexus.Api.Endpoints.Configuration;

public class AddUpdateConsumerConfiguration
{
    private readonly ILogger<AddUpdateConsumerConfiguration> _logger;
    private readonly INatsConnection _natsConnection;
    public AddUpdateConsumerConfiguration(ILogger<AddUpdateConsumerConfiguration> logger, INatsConnection connection)
    {
        _logger = logger;
        _natsConnection = connection;
    }

    public async Task<IResult> HandleAsync(int _noOfConsumers)
    {

        // TODO: store the value in the NATS KV store
        _logger.LogInformation("No of consumers {NoOfconsumers}", _noOfConsumers);
        await UpdateNoOfConsumersInKvStore(_noOfConsumers);
        return Results.Ok($"No of consumers are :  {_noOfConsumers}");
    }

    public async Task UpdateNoOfConsumersInKvStore(int _noOfConsumers)
    {
        var kv = _natsConnection.CreateKeyValueStoreContext();

        var bucketName = "consumer_bkt";
        var consumerKvConfig = new NatsKVConfig(bucket: bucketName)
        {
            Description = "No of consumers",
        };

        _logger.LogInformation("Creating or updating the consumer configuration in the KV store");
        var consumer = await kv.CreateOrUpdateStoreAsync(consumerKvConfig);

        _logger.LogInformation("Putting the no of consumers in the KV store");
        await consumer.PutAsync<int>("no_of_consumers", _noOfConsumers);
    }

}
