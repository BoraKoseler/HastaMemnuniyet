namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// İki dönemin memnuniyet göstergelerini karşılaştıran veri transfer nesnesi.
/// "Önceki dönem" kıyas noktası, "güncel dönem" ise değerlendirilen dönemdir.
/// Değişim yüzdeleri güncel dönemin önceki döneme göre yüzdesel farkını gösterir.
/// </summary>
public class DonemselKarsilastirmaDto
{
    /// <summary>Önceki (kıyas) dönemin başlangıç tarihi.</summary>
    public DateTime OncekiBaslangic { get; set; }
    /// <summary>Önceki (kıyas) dönemin bitiş tarihi.</summary>
    public DateTime OncekiBitis { get; set; }
    /// <summary>Güncel dönemin başlangıç tarihi.</summary>
    public DateTime GuncelBaslangic { get; set; }
    /// <summary>Güncel dönemin bitiş tarihi.</summary>
    public DateTime GuncelBitis { get; set; }

    /// <summary>Önceki dönem ortalama puanı.</summary>
    public double OncekiOrtalamaPuan { get; set; }
    /// <summary>Güncel dönem ortalama puanı.</summary>
    public double GuncelOrtalamaPuan { get; set; }
    /// <summary>Ortalama puanın yüzdesel değişimi.</summary>
    public double OrtalamaPuanDegisimYuzdesi { get; set; }

    /// <summary>Önceki dönem geçerli yanıt sayısı.</summary>
    public int OncekiYanitSayisi { get; set; }
    /// <summary>Güncel dönem geçerli yanıt sayısı.</summary>
    public int GuncelYanitSayisi { get; set; }
    /// <summary>Yanıt sayısının yüzdesel değişimi.</summary>
    public double YanitSayisiDegisimYuzdesi { get; set; }

    /// <summary>Önceki dönem yanıt oranı (yüzde).</summary>
    public double OncekiYanitOrani { get; set; }
    /// <summary>Güncel dönem yanıt oranı (yüzde).</summary>
    public double GuncelYanitOrani { get; set; }
    /// <summary>Yanıt oranının puan (yüzde) cinsinden farkı.</summary>
    public double YanitOraniFarki { get; set; }

    /// <summary>Önceki dönem NPS skoru.</summary>
    public int OncekiNps { get; set; }
    /// <summary>Güncel dönem NPS skoru.</summary>
    public int GuncelNps { get; set; }
    /// <summary>NPS skorunun puan cinsinden farkı.</summary>
    public int NpsFarki { get; set; }

    /// <summary>Hastane bazlı dönemsel karşılaştırma satırları.</summary>
    public List<HastaneDonemKarsilastirmaDto> HastaneKarsilastirmalari { get; set; } = new();
}

/// <summary>
/// Tek bir hastanenin iki dönem arasındaki ortalama puan karşılaştırmasını taşıyan veri transfer nesnesi.
/// </summary>
public class HastaneDonemKarsilastirmaDto
{
    /// <summary>Hastane kimliği.</summary>
    public int HastaneId { get; set; }
    /// <summary>Hastane adı.</summary>
    public string HastaneAdi { get; set; } = string.Empty;
    /// <summary>Önceki dönem ortalama puanı.</summary>
    public double OncekiOrtalamaPuan { get; set; }
    /// <summary>Güncel dönem ortalama puanı.</summary>
    public double GuncelOrtalamaPuan { get; set; }
    /// <summary>Ortalama puan farkı (güncel − önceki).</summary>
    public double Fark { get; set; }
    /// <summary>Önceki dönem yanıt sayısı.</summary>
    public int OncekiYanitSayisi { get; set; }
    /// <summary>Güncel dönem yanıt sayısı.</summary>
    public int GuncelYanitSayisi { get; set; }
}
