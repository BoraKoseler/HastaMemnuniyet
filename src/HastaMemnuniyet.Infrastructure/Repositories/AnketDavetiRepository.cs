using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Anket daveti varlığına özgü veri erişim işlemlerini uygular.</summary>
public class AnketDavetiRepository : GenericRepository<AnketDaveti>, IAnketDavetiRepository
{
    /// <summary>Yeni bir <see cref="AnketDavetiRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public AnketDavetiRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<AnketDaveti>> GetAllAsync()
        => await KayitSeti.AsNoTracking()
            .Include(d => d.Anket)
            .Include(d => d.Hastane)
            .Include(d => d.Birim)
            .OrderByDescending(d => d.OlusturulmaTarihi)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<AnketDaveti?> GetByTokenAsync(string token)
        => await KayitSeti
            .Include(d => d.Anket)
            .Include(d => d.Hastane)
            .Include(d => d.Birim)
            .FirstOrDefaultAsync(d => d.Token == token);
}
