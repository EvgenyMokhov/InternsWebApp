using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Consumers.Direction
{
    public class GetAllDirectionsConsumer : IConsumer<GetAllDirectionsRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetAllDirectionsConsumer> logger;
        public GetAllDirectionsConsumer(IServiceProvider provider, ILogger<GetAllDirectionsConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetAllDirectionsRequest> context)
        {
            GetAllDirectionsResponse response = new() { ResponseData = await serviceManager.Directions.GetAllDirectionAsync() };
            await context.RespondAsync(response);
        }
    }
}
