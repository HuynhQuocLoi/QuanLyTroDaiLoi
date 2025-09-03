using Microsoft.EntityFrameworkCore;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using QuestPDF.Infrastructure; // 👈 Thêm using này

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Cấu hình License cho QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Razor Pages
builder.Services.AddRazorPages();

// Lấy connection string từ Railway DATABASE_URL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                  ?? "postgresql://postgres:PRyQzqSjQqPAyIslcwLpsELOYPAMOcTH@hopper.proxy.rlwy.net:10473/railway";

if (string.IsNullOrWhiteSpace(databaseUrl))
    throw new InvalidOperationException("DATABASE_URL chưa được cấu hình.");

var databaseUri = new Uri(databaseUrl);
var userInfo = databaseUri.UserInfo.Split(':');

var pgConnectionString =
    $"Host={databaseUri.Host};" +
    $"Port={databaseUri.Port};" +
    $"Username={userInfo[0]};" +
    $"Password={userInfo[1]};" +
    $"Database={databaseUri.AbsolutePath.TrimStart('/')};" +
    $"Pooling=true;" +
    $"SSL Mode=Require;" +
    $"Trust Server Certificate=True;";

builder.Services.AddDbContext<TroDbContext>(options =>
    options.UseNpgsql(pgConnectionString));

var app = builder.Build();

// Áp dụng migration tự động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TroDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("Migration PostgreSQL thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration lỗi: " + ex.Message);
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // phục vụ CSS/JS trong wwwroot
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
