using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Queries
{
    public record GetAllLocationsQuery() : IRequest<List<LocationDto>>;
}