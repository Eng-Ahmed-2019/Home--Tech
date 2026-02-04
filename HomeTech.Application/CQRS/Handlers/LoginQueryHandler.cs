using MediatR;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

public class LoginQueryHandler
    : IRequestHandler<LoginQuery, LoginResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditService _audit;

    public LoginQueryHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IHttpContextAccessor httpContextAccessor,
        IAuditService audit)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _audit = audit;
    }

    public async Task<LoginResponseDto> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(query.Dto.Email.Trim());

        if (user is null)
            throw new unAuthorizedException("فشل عملية الدخول, برجاء المحاولة مرة اخري");

        if (!user.EmailConfirmed)
            throw new ForbiddenException("لم يتم تاكيد البريد بعد, برجاء تاكيد البريد");

        if (!user.IsActive)
            throw new ForbiddenException("هذا المستخدم غير نشط");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            query.Dto.Password.Trim(),
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: null,
                action: "Failed Login Attempt",
                reason: "Invalid credentials provided",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!
                    .ToString(),
                cancellationToken: cancellationToken
                );
            throw new unAuthorizedException("فشل عملية الدخول, برجاء المحاولة مرة اخري");
        }

        var ip = _httpContextAccessor.HttpContext!
                .Connection.RemoteIpAddress!
                .ToString();

        await _audit.LogActionAsync(
            userId: user.Id,
            sessionId: null,
            action: "User Login",
            reason: "User logged in successfully",
            ipAddress: ip,
            cancellationToken: cancellationToken
            );

        return await _jwtService.GenerateTokenAsync(user, ip);
    }
}