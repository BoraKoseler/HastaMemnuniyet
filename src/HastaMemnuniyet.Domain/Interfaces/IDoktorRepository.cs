using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Doktor varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IDoktorRepository : IGenericRepository<Doktor>
{
    /// <summary>Verilen birimde görev yapan doktorları getirir.</summary>
    /// <param name="birimId">Birim kimliği.</param>
    /// <returns>Doktor listesi.</returns>
    Task<IReadOnlyList<Doktor>> BirimeGoreGetirAsync(int birimId);

    /// <summary>Tüm doktorları birim ve hastane ilişkileri ile birlikte getirir.</summary>
    /// <returns>Doktor listesi.</returns>
    Task<IReadOnlyList<Doktor>> BirimleriyleTumunuGetirAsync();

    /// <summary>Verilen kimliğe sahip doktoru birim ilişkileri ile birlikte (izlenerek) getirir.</summary>
    /// <param name="id">Doktor kimliği.</param>
    /// <returns>Doktor ya da null.</returns>
    Task<Doktor?> BirimleriyleGetirAsync(int id);
}
