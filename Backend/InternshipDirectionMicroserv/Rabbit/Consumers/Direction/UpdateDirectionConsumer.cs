using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Consumers.Direction
{
    public class UpdateDirectionConsumer : IConsumer<UpdateDirectionRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<UpdateDirectionConsumer> logger;
        public UpdateDirectionConsumer(IServiceProvider provider, ILogger<UpdateDirectionConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateDirectionRequest> context)
        {
            UpdateDirectionResponse response = new();
            await serviceManager.Directions.UpdateDirectionAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
