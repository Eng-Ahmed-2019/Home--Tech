using FluentValidation;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;

namespace HomeTech.Application.Validator
{
    public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
    {
        private readonly ILocationRepository _locationRepository;

        public CreateLocationValidator(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;

            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("اسم الموقع مطلوب")
                .MinimumLength(3).WithMessage("يجب ان يكون اسم الموقع اكبر من 3 احرف")
                .MaximumLength(100).WithMessage("يجب ان يكون اسم الموقع اقل من 100 حرف");

            RuleFor(l => l)
                .MustAsync(async (dto, ct) =>
                    !await _locationRepository
                        .ExistsByNameAsync(dto.Name, null, ct))
                .WithMessage("هذا الاسم موجود بالفعل");
        }
    }
}