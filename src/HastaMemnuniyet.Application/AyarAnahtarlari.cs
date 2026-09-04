namespace HastaMemnuniyet.Application;

/// <summary>
/// Sistem ayarlarına erişimde kullanılan sabit anahtar adlarını içerir.
/// Magic string kullanımını önlemek için tüm ayar anahtarları burada tanımlanır.
/// </summary>
public static class AyarAnahtarlari
{
    /// <summary>Davet bağlantısının geçerlilik süresi (saat).</summary>
    public const string DavetGecerlilikSaati = "DavetGecerlilikSaati";

    /// <summary>Aynı telefona minimum gönderim aralığı (gün).</summary>
    public const string MinimumGonderimAraligi = "MinimumGonderimAraligi";

    /// <summary>Düşük örneklem uyarı eşiği.</summary>
    public const string DusukOrneklemEsigi = "DusukOrneklemEsigi";

    /// <summary>Açık uçlu cevaplar için varsayılan karakter limiti.</summary>
    public const string AcikUcluKarakterLimiti = "AcikUcluKarakterLimiti";

    /// <summary>Dashboard için varsayılan gösterilecek gün sayısı.</summary>
    public const string DashboardVarsayilanGunSayisi = "DashboardVarsayilanGunSayisi";

    /// <summary>Bir davet için gönderilebilecek maksimum hatırlatma sayısı.</summary>
    public const string MaksimumHatirlatmaSayisi = "MaksimumHatirlatmaSayisi";

    /// <summary>Kişisel verilerin (telefon hash, IP) saklanma süresi (gün).</summary>
    public const string KisiselVeriSaklamaSuresiGun = "KisiselVeriSaklamaSuresiGun";

    /// <summary>Anket yanıtı içeriğinin saklanma süresi (gün).</summary>
    public const string AnketYanitiSaklamaSuresiGun = "AnketYanitiSaklamaSuresiGun";

    /// <summary>Süresi dolan kişisel verilerin otomatik anonimleştirilip anonimleştirilmeyeceği.</summary>
    public const string OtomatikAnonimlestir = "OtomatikAnonimlestir";
}
