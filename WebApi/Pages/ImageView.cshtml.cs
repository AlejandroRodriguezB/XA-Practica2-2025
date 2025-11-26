using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Minio.DataModel.Args;
using System.Security.AccessControl;
using WebApi.Services;

namespace WebApi.Pages
{
    public class ImageViewModel(MinioService minio) : PageModel
    {
        private readonly MinioService _minio = minio;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_minio.Enabled)
                return NotFound();

            var img = await _minio.GetObjectAsync();

            if (img == null)
                return NotFound();

            return File(img.FileContents, img.ContentType);
        }
    }
}
