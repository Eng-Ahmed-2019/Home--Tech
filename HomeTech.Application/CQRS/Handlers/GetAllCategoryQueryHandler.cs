using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetAllCategoryQueryHandler :
        IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> Handle(
            GetAllCategoriesQuery query, CancellationToken cancellationToken
        )
        {
            var result = await _categoryRepository.GetAllAsync(cancellationToken);

            return result.Select(r => new CategoryDto(r.Id, r.Name.Trim())).ToList();
        }
    }
}