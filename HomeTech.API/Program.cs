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
// Add services to the container...
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
    r.AddFixedWindowLimiter("admin", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 30;
    });
    r.AddFixedWindowLimiter("email", o =>
    {
        o.Window = TimeSpan.FromMinutes(10);
        o.PermitLimit = 3;
    });
    r.AddFixedWindowLimiter("profile", o =>
    {
        o.Window = TimeSpan.FromMinutes(10);
        o.PermitLimit = 3;
    });
    r.AddFixedWindowLimiter("delete", o =>
    {
        o.Window = TimeSpan.FromMinutes(10);
        o.PermitLimit = 3;
    });
    r.AddFixedWindowLimiter("services", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 30;
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
builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateLocationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateHomeTechServiceValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateHomeTechServicesValidator>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(RegisterUserCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(LoginQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(LogoutCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(CreateLocationCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(UpdateLocationCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(DeleteLocationCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetAllLocationQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetMainLocationsHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetSubLocationsQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(CreateCategoryCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(UpdateCategoryCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(DeleteCategoryCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetAllCategoryQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(UpdateUserCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(DeleteAccountCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(ConfirmEmailCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(ChangeEmailCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(ReactivateAccountCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(ChangePasswordCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(CreateHomeTechServicesCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(UpdateHomeTechServiceCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(DeleteHomeTechServicesCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetAllHomeTechServicesQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(GetProfileQueryHandler).Assembly);
});

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddSingleton<IEmailQueue, EmailQueue>();
builder.Services.AddSingleton<AuditQueue>();
builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddHostedService<AuditBackgroundService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ITokenReplayService, TokenReplayService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IHomeTechServiceRepository, HomeTechServiceRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

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
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<SessionRefreshMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();