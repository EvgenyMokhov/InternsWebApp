using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Consumers.Direction
{
    public class GetDirectionConsumer : IConsumer<GetDirectionRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetDirectionConsumer> logger;
        public GetDirectionConsumer(IServiceProvider provider, ILogger<GetDirectionConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetDirectionRequest> context)
        {
            GetDirectionResponse response = new() { ResponseData = await serviceManager.Directions.GetDirectionAsync(context.Message.Id) };
            await context.RespondAsync(response);
        }
    }
}
