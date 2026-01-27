using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Common;

namespace HomeTech.Application.CQRS.Queries
{
    public record GetMainLocationsQuery(int Page, int PageSize)
    : IRequest<PagedResult<LocationDto>>;
}