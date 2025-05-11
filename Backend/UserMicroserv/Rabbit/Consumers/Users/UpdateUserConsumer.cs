using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Users.Requests;
using Rabbit.Users.Responses;

namespace RabbitMQ.Consumers.Users
{
    public class UpdateUserConsumer : IConsumer<UpdateUserRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<UpdateUserConsumer> logger;
        public UpdateUserConsumer(IServiceProvider provider, ILogger<UpdateUserConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateUserRequest> context)
        {
            UpdateUserResponse resposne = new();
            await serviceManager.Users.UpdateUserAsync(context.Message.RequestData);
            await context.RespondAsync(resposne);
        }
    }
}
