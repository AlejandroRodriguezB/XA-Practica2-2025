using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, AppDbContext context, IConnectionMultiplexer? redis = null) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly AppDbContext _context = context;
        private readonly IConnectionMultiplexer? _redis = redis;

        public List<Product> Products { get; set; } = [];

        [BindProperty]
        public Product NewProduct { get; set; } = new();

        private const string CacheKey = "products:list";

        public async Task OnGetAsync()
        {
            try
            {
                if (_redis is not null)
                {
                    var db = _redis.GetDatabase();
                    var cachedJson = await db.StringGetAsync(CacheKey);
                    if (cachedJson.HasValue)
                    {
                        Products = JsonSerializer.Deserialize<List<Product>>(cachedJson.ToString()) ?? [];
                        return;
                    }
                }
            
                Products = [.. _context.Products.OrderBy(p => p.Id)];

                if (_redis is not null)
                {
                    var db = _redis.GetDatabase();
                    var json = JsonSerializer.Serialize(Products);
                    await db.StringSetAsync(
                        CacheKey,
                        json,
                        TimeSpan.FromMinutes(5));
                }

            } catch(Exception ex) {
                _logger.LogError(ex, "Error getting index");
            }
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                _context.Products.Add(NewProduct);
                await _context.SaveChangesAsync();

                if (_redis is not null)
                {
                    var db = _redis.GetDatabase();
                    await db.KeyDeleteAsync(CacheKey);
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError(string.Empty, "Unable to create product. Try again later.");
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    if (_redis is not null)
                    {
                        var db = _redis.GetDatabase();
                        await db.KeyDeleteAsync(CacheKey);
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product id={Id}", id);
                ModelState.AddModelError(string.Empty, "Unable to delete product. Try again later.");
                await OnGetAsync();
                return Page();
            }
        }
    }
}
