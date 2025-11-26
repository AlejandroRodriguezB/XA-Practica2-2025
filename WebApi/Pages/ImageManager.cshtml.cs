using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApi.Services;

namespace WebApi.Pages
{
    public class ImageManagerModel(MinioService minio) : PageModel
    {
        private readonly MinioService _minio = minio;
        public bool MinioAvailable { get; set; }

        public void OnGet()
        {
            MinioAvailable = _minio.Enabled;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!_minio.Enabled)
            {
                ModelState.AddModelError("", "MinIO no está disponible.");
                return Page();
            }

            using var stream = file.OpenReadStream();

            await _minio.PutObjectAsync(stream, file);

            return RedirectToPage();
        }
    }
}
