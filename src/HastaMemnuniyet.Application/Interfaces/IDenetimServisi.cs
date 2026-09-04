using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Denetim kayıtlarının sorgulanması iş kurallarını tanımlayan servis arayüzü.</summary>
public interface IDenetimServisi
{
    /// <summary>
    /// En son oluşturulan denetim kayıtlarını, isteğe bağlı filtreler ile getirir.
    /// </summary>
    /// <param name="adet">Getirilecek en fazla kayıt sayısı.</param>
    /// <param name="islem">İşlem türü filtresi (isteğe bağlı).</param>
    /// <param name="kullaniciAdi">Kullanıcı adı filtresi (isteğe bağlı).</param>
    /// <returns>Tarihe göre azalan sıralı denetim kaydı listesi.</returns>
    Task<IReadOnlyList<DenetimKaydiDto>> SonKayitlariGetirAsync(int adet = 200, string? islem = null, string? kullaniciAdi = null);
}
