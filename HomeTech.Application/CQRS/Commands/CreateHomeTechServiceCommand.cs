using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{

    public record CreateHomeTechServiceCommand(CreateServiceDto Dto) : IRequest<string>;
}