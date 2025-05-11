using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Consumers.Direction
{
    public class DeleteDirectionConsumer : IConsumer<DeleteDirectionRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<DeleteDirectionConsumer> logger;
        public DeleteDirectionConsumer(IServiceProvider provider, ILogger<DeleteDirectionConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<DeleteDirectionRequest> context)
        {
            DeleteDirectionResponse response = new();
            await serviceManager.Directions.DeleteDirectionAsync(context.Message.Id);
            await context.RespondAsync(response);
        }
    }
}
