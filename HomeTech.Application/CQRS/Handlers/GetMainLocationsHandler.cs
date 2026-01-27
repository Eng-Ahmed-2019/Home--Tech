using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Common;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetMainLocationsHandler
    : IRequestHandler<GetMainLocationsQuery, PagedResult<LocationDto>>
    {
        private readonly ILocationRepository _repo;

        public GetMainLocationsHandler(ILocationRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<LocationDto>> Handle(
            GetMainLocationsQuery q, CancellationToken ct)
        {
            var page = Math.Max(q.Page, 1);
            var pageSize = Math.Clamp(q.PageSize, 1, 100);

            var result = await _repo.GetMainAsync(page, pageSize, ct);

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