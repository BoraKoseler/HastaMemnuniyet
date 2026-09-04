using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>Hastane varlığına özgü veri erişim işlemlerini uygular.</summary>
public class HastaneRepository : GenericRepository<Hastane>, IHastaneRepository
{
    /// <summary>Yeni bir <see cref="HastaneRepository"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public HastaneRepository(AppDbContext context) : base(context)
    {
    }
}
