using FluentValidation;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.Validator
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(l => l.Dto.name)
                .NotEmpty().WithMessage("اسم الفئة مطلوب")
                .MinimumLength(3).WithMessage("يجب ان يكون اسم الفئة اكبر من 3 احرف")
                .MaximumLength(100).WithMessage("يجب ان يكون اسم الفئة اقل من 100 حرف");

            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                    !await _categoryRepository.ExistsByNameAsync(dto.Dto.name.Trim(), null, ct)
                )
                .WithMessage("هذا الاسم مستخدم بالفعل");

            RuleFor(l => l.Dto.isActive)
                .NotNull()
                .WithMessage("حالة الفئة مطلوبة");
        }
    }
}