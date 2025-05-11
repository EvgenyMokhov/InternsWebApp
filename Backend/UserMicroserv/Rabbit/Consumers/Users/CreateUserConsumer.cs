using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Users.Requests;
using Rabbit.Users.Responses;

namespace RabbitMQ.Consumers.Users
{
    public class CreateUserConsumer : IConsumer<CreateUserRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<CreateUserConsumer> logger;
        public CreateUserConsumer(IServiceProvider provider, ILogger<CreateUserConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateUserRequest> context)
        {
            CreateUserResponse response = new();
            await serviceManager.Users.CreateUserAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
