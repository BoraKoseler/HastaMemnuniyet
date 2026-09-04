using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Anket daveti varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IAnketDavetiRepository : IGenericRepository<AnketDaveti>
{
    /// <summary>Verilen token'a sahip daveti getirir.</summary>
    /// <param name="token">Davet token değeri.</param>
    /// <returns>Bulunan davet ya da null.</returns>
    Task<AnketDaveti?> GetByTokenAsync(string token);
}
