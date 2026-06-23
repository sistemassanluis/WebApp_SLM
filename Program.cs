using QuestPDF.Infrastructure;
using WebApp_SLM.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITiempoExtraRepository, TiempoExtraRepository>();
builder.Services.AddTransient<ITablasRepository, TablasRepository>();
builder.Services.AddTransient<IGeneraPdfService, GeneraPdfService>();
var app = builder.Build();
QuestPDF.Settings.License = LicenseType.Community;
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
