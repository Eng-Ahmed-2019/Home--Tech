using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record DeleteCategoryCommand(DeleteCategoryDto Dto) : IRequest<bool>;
}