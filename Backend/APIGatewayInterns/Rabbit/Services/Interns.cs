using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rabbit.Interns;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Services
{
    public class Interns
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Interns> Logger;
        public Interns(IServiceProvider provider, ILogger<Interns> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task CreateInternAsync(InternDto intern, Guid transactionId)
        {
            IRequestClient<CreateInternRequest> requestClient = Provider.GetRequiredService<IRequestClient<CreateInternRequest>>();
            await requestClient.GetResponse<CreateInternResponse>(new() { RequestData = intern, TransactionId = transactionId });
        }

        public async Task UpdateInternAsync(InternDto intern, Guid transactionId)
        {
            IRequestClient<UpdateInternRequest> requestClient = Provider.GetRequiredService<IRequestClient<UpdateInternRequest>>();
            await requestClient.GetResponse<UpdateInternResponse>(new() { RequestData = intern, TransactionId = transactionId });
        }

        public async Task<List<InternDto>> GetAllInternsAsync(Guid transactionId)
        {
            IRequestClient<GetAllInternsRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllInternsRequest>>();
            return (await requestClient.GetResponse<GetAllInternsResponse>(new() { TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task<InternDto> GetInternAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetInternRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetInternRequest>>();
            return (await requestClient.GetResponse<GetInternResponse>(new() { Id = id, TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task<List<InternDto>> GetInternsByDirection(Guid directionId, Guid transactionId)
        {
            IRequestClient<GetInternsByDirectionRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetInternsByDirectionRequest>>();
            return (await requestClient.GetResponse<GetInternsByDirectionResponse>(new() { DirectionId = directionId, TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task<List<InternDto>> GetInternsByProject(Guid projectId, Guid transactionId)
        {
            using IServiceScope scope = Provider.CreateScope();
            IRequestClient<GetInternsByProjectRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetInternsByProjectRequest>>();
            return (await requestClient.GetResponse<GetInternsByProjectResponse>(new() { ProjectId = projectId, TransactionId = transactionId })).Message.ResponseData;
        }
    }
}
