namespace HomeTech.Application.Interfaces
{
    public interface ITokenReplayService
    {
        Task<bool> IsJtiUsedAsync(string jti);
        Task MarkJtiAsUsedAsync(string jti, TimeSpan ttl);
    }
}