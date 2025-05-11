using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Consumers.Interns
{
    public class CreateInternConsumer : IConsumer<CreateInternRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<CreateInternConsumer> logger;
        public CreateInternConsumer(IServiceProvider provider, ILogger<CreateInternConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateInternRequest> context)
        {
            CreateInternResponse response = new();
            await serviceManager.Interns.CreateInternAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
