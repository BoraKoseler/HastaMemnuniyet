using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Tamamlanmış anket yanıtlarının görüntülenmesi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IYanitServisi
{
    /// <summary>Verilen filtrelere uyan yanıtları getirir.</summary>
    /// <param name="baslangic">Başlangıç tarihi (isteğe bağlı).</param>
    /// <param name="bitis">Bitiş tarihi (isteğe bağlı).</param>
    /// <param name="hastaneId">Hastane filtresi (isteğe bağlı).</param>
    /// <param name="anketId">Anket filtresi (isteğe bağlı).</param>
    /// <param name="kapsam">Kullanıcı veri kapsamı (null ise kısıt uygulanmaz).</param>
    /// <returns>Yanıt listesi.</returns>
    Task<IReadOnlyList<AnketYanitiDto>> TumunuGetirAsync(
        DateTime? baslangic = null,
        DateTime? bitis = null,
        int? hastaneId = null,
        int? anketId = null,
        KapsamFiltresi? kapsam = null);

    /// <summary>Verilen kimliğe sahip yanıtı tüm cevapları ile birlikte getirir.</summary>
    /// <param name="id">Yanıt kimliği.</param>
    /// <returns>Yanıt ya da null.</returns>
    Task<AnketYanitiDto?> DetayGetirAsync(int id);
}
