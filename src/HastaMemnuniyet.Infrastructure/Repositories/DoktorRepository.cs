using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Doktor varlığına özgü veri erişim işlemlerini uygular.</summary>
public class DoktorRepository : GenericRepository<Doktor>, IDoktorRepository
{
    /// <summary>Yeni bir <see cref="DoktorRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public DoktorRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Doktor>> BirimeGoreGetirAsync(int birimId)
        => await KayitSeti.AsNoTracking()
            .Where(d => d.DoktorBirimleri.Any(db => db.BirimId == birimId && db.AktifMi))
            .OrderBy(d => d.Soyad)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<Doktor>> BirimleriyleTumunuGetirAsync()
        => await KayitSeti.AsNoTracking()
            .Include(d => d.DoktorBirimleri)
                .ThenInclude(db => db.Birim)
                    .ThenInclude(b => b!.Hastane)
            .OrderBy(d => d.Soyad)
            .ThenBy(d => d.Ad)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<Doktor?> BirimleriyleGetirAsync(int id)
        => await KayitSeti
            .Include(d => d.DoktorBirimleri)
                .ThenInclude(db => db.Birim)
                    .ThenInclude(b => b!.Hastane)
            .FirstOrDefaultAsync(d => d.Id == id);
}
