using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Common;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetSubLocationsQueryHandler
        : IRequestHandler<GetSubLocationsQuery, PagedResult<LocationDto>>
    {
        private readonly ILocationRepository _repository;

        public GetSubLocationsQueryHandler(ILocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<LocationDto>> Handle(
            GetSubLocationsQuery request,
            CancellationToken ct)
        {
            var page = Math.Max(request.Page, 1);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var result = await _repository.GetSubAsync(
                request.ParentId,
                page,
                pageSize,
                ct);

            return new PagedResult<LocationDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                Items = result.Items
                    .Select(x => new LocationDto(x.Id, x.Name))
                    .ToList()
            };
        }
    }
}