using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Anket varlığına özgü veri erişim işlemlerini uygular.</summary>
public class AnketRepository : GenericRepository<Anket>, IAnketRepository
{
    /// <summary>Yeni bir <see cref="AnketRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public AnketRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<Anket>> GetAllAsync()
        => await KayitSeti.AsNoTracking()
            .Include(a => a.Sorular)
            .OrderBy(a => a.Ad)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<Anket?> SorulariylaGetirAsync(int anketId)
        => await KayitSeti.AsNoTracking()
            .Include(a => a.Sorular.OrderBy(s => s.SiraNo))
                .ThenInclude(s => s.Secenekler.OrderBy(o => o.SiraNo))
            .FirstOrDefaultAsync(a => a.Id == anketId);
}
