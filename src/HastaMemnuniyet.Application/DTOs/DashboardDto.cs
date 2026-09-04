namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Dashboard istatistiklerini taşıyan veri transfer nesnesi.</summary>
public class DashboardDto
{
    /// <summary>Toplam aktif anket sayısı.</summary>
    public int ToplamAnketSayisi { get; set; }
    /// <summary>Toplam davet sayısı.</summary>
    public int ToplamDavetSayisi { get; set; }
    /// <summary>Toplam tamamlanan yanıt sayısı.</summary>
    public int ToplamYanitSayisi { get; set; }
    /// <summary>Yanıt oranı (yüzde).</summary>
    public double YanitOrani { get; set; }
    /// <summary>Genel ortalama puan.</summary>
    public double OrtalamaPuan { get; set; }
    /// <summary>Hastane bazlı puan dağılımı.</summary>
    public List<HastanePuanDto> HastanePuanlari { get; set; } = new();
    /// <summary>Birim bazlı puan dağılımı.</summary>
    public List<BirimPuanDto> BirimPuanlari { get; set; } = new();
    /// <summary>Aktif kritik geri bildirim sayısı.</summary>
    public int KritikGeriBildirimSayisi { get; set; }
    /// <summary>Açık (yeni veya devam eden) iyileştirme aksiyonu sayısı.</summary>
    public int AcikAksiyonSayisi { get; set; }
    /// <summary>Gecikmiş (hedef tarihi geçmiş ve kapanmamış) iyileştirme aksiyonu sayısı.</summary>
    public int GecikmisAksiyonSayisi { get; set; }
    /// <summary>İstatistiğin kapsadığı başlangıç tarihi.</summary>
    public DateTime BaslangicTarihi { get; set; }
    /// <summary>İstatistiğin kapsadığı bitiş tarihi.</summary>
    public DateTime BitisTarihi { get; set; }
}
