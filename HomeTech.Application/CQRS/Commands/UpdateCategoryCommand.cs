using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record UpdateCategoryCommand(UpdateCategoryDto Dto) : IRequest<bool>;
}