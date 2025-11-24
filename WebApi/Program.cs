using Microsoft.EntityFrameworkCore;
using Prometheus;
using StackExchange.Redis;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Postgres config
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Redis config
var redisUrl = builder.Configuration["Redis:Connection"];
if (!string.IsNullOrEmpty(redisUrl))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(redisUrl));
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHttpMetrics();

app.MapMetrics("/metrics");

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
