using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// İyileştirme aksiyonu varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IIyilestirmeAksiyonuRepository : IGenericRepository<IyilestirmeAksiyonu>
{
    /// <summary>Aksiyonu geçmişi ve sorumlu kullanıcısıyla birlikte getirir.</summary>
    /// <param name="id">Aksiyon kimliği.</param>
    /// <returns>Detaylarıyla aksiyon ya da null.</returns>
    Task<IyilestirmeAksiyonu?> DetayGetirAsync(int id);

    /// <summary>Aksiyonları sorumlu kullanıcı bilgileriyle birlikte listeler.</summary>
    /// <returns>Aksiyon listesi.</returns>
    Task<IReadOnlyList<IyilestirmeAksiyonu>> TumunuIliskileriyleGetirAsync();
}
