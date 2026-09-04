using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Kritik geri bildirim varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IKritikGeriBildirimRepository : IGenericRepository<KritikGeriBildirim>
{
    /// <summary>Kritik geri bildirimi ilişkili yanıt, kural ve aksiyonlarıyla birlikte getirir.</summary>
    /// <param name="id">Kritik geri bildirim kimliği.</param>
    /// <returns>Detaylarıyla kritik geri bildirim ya da null.</returns>
    Task<KritikGeriBildirim?> DetayGetirAsync(int id);

    /// <summary>Kritik geri bildirimleri kural ve yanıt bilgileriyle birlikte listeler.</summary>
    /// <returns>Kritik geri bildirim listesi.</returns>
    Task<IReadOnlyList<KritikGeriBildirim>> TumunuIliskileriyleGetirAsync();
}
