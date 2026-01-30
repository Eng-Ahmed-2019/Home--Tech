using FluentValidation;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.Validator
{
    public class UpdateLocationValidator : AbstractValidator<UpdateLocationCommand>
    {
        private readonly ILocationRepository _locationRepository;

        public UpdateLocationValidator(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;

            RuleFor(l => l.Dto.Name)
                .NotEmpty().WithMessage("اسم الموقع مطلوب")
                .MinimumLength(3).WithMessage("يجب ان يكون اسم الموقع اكبر من 3 احرف")
                .MaximumLength(100).WithMessage("يجب ان يكون اسم الموقع اقل من 100 حرف");

            RuleFor(l => l)
                .MustAsync(async (dto, ct) =>
                    !await _locationRepository
                        .ExistsByNameAsync(dto.Dto.Name, dto.Dto.Id, ct))
                .WithMessage("هذا الاسم موجود بالفعل");

            RuleFor(l => l.Dto.IsActive)
                .NotNull()
                .WithMessage("حالة الموقع مطلوبة");
        }
    }
}