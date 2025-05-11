using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Consumers.Direction
{
    public class CreateDirectionConsumer : IConsumer<CreateDirectionRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<CreateDirectionConsumer> logger;
        public CreateDirectionConsumer(IServiceProvider provider, ILogger<CreateDirectionConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<CreateDirectionRequest> context)
        {
            CreateDirectionResponse response = new();
            await serviceManager.Directions.CreateDirectionAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
