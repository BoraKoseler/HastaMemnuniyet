using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// Denetim kayıtlarını veritabanından sorgulayan servis.
/// </summary>
public class DenetimServisi : IDenetimServisi
{
    private readonly AppDbContext _context;

    /// <summary>Yeni bir <see cref="DenetimServisi"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public DenetimServisi(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DenetimKaydiDto>> SonKayitlariGetirAsync(int adet = 200, string? islem = null, string? kullaniciAdi = null)
    {
        var sorgu = _context.DenetimKayitlari.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(islem))
        {
            sorgu = sorgu.Where(d => d.Islem == islem);
        }

        if (!string.IsNullOrWhiteSpace(kullaniciAdi))
        {
            sorgu = sorgu.Where(d => d.KullaniciAdi != null && d.KullaniciAdi.Contains(kullaniciAdi));
        }

        return await sorgu
            .OrderByDescending(d => d.Tarih)
            .Take(adet)
            .Select(d => new DenetimKaydiDto
            {
                Id = d.Id,
                KullaniciId = d.KullaniciId,
                KullaniciAdi = d.KullaniciAdi,
                Islem = d.Islem,
                Tablo = d.Tablo,
                KayitId = d.KayitId,
                EskiDeger = d.EskiDeger,
                YeniDeger = d.YeniDeger,
                IpAdresi = d.IpAdresi,
                Tarih = d.Tarih,
                Detay = d.Detay
            })
            .ToListAsync();
    }
}
