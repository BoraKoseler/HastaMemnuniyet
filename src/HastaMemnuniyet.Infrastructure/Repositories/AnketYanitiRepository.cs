using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Anket yanıtı varlığına özgü veri erişim işlemlerini uygular.</summary>
public class AnketYanitiRepository : GenericRepository<AnketYaniti>, IAnketYanitiRepository
{
    /// <summary>Yeni bir <see cref="AnketYanitiRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public AnketYanitiRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<AnketYaniti?> CevaplariylaGetirAsync(int yanitId)
        => await KayitSeti.AsNoTracking()
            .Include(y => y.Davet)
            .Include(y => y.Cevaplar)
                .ThenInclude(c => c.Soru)
            .Include(y => y.Cevaplar)
                .ThenInclude(c => c.Secenek)
            .FirstOrDefaultAsync(y => y.Id == yanitId);
}
