// Hardcoded array of image filenames
const images = ['1.png','2.jpg'];
let currentSlide = 0;

document.addEventListener('DOMContentLoaded', function() {
    const slideshow = document.getElementById('slideshow');
    
    // Create image elements
    images.forEach((image, index) => {
        const img = document.createElement('img');
        img.src = `images/${image}`;
        img.className = index === 0 ? 'active' : '';
        slideshow.appendChild(img);
    });

    // Setup navigation
    document.getElementById('prevButton').addEventListener('click', showPreviousSlide);
    document.getElementById('nextButton').addEventListener('click', showNextSlide);
    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        if (e.key === 'ArrowLeft') showPreviousSlide();
        if (e.key === 'ArrowRight') showNextSlide();
    });
});

function showPreviousSlide() {
    const slides = document.querySelectorAll('#slideshow img');
    slides[currentSlide].className = '';
    currentSlide = (currentSlide - 1 + slides.length) % slides.length;
    slides[currentSlide].className = 'active';
}

function showNextSlide() {
    const slides = document.querySelectorAll('#slideshow img');
    slides[currentSlide].className = '';
    currentSlide = (currentSlide + 1) % slides.length;
    slides[currentSlide].className = 'active';
}