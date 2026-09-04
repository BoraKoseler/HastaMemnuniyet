using System.Security.Claims;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// Denetim kayıtlarını veritabanına yazan servis. Kullanıcı ve IP bilgilerini geçerli HTTP bağlamından alır.
/// </summary>
public class AuditLogServisi : IAuditLogger
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Yeni bir <see cref="AuditLogServisi"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    /// <param name="httpContextAccessor">Geçerli HTTP bağlamına erişim sağlayan bileşen.</param>
    public AuditLogServisi(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public async Task LoglaAsync(
        string islem,
        string? tablo = null,
        string? kayitId = null,
        string? eskiDeger = null,
        string? yeniDeger = null,
        string? detay = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var kullanici = httpContext?.User;

        var kayit = new DenetimKaydi
        {
            Islem = islem,
            Tablo = tablo,
            KayitId = kayitId,
            EskiDeger = eskiDeger,
            YeniDeger = yeniDeger,
            Detay = detay,
            KullaniciId = kullanici?.FindFirstValue(ClaimTypes.NameIdentifier),
            KullaniciAdi = kullanici?.Identity?.Name,
            IpAdresi = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Tarih = DateTime.UtcNow
        };

        await _context.DenetimKayitlari.AddAsync(kayit);
        await _context.SaveChangesAsync();
    }
}
