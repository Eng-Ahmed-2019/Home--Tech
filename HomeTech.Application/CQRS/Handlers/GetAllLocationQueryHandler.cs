using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetAllLocationQueryHandler
        : IRequestHandler<GetAllLocationsQuery, List<LocationDto>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetAllLocationQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationDto>> Handle(
            GetAllLocationsQuery query,
            CancellationToken ct)
        {
            var result = await _locationRepository
                .GetAllActiveAsync(ct);

            return result.Select(x => new LocationDto(x.Id, x.Name)).ToList();
        }
    }
}