namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// Bir doktorun belirli bir dönemdeki performans karnesini taşıyan veri transfer nesnesi.
/// Soru bazlı ortalamalar, genel ortalama, NPS, önceki döneme göre trend ve
/// güçlü/zayıf alanları içerir.
/// </summary>
public class DoktorKarnesiDto
{
    /// <summary>Doktor kimliği.</summary>
    public int DoktorId { get; set; }
    /// <summary>Doktorun tam adı (unvan + ad + soyad).</summary>
    public string DoktorAdSoyad { get; set; } = string.Empty;
    /// <summary>Doktorun unvanı.</summary>
    public string? Unvan { get; set; }
    /// <summary>Doktorun görev yaptığı birimlerin görünen adları.</summary>
    public List<string> Birimler { get; set; } = new();

    /// <summary>Karnenin kapsadığı başlangıç tarihi.</summary>
    public DateTime BaslangicTarihi { get; set; }
    /// <summary>Karnenin kapsadığı bitiş tarihi.</summary>
    public DateTime BitisTarihi { get; set; }

    /// <summary>Doktora ait geçerli yanıt sayısı (örneklem büyüklüğü).</summary>
    public int ToplamYanit { get; set; }
    /// <summary>Doktorun genel ortalama puanı (yalnızca puanlama sorularından).</summary>
    public double OrtalamaPuan { get; set; }

    /// <summary>Doktora ait NPS skoru (NPS sorusu yanıtlanmışsa; aksi hâlde null).</summary>
    public int? NpsSkoru { get; set; }

    /// <summary>Soru bazlı ortalama puanlar.</summary>
    public List<SoruOrtalamaDto> SoruBazliOrtalamalar { get; set; } = new();

    /// <summary>En yüksek ortalamaya sahip güçlü alanlar.</summary>
    public List<SoruOrtalamaDto> GucluAlanlar { get; set; } = new();

    /// <summary>En düşük ortalamaya sahip, iyileştirme gerektiren zayıf alanlar.</summary>
    public List<SoruOrtalamaDto> ZayifAlanlar { get; set; } = new();

    /// <summary>Önceki eşit uzunluktaki dönemin genel ortalama puanı (veri yoksa null).</summary>
    public double? OncekiDonemOrtalamaPuan { get; set; }

    /// <summary>Önceki döneme göre ortalama puandaki yüzdesel değişim (veri yoksa null).</summary>
    public double? TrendYuzdesi { get; set; }

    /// <summary>Örneklem büyüklüğü uyarı eşiği.</summary>
    public int MinimumOrneklem { get; set; }

    /// <summary>Örneklem, güvenilir yorum için eşiğin altındaysa true.</summary>
    public bool DusukOrneklemUyarisi => ToplamYanit > 0 && ToplamYanit < MinimumOrneklem;

    /// <summary>Karnede gösterilecek veri olup olmadığını belirtir.</summary>
    public bool VeriVarMi => ToplamYanit > 0;
}

/// <summary>
/// Tek bir soru için ortalama puanı ve yanıt sayısını taşıyan veri transfer nesnesi.
/// </summary>
public class SoruOrtalamaDto
{
    /// <summary>Soru kimliği.</summary>
    public int SoruId { get; set; }
    /// <summary>Soru metni.</summary>
    public string SoruMetni { get; set; } = string.Empty;
    /// <summary>Sorunun kategorisi (varsa).</summary>
    public string? Kategori { get; set; }
    /// <summary>Soru için ortalama puan.</summary>
    public double Ortalama { get; set; }
    /// <summary>Soruya verilen yanıt (puan) sayısı.</summary>
    public int YanitSayisi { get; set; }
}
