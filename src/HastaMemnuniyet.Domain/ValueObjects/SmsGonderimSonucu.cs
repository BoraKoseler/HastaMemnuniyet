namespace HastaMemnuniyet.Domain.ValueObjects;

/// <summary>
/// SMS gönderim işleminin sonucunu taşıyan değer nesnesi.
/// </summary>
public class SmsGonderimSonucu
{
    /// <summary>Gönderimin başarılı olup olmadığını belirtir.</summary>
    public bool Basarili { get; set; }

    /// <summary>SMS sağlayıcısından dönen mesaj/yanıt.</summary>
    public string? Mesaj { get; set; }

    /// <summary>Sağlayıcı tarafından üretilen gönderim referans/işlem kimliği.</summary>
    public string? SaglayiciReferansi { get; set; }

    /// <summary>Başarılı bir gönderim sonucu oluşturur.</summary>
    /// <param name="mesaj">İsteğe bağlı bilgi mesajı.</param>
    /// <param name="saglayiciReferansi">İsteğe bağlı sağlayıcı referansı.</param>
    /// <returns>Başarılı sonucu temsil eden nesne.</returns>
    public static SmsGonderimSonucu BasariliOlustur(string? mesaj = null, string? saglayiciReferansi = null)
        => new() { Basarili = true, Mesaj = mesaj, SaglayiciReferansi = saglayiciReferansi };

    /// <summary>Başarısız bir gönderim sonucu oluşturur.</summary>
    /// <param name="mesaj">Hata mesajı.</param>
    /// <returns>Başarısız sonucu temsil eden nesne.</returns>
    public static SmsGonderimSonucu BasarisizOlustur(string mesaj)
        => new() { Basarili = false, Mesaj = mesaj };
}
