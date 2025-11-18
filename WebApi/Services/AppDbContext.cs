using Microsoft.EntityFrameworkCore;
using WebApi.Dto;

namespace WebApi.Services
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}
