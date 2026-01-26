using MediatR;
using FluentValidation;
using HomeTech.API.Extensions;
using HomeTech.API.Middleware;
using HomeTech.Infrastructure;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Behaviors;
using HomeTech.Application.Validator;
using HomeTech.Application.Interfaces;
using HomeTech.Infrastructure.Services;
using Microsoft.AspNetCore.RateLimiting;
using HomeTech.Application.CQRS.Handlers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<HomeTechDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<HomeTechDbContext>());

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(r =>
{
    r.AddFixedWindowLimiter("login", Limiter =>
    {
        Limiter.Window = TimeSpan.FromMinutes(1);
        Limiter.PermitLimit = 5;
    });
    r.AddFixedWindowLimiter("register", o =>
    {
        o.Window = TimeSpan.FromMinutes(5);
        o.PermitLimit = 3;
    });
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(RegisterUserCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(LoginQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(LogoutCommandHandler).Assembly);
});

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddSingleton<IEmailQueue, EmailQueue>();
builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<IpBanMiddleware>();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await IdentitySeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<SessionRefreshMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();