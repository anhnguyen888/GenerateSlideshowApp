using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult GenerateSlideshow()
    {
        try
        {
            var wwwrootPath = _webHostEnvironment.WebRootPath;
            var sourceImagePath = Path.Combine(wwwrootPath, "images");
            var generatedAppPath = Path.Combine(wwwrootPath, "GeneratedApp");
            var generatedImagesPath = Path.Combine(generatedAppPath, "images");

            // Create directories
            Directory.CreateDirectory(generatedAppPath);
            Directory.CreateDirectory(generatedImagesPath);

            // Get and copy image files
            var imageFiles = Directory.GetFiles(sourceImagePath)
                .Where(f => IsImageFile(f))
                .OrderBy(f => f)
                .ToList();

            var imageFileNames = new List<string>();
            foreach (var imageFile in imageFiles)
            {
                var fileName = Path.GetFileName(imageFile);
                imageFileNames.Add(fileName);
                var destinationPath = Path.Combine(generatedImagesPath, fileName);
                System.IO.File.Copy(imageFile, destinationPath, true);
            }

            // Generate files
            GenerateHtmlFile(generatedAppPath);
            GenerateCssFile(generatedAppPath);
            GenerateJsFile(generatedAppPath, imageFileNames);

            return Json(new { success = true, message = "Slideshow generated successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    private bool IsImageFile(string filePath)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        return validExtensions.Contains(Path.GetExtension(filePath).ToLower());
    }

    private void GenerateHtmlFile(string basePath)
    {
        var html = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Image Slideshow</title>
    <link rel='stylesheet' href='style.css'>
</head>
<body>
    <div class='slideshow-container'>
        <div id='slideshow'></div>
        <button id='prevButton' class='nav-button'>&lt;</button>
        <button id='nextButton' class='nav-button'>&gt;</button>
    </div>
    <script src='script.js'></script>
</body>
</html>";

        System.IO.File.WriteAllText(Path.Combine(basePath, "index.html"), html.TrimStart());
    }

    private void GenerateCssFile(string basePath)
    {
        var css = @"
body {
    margin: 0;
    padding: 0;
    background: #000;
    overflow: hidden;
}

.slideshow-container {
    position: relative;
    width: 100vw;
    height: 100vh;
}

#slideshow {
    width: 100%;
    height: 100%;
}

#slideshow img {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    object-fit: contain;
    opacity: 0;
    transition: opacity 0.5s ease-in-out;
}

#slideshow img.active {
    opacity: 1;
}

.nav-button {
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    background: rgba(255, 255, 255, 0.3);
    border: none;
    color: white;
    padding: 15px 20px;
    cursor: pointer;
    font-size: 24px;
    transition: background 0.3s;
    z-index: 1000;
}

.nav-button:hover {
    background: rgba(255, 255, 255, 0.5);
}

#prevButton {
    left: 20px;
}

#nextButton {
    right: 20px;
}";

        System.IO.File.WriteAllText(Path.Combine(basePath, "style.css"), css.TrimStart());
    }

    private void GenerateJsFile(string basePath, List<string> imageFiles)
    {
        var imagesArrayJson = "[" + string.Join(",", imageFiles.Select(f => $"'{f}'")) + "]";

        var js = $@"
// Hardcoded array of image filenames
const images = {imagesArrayJson};
let currentSlide = 0;

document.addEventListener('DOMContentLoaded', function() {{
    const slideshow = document.getElementById('slideshow');
    
    // Create image elements
    images.forEach((image, index) => {{
        const img = document.createElement('img');
        img.src = `images/${{image}}`;
        img.className = index === 0 ? 'active' : '';
        slideshow.appendChild(img);
    }});

    // Setup navigation
    document.getElementById('prevButton').addEventListener('click', showPreviousSlide);
    document.getElementById('nextButton').addEventListener('click', showNextSlide);
    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {{
        if (e.key === 'ArrowLeft') showPreviousSlide();
        if (e.key === 'ArrowRight') showNextSlide();
    }});
}});

function showPreviousSlide() {{
    const slides = document.querySelectorAll('#slideshow img');
    slides[currentSlide].className = '';
    currentSlide = (currentSlide - 1 + slides.length) % slides.length;
    slides[currentSlide].className = 'active';
}}

function showNextSlide() {{
    const slides = document.querySelectorAll('#slideshow img');
    slides[currentSlide].className = '';
    currentSlide = (currentSlide + 1) % slides.length;
    slides[currentSlide].className = 'active';
}}";

        System.IO.File.WriteAllText(Path.Combine(basePath, "script.js"), js.TrimStart());
    }
}