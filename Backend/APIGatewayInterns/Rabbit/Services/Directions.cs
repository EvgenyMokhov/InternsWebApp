using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rabbit.Direction;
using Rabbit.Direction.Requests;
using Rabbit.Direction.Responses;

namespace RabbitMQ.Services
{
    public class Directions
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Directions> Logger;
        public Directions(IServiceProvider provider, ILogger<Directions> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task CreateDirectionAsync(DirectionDto direction, Guid transactionId)
        {
            IRequestClient<CreateDirectionRequest> requestClient = Provider.GetRequiredService<IRequestClient<CreateDirectionRequest>>();
            await requestClient.GetResponse<CreateDirectionResponse>(new() { RequestData = direction, TransactionId = transactionId });
        }

        public async Task UpdateDirectionAsync(DirectionDto direction, Guid transactionId)
        {
            IRequestClient<UpdateDirectionRequest> requestClient = Provider.GetRequiredService<IRequestClient<UpdateDirectionRequest>>();
            await requestClient.GetResponse<UpdateDirectionResponse>(new() { RequestData = direction, TransactionId = transactionId });
        }

        public async Task<DirectionDto> GetDirectionAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetDirectionRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetDirectionRequest>>();
            try
            {
                return (await requestClient.GetResponse<GetDirectionResponse>(new() { Id = id, TransactionId = transactionId })).Message.ResponseData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.Split(':')[1]);
            }
        }

        public async Task<List<DirectionDto>> GetAllDirectionsAsync(Guid transactionId)
        {
            IRequestClient<GetAllDirectionsRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllDirectionsRequest>>();
            return (await requestClient.GetResponse<GetAllDirectionsResponse>(new() { TransactionId = transactionId })).Message.ResponseData;
        }
    }
}
