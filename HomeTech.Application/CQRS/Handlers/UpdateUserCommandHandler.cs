using MediatR;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _audit;
        private readonly IHttpContextAccessor _http;
        private readonly IFileStorageService _fileStorage;

        public UpdateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService audit,
            IHttpContextAccessor http,
            IFileStorageService fileStorage)
        {
            _userManager = userManager;
            _audit = audit;
            _http = http;
            _fileStorage = fileStorage;
        }

        public async Task<string> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new UserNotFoundException("المستخدم غير موجود");

            if (!user.IsActive)
                throw new FailedUpdateUserException("لا يمكن تعديل حساب غير مفعل");

            if (request.Dto.formFile != null)
            {
                _fileStorage.Delete(user.ProfileImagePath);

                user.ProfileImagePath =
                    await _fileStorage.SaveAsync(request.Dto.formFile, "users");
            }

            user.FullName = request.Dto.FullName.Trim();
            user.Gender = request.Dto.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new FailedUpdateUserException(errors);
            }

            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: _http.HttpContext?.Items["SessionId"] as Guid?,
                action: "Update Profile",
                reason: "Profile updated successfully",
                ipAddress: _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                cancellationToken: cancellationToken
            );

            return "تم تحديث البيانات بنجاح";
        }
    }
}