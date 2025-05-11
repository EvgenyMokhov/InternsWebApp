using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Consumers.Interns
{
    public class GetInternsByDirectionConsumer : IConsumer<GetInternsByDirectionRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetInternsByDirectionConsumer> logger;
        public GetInternsByDirectionConsumer(IServiceProvider provider, ILogger<GetInternsByDirectionConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetInternsByDirectionRequest> context)
        {
            GetInternsByDirectionResponse response = new() { ResponseData = await serviceManager.Interns.GetInternsByDirectionAsync(context.Message.DirectionId) };
            await context.RespondAsync(response);
        }
    }
}
