using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record UpdateLocationCommand(UpdateLocationDto Dto) : IRequest<bool>;
}