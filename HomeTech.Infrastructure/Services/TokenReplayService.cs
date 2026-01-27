using StackExchange.Redis;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class TokenReplayService : ITokenReplayService
    {
        private readonly IDatabase _redis;

        public TokenReplayService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<bool> IsJtiUsedAsync(string jti)
            => await _redis.KeyExistsAsync($"jti:{jti}");

        public async Task MarkJtiAsUsedAsync(string jti, TimeSpan ttl)
            => await _redis.StringSetAsync($"jti:{jti}", "1", ttl);
    }
}