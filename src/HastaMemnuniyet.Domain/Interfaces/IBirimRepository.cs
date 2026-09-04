using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Birim varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IBirimRepository : IGenericRepository<Birim>
{
    /// <summary>Verilen hastaneye bağlı birimleri getirir.</summary>
    /// <param name="hastaneId">Hastane kimliği.</param>
    /// <returns>Birim listesi.</returns>
    Task<IReadOnlyList<Birim>> HastaneyeGoreGetirAsync(int hastaneId);
}
