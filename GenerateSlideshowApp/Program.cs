var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Create necessary directories
var webRootPath = app.Environment.WebRootPath;
Directory.CreateDirectory(Path.Combine(webRootPath, "images"));
Directory.CreateDirectory(Path.Combine(webRootPath, "slideshows"));
app.Run();
