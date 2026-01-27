using FluentValidation;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;

namespace HomeTech.Application.Validator
{
    public class UpdateLocationValidator : AbstractValidator<UpdateLocationDto>
    {
        private readonly ILocationRepository _locationRepository;

        public UpdateLocationValidator(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;

            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("اسم الموقع مطلوب")
                .MinimumLength(3).WithMessage("يجب ان يكون اسم الموقع اكبر من 3 احرف")
                .MaximumLength(100).WithMessage("يجب ان يكون اسم الموقع اقل من 100 حرف");

            RuleFor(l => l)
                .MustAsync(async (dto, ct) =>
                    !await _locationRepository
                        .ExistsByNameAsync(dto.Name, dto.Id, ct))
                .WithMessage("هذا الاسم موجود بالفعل");

            RuleFor(l => l.IsActive)
                .NotNull()
                .WithMessage("حالة الموقع مطلوبة");
        }
    }
}