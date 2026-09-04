using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Anket, soru ve seçenek yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IAnketServisi
{
    /// <summary>Tüm anketleri getirir.</summary>
    /// <returns>Anket listesi.</returns>
    Task<IReadOnlyList<AnketDto>> TumunuGetirAsync();

    /// <summary>Verilen kimliğe sahip anketi soruları ile birlikte getirir.</summary>
    /// <param name="id">Anket kimliği.</param>
    /// <returns>Anket ya da null.</returns>
    Task<AnketDto?> GetirAsync(int id);

    /// <summary>Yeni bir anket oluşturur.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <returns>Oluşturulan anket.</returns>
    Task<AnketDto> OlusturAsync(AnketOlusturDto dto);

    /// <summary>Mevcut bir anketi günceller.</summary>
    /// <param name="dto">Güncelleme bilgileri.</param>
    /// <returns>Güncellenen anket.</returns>
    Task<AnketDto> GuncelleAsync(AnketDto dto);

    /// <summary>Verilen kimliğe sahip anketi siler.</summary>
    /// <param name="id">Anket kimliği.</param>
    Task SilAsync(int id);

    /// <summary>
    /// Mevcut anketin sorularıyla birlikte yeni bir sürümünü oluşturur.
    /// Yeni sürümün sürüm numarası bir artırılır ve aktif yapılırken, önceki sürüm pasifleştirilir.
    /// </summary>
    /// <param name="anketId">Sürümü oluşturulacak (kaynak) anketin kimliği.</param>
    /// <returns>Oluşturulan yeni sürüm anketi.</returns>
    Task<AnketDto> SurumOlusturAsync(int anketId);

    /// <summary>
    /// Verilen anketle aynı sürüm zincirine ait tüm sürümleri (kendisi dâhil) sürüm numarasına göre getirir.
    /// </summary>
    /// <param name="anketId">Sürüm geçmişi istenen anketin kimliği.</param>
    /// <returns>Aynı zincirdeki anket sürümleri.</returns>
    Task<IReadOnlyList<AnketDto>> GecmisSurumleriGetirAsync(int anketId);

    /// <summary>Ankete yeni bir soru ekler.</summary>
    /// <param name="dto">Soru oluşturma bilgileri.</param>
    /// <returns>Oluşturulan soru.</returns>
    Task<AnketSorusuDto> SoruEkleAsync(AnketSorusuOlusturDto dto);

    /// <summary>Mevcut bir soruyu günceller.</summary>
    /// <param name="dto">Soru bilgileri.</param>
    /// <returns>Güncellenen soru.</returns>
    Task<AnketSorusuDto> SoruGuncelleAsync(AnketSorusuDto dto);

    /// <summary>Verilen kimliğe sahip soruyu siler.</summary>
    /// <param name="soruId">Soru kimliği.</param>
    Task SoruSilAsync(int soruId);

    /// <summary>Bir soruya seçenek ekler.</summary>
    /// <param name="soruId">Soru kimliği.</param>
    /// <param name="metinDegeri">Seçenek metni.</param>
    /// <returns>Oluşturulan seçenek.</returns>
    Task<AnketSoruSecenegiDto> SecenekEkleAsync(int soruId, string metinDegeri);

    /// <summary>Verilen kimliğe sahip seçeneği siler.</summary>
    /// <param name="secenekId">Seçenek kimliği.</param>
    Task SecenekSilAsync(int secenekId);
}
