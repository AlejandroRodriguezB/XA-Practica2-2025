using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using StackExchange.Redis;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
    .SetApplicationName("WebApi");

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

// MinIO config
builder.Services.AddSingleton<MinioService>();

var app = builder.Build();

// MinIO create bucket if not exists
using (var scope = app.Services.CreateScope())
{
    var minio = scope.ServiceProvider.GetRequiredService<MinioService>();
    await minio.TryEnsureBucketAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHttpMetrics();

app.MapMetrics("/metrics");

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
