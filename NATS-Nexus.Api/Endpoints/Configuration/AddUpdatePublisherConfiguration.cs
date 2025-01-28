namespace NATS_Nexus.Api.Endpoints.Configuration
{
    public class AddUpdatePublisherConfiguration
    {
        private readonly ILogger<AddUpdatePublisherConfiguration> _logger;
        public AddUpdatePublisherConfiguration(ILogger<AddUpdatePublisherConfiguration> logger)
        {
            _logger = logger;
        }

        public async Task<IResult> HandleAsync(int _publishRate)
        {

            // TODO: store the value in the NATS KV store
            _logger.LogInformation("Publish rate set to {PublishRate}", _publishRate);
            return Results.Ok($"Publish rate set to {_publishRate}");
        }
    }
}
