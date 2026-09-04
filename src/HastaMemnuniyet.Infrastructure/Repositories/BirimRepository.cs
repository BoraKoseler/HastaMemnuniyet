using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Birim varlığına özgü veri erişim işlemlerini uygular.</summary>
public class BirimRepository : GenericRepository<Birim>, IBirimRepository
{
    /// <summary>Yeni bir <see cref="BirimRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public BirimRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<Birim>> GetAllAsync()
        => await KayitSeti.AsNoTracking()
            .Include(b => b.Hastane)
            .OrderBy(b => b.Hastane!.Ad)
            .ThenBy(b => b.Ad)
            .ToListAsync();

    /// <inheritdoc />
    public override async Task<Birim?> GetByIdAsync(int id)
        => await KayitSeti
            .Include(b => b.Hastane)
            .FirstOrDefaultAsync(b => b.Id == id);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Birim>> HastaneyeGoreGetirAsync(int hastaneId)
        => await KayitSeti.AsNoTracking()
            .Include(b => b.Hastane)
            .Where(b => b.HastaneId == hastaneId)
            .OrderBy(b => b.Ad)
            .ToListAsync();
}
