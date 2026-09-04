using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Anket yanıtı varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IAnketYanitiRepository : IGenericRepository<AnketYaniti>
{
    /// <summary>Yanıtı cevaplarıyla birlikte getirir.</summary>
    /// <param name="yanitId">Yanıt kimliği.</param>
    /// <returns>Detaylarıyla yanıt ya da null.</returns>
    Task<AnketYaniti?> CevaplariylaGetirAsync(int yanitId);
}
