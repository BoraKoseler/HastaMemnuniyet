using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Sistem ayarı varlığına özgü veri erişim işlemlerini uygular.</summary>
public class SistemAyariRepository : ISistemAyariRepository
{
    private readonly AppDbContext _context;

    /// <summary>Yeni bir <see cref="SistemAyariRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public SistemAyariRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<SistemAyari?> AnahtaraGoreGetirAsync(string anahtar)
        => await _context.SistemAyarlari.FirstOrDefaultAsync(a => a.Anahtar == anahtar);

    /// <inheritdoc />
    public async Task<IReadOnlyList<SistemAyari>> TumunuGetirAsync()
        => await _context.SistemAyarlari.AsNoTracking().OrderBy(a => a.Anahtar).ToListAsync();

    /// <inheritdoc />
    public async Task<string> DegerGetirAsync(string anahtar, string varsayilan)
    {
        var ayar = await _context.SistemAyarlari.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Anahtar == anahtar);
        return ayar is null || string.IsNullOrWhiteSpace(ayar.Deger) ? varsayilan : ayar.Deger;
    }

    /// <inheritdoc />
    public async Task<SistemAyari> AddAsync(SistemAyari ayar)
    {
        await _context.SistemAyarlari.AddAsync(ayar);
        return ayar;
    }

    /// <inheritdoc />
    public void Update(SistemAyari ayar)
    {
        ayar.GuncellenmeTarihi = DateTime.UtcNow;
        _context.SistemAyarlari.Update(ayar);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
