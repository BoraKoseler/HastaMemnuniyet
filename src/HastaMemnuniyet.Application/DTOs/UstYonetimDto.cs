namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// Üst yönetim dashboard'unda hastaneleri karşılaştırmak için kullanılan özet veri transfer nesnesi.
/// Kişisel veri içermez; yalnızca kurumsal düzeyde toplulaştırılmış değerler taşır.
/// </summary>
public class HastaneKarsilastirmaDto
{
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string HastaneAdi { get; set; } = string.Empty;
    /// <summary>Hastanenin ortalama memnuniyet puanı.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>Hastaneye ait geçerli yanıt sayısı.</summary>
    public int YanitSayisi { get; set; }
    /// <summary>Hastaneye ait davet sayısı.</summary>
    public int DavetSayisi { get; set; }
    /// <summary>Hastanenin yanıt oranı (yüzde).</summary>
    public double YanitOrani { get; set; }
}

/// <summary>
/// Üst yönetim dashboard'unda doktor bazlı puanları gösteren veri transfer nesnesi.
/// Yalnızca minimum örneklem eşiğini karşılayan doktorlar için üretilir.
/// </summary>
public class DoktorPuanDto
{
    /// <summary>Doktor kimliği.</summary>
    public int DoktorId { get; set; }
    /// <summary>Doktorun görüntülenen adı (unvan dâhil).</summary>
    public string DoktorAdi { get; set; } = string.Empty;
    /// <summary>Doktorun ortalama memnuniyet puanı.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>Doktora ait geçerli yanıt sayısı (örneklem büyüklüğü).</summary>
    public int YanitSayisi { get; set; }
}

/// <summary>
/// Üst yönetim dashboard'unda aylık trend gösterimi için kullanılan veri transfer nesnesi.
/// </summary>
public class AylikTrendDto
{
    /// <summary>Trend noktasının ait olduğu yıl.</summary>
    public int Yil { get; set; }
    /// <summary>Trend noktasının ait olduğu ay (1-12).</summary>
    public int Ay { get; set; }
    /// <summary>"MM.yyyy" biçiminde ay etiketi.</summary>
    public string Etiket { get; set; } = string.Empty;
    /// <summary>İlgili aydaki ortalama memnuniyet puanı.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>İlgili aydaki geçerli yanıt sayısı.</summary>
    public int YanitSayisi { get; set; }
}
