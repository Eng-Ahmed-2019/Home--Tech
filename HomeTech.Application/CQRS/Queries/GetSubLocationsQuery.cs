using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Common;

namespace HomeTech.Application.CQRS.Queries
{
    public record GetSubLocationsQuery(int ParentId, int Page, int PageSize)
    : IRequest<PagedResult<LocationDto>>;
}