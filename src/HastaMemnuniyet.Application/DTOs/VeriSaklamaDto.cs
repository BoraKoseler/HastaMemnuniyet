namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// KVKK veri saklama politikası özetini ve süresi dolan kayıt sayılarını taşıyan veri transfer nesnesi.
/// </summary>
public class VeriSaklamaOzetiDto
{
    /// <summary>Kişisel verilerin (IP, telefon hash) saklanma süresi (gün).</summary>
    public int KisiselVeriSaklamaSuresiGun { get; set; }

    /// <summary>Anket yanıtı içeriğinin saklanma süresi (gün).</summary>
    public int AnketYanitiSaklamaSuresiGun { get; set; }

    /// <summary>Süresi dolan kişisel veri saklama eşik tarihi.</summary>
    public DateTime KisiselVeriEsikTarihi { get; set; }

    /// <summary>Süresi dolduğu için anonimleştirilmesi gereken yanıt sayısı.</summary>
    public int AnonimlestirilecekYanitSayisi { get; set; }

    /// <summary>Halihazırda anonimleştirilmiş (kişisel verisi temizlenmiş) yanıt sayısı.</summary>
    public int AnonimlestirilmisYanitSayisi { get; set; }

    /// <summary>Sistemdeki toplam yanıt sayısı.</summary>
    public int ToplamYanitSayisi { get; set; }

    /// <summary>Otomatik anonimleştirmenin etkin olup olmadığı.</summary>
    public bool OtomatikAnonimlestirmeEtkin { get; set; }
}

/// <summary>
/// Süresi dolan ve anonimleştirilebilecek tek bir anket yanıtını özetleyen veri transfer nesnesi.
/// </summary>
public class SuresiDolanKayitDto
{
    /// <summary>Yanıt kimliği.</summary>
    public int YanitId { get; set; }

    /// <summary>İlişkili anket adı.</summary>
    public string? AnketAdi { get; set; }

    /// <summary>Yanıtın başlama tarihi.</summary>
    public DateTime BaslamaTarihi { get; set; }

    /// <summary>Yanıtın tamamlanma tarihi (varsa).</summary>
    public DateTime? TamamlanmaTarihi { get; set; }

    /// <summary>Kayıtta hâlâ kişisel veri (IP veya telefon hash) bulunup bulunmadığı.</summary>
    public bool KisiselVeriIceriyor { get; set; }
}

/// <summary>
/// Toplu anonimleştirme işleminin sonucunu özetleyen veri transfer nesnesi.
/// </summary>
public class ImhaRaporuDto
{
    /// <summary>İşlem sırasında anonimleştirilen yanıt sayısı.</summary>
    public int AnonimlestirilenYanitSayisi { get; set; }

    /// <summary>İşlemin gerçekleştirildiği tarih.</summary>
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
}
