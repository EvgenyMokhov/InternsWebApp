using MassTransit.Initializers;
using System.Text.RegularExpressions;
using HttpDtos;
using Rabbit.Direction;
using RabbitMQ;
using Other.Enums;
using Rabbit.Interns;

namespace BusinessLogic.Services
{
    public class Directions
    {
        private readonly RabbitManager Rabbit;
        private readonly List<Action> RollbackList = new();
        public Directions(RabbitManager rabbit)
        {
            Rabbit = rabbit;
        }

        public async Task<IEnumerable<DirectionHttpDto>> GetDirectionsAsync(Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            Dictionary<Guid, DirectionHttpDto> directions = (await Rabbit.Directions.GetAllDirectionsAsync(transactionId))
                .Select(CreateEmptyDirectionHttpDto)
                .ToDictionary(dir => dir.Id);
            List<InternLowDetailHttpDto> interns = await internsService.GetLowDetailInternsAsync(transactionId);
            foreach (InternLowDetailHttpDto intern in interns)
                if (directions.ContainsKey(intern.DirectionId))
                    directions[intern.DirectionId].Interns.Add(intern);
            return directions.Values.Where(dir => dir.IsActive);
        }

        public async Task<List<DirectionLowDetailHttpDto>> GetDirectionsLowDetailAsync(Guid transactionId)
        {
            List<DirectionDto> directions = await Rabbit.Directions.GetAllDirectionsAsync(transactionId);
            return directions.Select(CreateDirectionLowDetailHttpDto).Where(direction => direction.IsActive).ToList();
        }

        public async Task<DirectionHttpDto> GetDirectionAsync(Guid directionId, Guid transactionId)
        {
            DirectionDto direction = await Rabbit.Directions.GetDirectionAsync(directionId, transactionId);
            return await CreateDirectionHttpDto(direction, transactionId);
        }

        public async Task<Guid> CreateDirectionAsync(DirectionLowDetailHttpDto direction, Guid transactionId)
        {
            direction.Id = Guid.NewGuid();
            await Rabbit.Directions.CreateDirectionAsync(CreateDirectionDto(direction), transactionId);
            return direction.Id;
        }

        public async Task<GetPagedDirectionsHttpResponseDto> GetPagedDirectionsAsync(GetPagedDirectionsHttpRequestDto requestData, Guid transactionId)
        {
            List<DirectionHttpDto> directions = (await GetDirectionsAsync(transactionId))
                .Where(dir => Regex.IsMatch(dir.Name, requestData.DirectionsFilter))
                .ToList();
            if (requestData.SortingParameter == SortingParameter.InternCount)
                directions = directions.OrderByDescending(dir => dir.Interns.Count).ToList();
            else if (requestData.SortingParameter == SortingParameter.DirectionName)
                directions = directions.OrderByDescending(dir => dir.Name).ToList();
            GetPagedDirectionsHttpResponseDto response = new() { Directions = new(), TotalCount = directions.Count };
            for (int i = ((requestData.PageNumber - 1) * requestData.DirectionsCountOnPage); (i < requestData.PageNumber * requestData.DirectionsCountOnPage) && i < directions.Count; i++)
                response.Directions.Add(directions[i]);
            return response;
        }
        public async Task UpdateDirectionAsync(DirectionHttpDto direction, Guid transactionId)
        {
            HashSet<Guid> oldInternsIdsSet = (await Rabbit.Interns.GetInternsByDirection(direction.Id, transactionId))
                .Select(intern => intern.Id)
                .ToHashSet();
            try
            {
                foreach (InternLowDetailHttpDto newIntern in direction.Interns)
                    if (!oldInternsIdsSet.Contains(newIntern.Id))
                    {
                        InternDto internDto = await Rabbit.Interns.GetInternAsync(newIntern.Id, transactionId);
                        InternDto rollbackData = new()
                        {
                            Id = internDto.Id,
                            InternshipDirectionId = internDto.InternshipDirectionId,
                            ProjectId = internDto.ProjectId,
                            UserId = internDto.UserId
                        };
                        internDto.InternshipDirectionId = direction.Id;
                        await Rabbit.Interns.UpdateInternAsync(internDto, transactionId);
                        RollbackList.Add(async () => await Rabbit.Interns.UpdateInternAsync(rollbackData, transactionId));
                    }
                await Rabbit.Directions.UpdateDirectionAsync(CreateDirectionDto(direction), transactionId);
            }
            catch (Exception ex) 
            {
                foreach (Action rollbackFunction in RollbackList)
                    rollbackFunction();
                throw new(ex.Message);
            }
        }

        private async Task<DirectionHttpDto> CreateDirectionHttpDto(DirectionDto direction, Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Interns = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByDirection(direction.Id, transactionId))
                    .Select(async intern => await internsService.CreateInternLowDetailHttpDto(intern, transactionId))))
                    .ToList(),
                Name = direction.Name,
                IsActive = direction.IsActive,
            };
        }
        private DirectionHttpDto CreateEmptyDirectionHttpDto(DirectionDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Interns = new(),
                Name = direction.Name,
                IsActive= direction.IsActive
            };
        }
        public DirectionLowDetailHttpDto CreateDirectionLowDetailHttpDto(DirectionDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Name = direction.Name,
                Description = direction.Description,
                IsActive = direction.IsActive
            };
        }

        public DirectionDto CreateDirectionDto(DirectionLowDetailHttpDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Name = direction.Name,
                IsActive = direction.IsActive
            };
        }

        public DirectionDto CreateDirectionDto(DirectionHttpDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Name = direction.Name,
                IsActive = direction.IsActive
            };
        }
    }
}
