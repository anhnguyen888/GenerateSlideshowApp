using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GenerateSlideshowApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetImages()
        {
            try
            {
                string imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!Directory.Exists(imagesPath))
                {
                    return new JsonResult(Array.Empty<string>());
                }

                string[] supportedExtensions = { ".jpg", ".jpeg", ".png" };

                var imageFiles = Directory.GetFiles(imagesPath)
                    .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .OrderBy(f => f)
                    .Select(f => $"/images/{Path.GetFileName(f)}")
                    .ToArray();

                return new JsonResult(imageFiles);
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to retrieve images");
            }
        }
    }
}
