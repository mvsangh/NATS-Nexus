namespace NATS_Nexus.Api.Endpoints.Configuration
{
    public class AddUpdateConsumerConfiguration
    {
        private readonly ILogger<AddUpdateConsumerConfiguration> _logger;
        public AddUpdateConsumerConfiguration(ILogger<AddUpdateConsumerConfiguration> logger)
        {
            _logger = logger;
        }

        public async Task<IResult> HandleAsync(int _noOfConsumers)
        {

            // TODO: store the value in the NATS KV store
            _logger.LogInformation("No of consumers {NoOfconsumers}", _noOfConsumers);
            return Results.Ok($"No of consumers are :  {_noOfConsumers}");
        }
    }
}
