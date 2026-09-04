using HastaMemnuniyet.Application.DTOs;

namespace HastaMemnuniyet.Application.Interfaces;

/// <summary>Anonim anket doldurma sürecini yöneten servis arayüzü.</summary>
public interface IAnketDoldurmaServisi
{
    /// <summary>Verilen token'ın geçerli (aktif ve süresi dolmamış) olup olmadığını doğrular.</summary>
    /// <param name="token">Davet token değeri.</param>
    /// <returns>Token geçerliyse true.</returns>
    Task<bool> TokenGecerliMiAsync(string token);

    /// <summary>Token'a ait anketi soruları ile birlikte getirir.</summary>
    /// <param name="token">Davet token değeri.</param>
    /// <returns>Anket ya da null.</returns>
    Task<AnketDto?> AnketBilgileriniGetirAsync(string token);

    /// <summary>Hastanın doldurduğu anket cevaplarını kaydeder.</summary>
    /// <param name="dto">Doldurulan cevaplar.</param>
    /// <param name="ipAdresi">İstemci IP adresi.</param>
    /// <param name="kullaniciAjan">İstemci User-Agent bilgisi.</param>
    /// <returns>Kaydedilen yanıtın kimliği.</returns>
    Task<int> YanitKaydetAsync(AnketDoldurDto dto, string? ipAdresi, string? kullaniciAjan);
}
