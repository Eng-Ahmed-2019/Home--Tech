using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<string>;
}