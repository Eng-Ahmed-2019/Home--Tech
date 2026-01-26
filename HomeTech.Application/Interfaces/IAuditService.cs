namespace HomeTech.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogActionAsync(
            string? userId,
            Guid? sessionId,
            string action,
            string reason,
            string ipAddress,
            CancellationToken cancellationToken = default);
    }
}