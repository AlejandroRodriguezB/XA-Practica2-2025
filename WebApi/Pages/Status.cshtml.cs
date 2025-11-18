using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System.Text.Json;
using WebApi.Services;

namespace WebApi.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class StatusModel(ILogger<StatusModel> logger, AppDbContext context, IConnectionMultiplexer? redis = null) : PageModel
    {
        public string StatusJson { get; set; } = string.Empty;

        private readonly ILogger<StatusModel> _logger = logger;
        private readonly AppDbContext _context = context;
        private readonly IConnectionMultiplexer? _redis = redis;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public async Task OnGetAsync()
        {
            try
            {
                var dbOk = await _context.Database.CanConnectAsync();
                var cacheOk = _redis != null && _redis.IsConnected; // TODO: Don not show cacheOk redis if env is development

                var statusObj = new
                {
                    web = new { ok = true },
                    database = new { ok = dbOk },
                    cache = new { ok = cacheOk }
                };

                StatusJson = JsonSerializer.Serialize(statusObj, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking status");
            }
        }
    }
}
