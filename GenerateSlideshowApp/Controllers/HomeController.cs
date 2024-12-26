using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace SlideshowApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult GenerateApp()
        {
            string appPath = Path.Combine(_env.WebRootPath, "GeneratedApp");
            string imagesPath = Path.Combine(appPath, "images");

            // Create necessary directories
            Directory.CreateDirectory(appPath);
            Directory.CreateDirectory(imagesPath);

            // Copy images to the new folder
            string sourceImagesPath = Path.Combine(_env.WebRootPath, "images");
            string[] imageFiles = Directory.GetFiles(sourceImagesPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png"))
                .ToArray();

            foreach (var imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                string destFile = Path.Combine(imagesPath, fileName);
                System.IO.File.Copy(imageFile, destFile, true);
            }

            // Generate the HTML file
            string htmlContent = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Slideshow</title>
    <style>
        body { margin: 0; background: #000; color: #fff; display: flex; align-items: center; justify-content: center; height: 100vh; }
        img { max-width: 100%; max-height: 100%; display: none; }
        img.active { display: block; }
    </style>
</head>
<body>
    <script>
        const images = [PLACEHOLDER_IMAGES];
        let index = 0;

        function showImage(idx) {
            document.querySelectorAll('img').forEach((img, i) => {
                img.classList.toggle('active', i === idx);
            });
        }

        function nextImage() {
            index = (index + 1) % images.length;
            showImage(index);
        }

        function prevImage() {
            index = (index - 1 + images.length) % images.length;
            showImage(index);
        }

        window.addEventListener('keydown', (e) => {
            if (e.key === 'ArrowRight') nextImage();
            if (e.key === 'ArrowLeft') prevImage();
        });

        document.addEventListener('DOMContentLoaded', () => {
            const container = document.body;
            images.forEach(src => {
                const img = document.createElement('img');
                img.src = src;
                container.appendChild(img);
            });
            showImage(index);
        });
    </script>
</body>
</html>";
            // Replace the placeholder with image paths
            string[] imagePaths = imageFiles.Select(file => $"'images/{Path.GetFileName(file)}'").ToArray();
            htmlContent = htmlContent.Replace("PLACEHOLDER_IMAGES", string.Join(",", imagePaths));

            // Write the HTML file
            System.IO.File.WriteAllText(Path.Combine(appPath, "index.html"), htmlContent);

            return Content($"Slideshow application generated successfully. Navigate to the <a href='/GeneratedApp/index.html'>Generated Slideshow</a>.", "text/html");
        }
    }
}
