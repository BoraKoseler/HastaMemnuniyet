using HastaMemnuniyet.Application;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure;
using HastaMemnuniyet.Infrastructure.Data;
using HastaMemnuniyet.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

// PostgreSQL DateTime uyumluluğu: timezone-unaware DateTime değerlerini kabul et.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Katman servislerini kaydet (Clean Architecture — DIP).
builder.Services.InfrastructureKatmaniniEkle(builder.Configuration);
builder.Services.ApplicationKatmaniniEkle();

// Kimlik doğrulama çerez yollarını yapılandır.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Hesap/Giris";
    options.LogoutPath = "/Hesap/Cikis";
    options.AccessDeniedPath = "/Hesap/YetkisizErisim";
});

// Yönetim sayfaları için varsayılan yetkilendirme politikası (oturum zorunlu).
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Yonetim");
});

builder.Services.AddAuthorization();

var app = builder.Build();

// HTTP istek hattını yapılandır.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// --- Cascade dropdown AJAX uç noktaları (davet oluşturma ekranı için) ---
// Verilen hastaneye bağlı aktif birimleri JSON olarak döndürür.
app.MapGet("/api/birimler", async (int hastaneId, IBirimServisi birimServisi) =>
{
    var birimler = await birimServisi.HastaneyeGoreGetirAsync(hastaneId);
    var sonuc = birimler
        .Where(b => b.AktifMi)
        .Select(b => new { id = b.Id, ad = b.Ad });
    return Results.Ok(sonuc);
}).RequireAuthorization();

// Verilen birimde görev yapan aktif doktorları JSON olarak döndürür.
app.MapGet("/api/doktorlar", async (int birimId, IDoktorServisi doktorServisi) =>
{
    var doktorlar = await doktorServisi.BirimeGoreGetirAsync(birimId);
    var sonuc = doktorlar
        .Where(d => d.AktifMi)
        .Select(d => new { id = d.Id, ad = d.TamAd });
    return Results.Ok(sonuc);
}).RequireAuthorization();

// Veritabanı migration'larını otomatik uygula ve (istenirse) başlangıç verilerini yükle.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (app.Configuration.GetValue("SeedVerisiniYukle", false))
    {
        await SeedData.SeedAsync(scope.ServiceProvider);
    }
}

app.Run();
