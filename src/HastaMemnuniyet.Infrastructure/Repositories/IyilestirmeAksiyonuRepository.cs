using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>İyileştirme aksiyonu varlığına özgü veri erişim işlemlerini uygular.</summary>
public class IyilestirmeAksiyonuRepository : GenericRepository<IyilestirmeAksiyonu>, IIyilestirmeAksiyonuRepository
{
    /// <summary>Yeni bir <see cref="IyilestirmeAksiyonuRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public IyilestirmeAksiyonuRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IyilestirmeAksiyonu?> DetayGetirAsync(int id)
        => await KayitSeti.AsNoTracking()
            .Include(a => a.SorumluKullanici)
            .Include(a => a.KritikGeriBildirim)
            .Include(a => a.Gecmis)
            .FirstOrDefaultAsync(a => a.Id == id);

    /// <inheritdoc />
    public async Task<IReadOnlyList<IyilestirmeAksiyonu>> TumunuIliskileriyleGetirAsync()
        => await KayitSeti.AsNoTracking()
            .Include(a => a.SorumluKullanici)
            .ToListAsync();
}
