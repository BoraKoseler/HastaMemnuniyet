using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Anket daveti oluşturma ve yönetimi iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IDavetServisi
{
    /// <summary>Tüm davetleri getirir.</summary>
    /// <returns>Davet listesi.</returns>
    Task<IReadOnlyList<AnketDavetiDto>> TumunuGetirAsync();

    /// <summary>Verilen kimliğe sahip daveti getirir.</summary>
    /// <param name="id">Davet kimliği.</param>
    /// <returns>Davet ya da null.</returns>
    Task<AnketDavetiDto?> GetirAsync(int id);

    /// <summary>Verilen token'a sahip daveti getirir.</summary>
    /// <param name="token">Davet token değeri.</param>
    /// <returns>Davet ya da null.</returns>
    Task<AnketDavetiDto?> TokenIleGetirAsync(string token);

    /// <summary>Yeni bir davet oluşturur ve gerekiyorsa SMS gönderimini tetikler.</summary>
    /// <param name="dto">Oluşturma bilgileri.</param>
    /// <param name="olusturanKullaniciId">Daveti oluşturan kullanıcının kimliği.</param>
    /// <returns>Oluşturulan davet.</returns>
    Task<AnketDavetiDto> OlusturAsync(AnketDavetiOlusturDto dto, string? olusturanKullaniciId);

    /// <summary>Davetin durumunu günceller.</summary>
    /// <param name="davetId">Davet kimliği.</param>
    /// <param name="yeniDurum">Atanacak yeni durum.</param>
    Task DurumGuncelleAsync(int davetId, DavetDurumu yeniDurum);

    /// <summary>
    /// Gönderilmiş bir davet için hatırlatma SMS'i tetikler.
    /// Maksimum hatırlatma sayısı aşılırsa ya da davet uygun durumda değilse hata fırlatır.
    /// </summary>
    /// <param name="davetId">Hatırlatma gönderilecek davetin kimliği.</param>
    Task HatirlatmaGonderAsync(int davetId);
}
