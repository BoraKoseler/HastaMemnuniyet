using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Anket varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IAnketRepository : IGenericRepository<Anket>
{
    /// <summary>Anketi soruları ve seçenekleri ile birlikte getirir.</summary>
    /// <param name="anketId">Anket kimliği.</param>
    /// <returns>Detaylarıyla anket ya da null.</returns>
    Task<Anket?> SorulariylaGetirAsync(int anketId);
}
