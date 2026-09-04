namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// Net Tavsiye Skoru (NPS) hesaplama sonucunu taşıyan veri transfer nesnesi.
/// NPS, 0-10 ölçeğindeki tavsiye sorularından hesaplanır:
/// 9-10 puan verenler Destekçi (Promoter), 7-8 puan verenler Pasif (Passive),
/// 0-6 puan verenler Kötüleyen (Detractor) olarak sınıflandırılır.
/// NPS = (Destekçi % − Kötüleyen %), sonuç -100 ile +100 arasında bir tam sayıdır.
/// </summary>
public class NpsSonucuDto
{
    /// <summary>NPS sorularına verilen toplam geçerli yanıt (puan) sayısı.</summary>
    public int ToplamYanit { get; set; }

    /// <summary>Destekçi (9-10 puan) sayısı.</summary>
    public int DestekciSayisi { get; set; }

    /// <summary>Pasif (7-8 puan) sayısı.</summary>
    public int PasifSayisi { get; set; }

    /// <summary>Kötüleyen (0-6 puan) sayısı.</summary>
    public int KotuleyenSayisi { get; set; }

    /// <summary>Destekçilerin toplam içindeki yüzdesi.</summary>
    public double DestekciYuzdesi { get; set; }

    /// <summary>Pasiflerin toplam içindeki yüzdesi.</summary>
    public double PasifYuzdesi { get; set; }

    /// <summary>Kötüleyenlerin toplam içindeki yüzdesi.</summary>
    public double KotuleyenYuzdesi { get; set; }

    /// <summary>Hesaplanan NPS skoru (-100 ile +100 arası).</summary>
    public int NpsSkoru { get; set; }

    /// <summary>NPS skorunun sözel kategorisi (örn. "Mükemmel", "İyi", "Geliştirilmeli").</summary>
    public string Kategori { get; set; } = string.Empty;

    /// <summary>Sonucun kapsadığı başlangıç tarihi.</summary>
    public DateTime BaslangicTarihi { get; set; }

    /// <summary>Sonucun kapsadığı bitiş tarihi.</summary>
    public DateTime BitisTarihi { get; set; }

    /// <summary>Yeterli veri (en az bir NPS yanıtı) olup olmadığını belirtir.</summary>
    public bool VeriVarMi => ToplamYanit > 0;
}
