using MediatR;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Exceptions;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetProfileQueryHandler :
    IRequestHandler<GetProfileQuery, UserProfileDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetProfileQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileDto> Handle(
            GetProfileQuery query,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(query.UserId.Trim())
                ?? throw new UserNotFoundException("هذا المستخدم غير موجود");

            if (!user.IsActive) throw new UserNotFoundException("الحساب غير مفعل");

            return new UserProfileDto
            {
                FullName = user.FullName,
                Email = user.Email!,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt,
                ImageUrl = user.ProfileImagePath
            };
        }
    }
}