using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record UpdateHomeTechServicesCommand(UpdateServiceDto Dto) : IRequest<bool>;
}