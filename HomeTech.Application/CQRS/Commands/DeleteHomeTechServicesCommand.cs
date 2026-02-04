using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record DeleteHomeTechServicesCommand(DeleteServiceDto dto) : IRequest<bool>;
}