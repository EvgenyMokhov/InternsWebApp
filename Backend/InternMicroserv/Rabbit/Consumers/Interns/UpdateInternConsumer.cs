using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Consumers.Interns
{
    public class UpdateInternConsumer : IConsumer<UpdateInternRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<UpdateInternConsumer> logger;
        public UpdateInternConsumer(IServiceProvider provider, ILogger<UpdateInternConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateInternRequest> context)
        {
            UpdateInternResponse response = new();
            await serviceManager.Interns.UpdateInternAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
