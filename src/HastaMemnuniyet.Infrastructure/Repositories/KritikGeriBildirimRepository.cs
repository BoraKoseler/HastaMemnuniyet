using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Kritik geri bildirim varlığına özgü veri erişim işlemlerini uygular.</summary>
public class KritikGeriBildirimRepository : GenericRepository<KritikGeriBildirim>, IKritikGeriBildirimRepository
{
    /// <summary>Yeni bir <see cref="KritikGeriBildirimRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public KritikGeriBildirimRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<KritikGeriBildirim?> DetayGetirAsync(int id)
        => await KayitSeti.AsNoTracking()
            .Include(k => k.Kural)
            .Include(k => k.Yanit)
            .Include(k => k.Aksiyonlar)
            .FirstOrDefaultAsync(k => k.Id == id);

    /// <inheritdoc />
    public async Task<IReadOnlyList<KritikGeriBildirim>> TumunuIliskileriyleGetirAsync()
        => await KayitSeti.AsNoTracking()
            .Include(k => k.Kural)
            .Include(k => k.Yanit)
            .Include(k => k.Aksiyonlar)
            .ToListAsync();
}
