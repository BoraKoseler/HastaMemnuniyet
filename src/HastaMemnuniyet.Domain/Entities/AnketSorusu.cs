using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Bir ankete ait tekil soru varlığı.
/// </summary>
public class AnketSorusu : BaseEntity
{
    /// <summary>Sorunun bağlı olduğu anketin kimliği.</summary>
    public int AnketId { get; set; }

    /// <summary>Soru metni.</summary>
    public string SoruMetni { get; set; } = string.Empty;

    /// <summary>Sorunun cevaplanma biçimi (tipi).</summary>
    public SoruTipi SoruTipi { get; set; }

    /// <summary>Sorunun anket içindeki sıra numarası.</summary>
    public int SiraNo { get; set; }

    /// <summary>Sorunun cevaplanmasının zorunlu olup olmadığını belirtir.</summary>
    public bool ZorunluMu { get; set; }

    /// <summary>Sorunun aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Sorunun kategorisi (isteğe bağlı gruplama).</summary>
    public string? Kategori { get; set; }

    /// <summary>Puanlama tipi sorular için alt sınır değeri.</summary>
    public int? PuanlamaAltSinir { get; set; }

    /// <summary>Puanlama tipi sorular için üst sınır değeri.</summary>
    public int? PuanlamaUstSinir { get; set; }

    /// <summary>Açık uçlu tip sorular için izin verilen maksimum karakter sayısı.</summary>
    public int? MaksimumKarakterSayisi { get; set; }

    /// <summary>
    /// Bu sorunun görünürlüğünün bağlı olduğu (aynı anketteki daha önceki) sorunun kimliği.
    /// Null ise soru koşulsuzdur ve her zaman gösterilir.
    /// </summary>
    public int? KosulBagliSoruId { get; set; }

    /// <summary>
    /// Koşul sağlanması için bağlı sorunun sahip olması gereken değer (örn. "Evet",
    /// bir seçenek kimliği ya da puan). Null ise koşul uygulanmaz.
    /// </summary>
    public string? KosulDegeri { get; set; }

    /// <summary>Sorunun bağlı olduğu anket.</summary>
    public Anket? Anket { get; set; }

    /// <summary>Görünürlük koşulunun bağlı olduğu soru (isteğe bağlı).</summary>
    public AnketSorusu? KosulBagliSoru { get; set; }

    /// <summary>Soruya ait seçenekler (çoktan seçmeli tipler için).</summary>
    public ICollection<AnketSoruSecenegi> Secenekler { get; set; } = new List<AnketSoruSecenegi>();
}
