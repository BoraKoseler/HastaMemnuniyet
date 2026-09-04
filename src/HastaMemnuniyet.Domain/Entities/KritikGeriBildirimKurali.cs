using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Anket yanıtlarının kritik olarak işaretlenmesini sağlayan değerlendirme kuralını temsil eder.
/// </summary>
public class KritikGeriBildirimKurali : BaseEntity
{
    /// <summary>Kuralın uygulanacağı anketin kimliği. Boş ise tüm anketlere uygulanır.</summary>
    public int? AnketId { get; set; }

    /// <summary>Kuralın uygulanacağı sorunun kimliği. Boş ise anketin ilgili tipteki tüm sorularına uygulanır.</summary>
    public int? SoruId { get; set; }

    /// <summary>Kuralın değerlendirme tipi.</summary>
    public KuralTipi KuralTipi { get; set; }

    /// <summary>Puan altı kuralları için eşik değeri. Bu değere eşit veya altındaki puanlar kritik sayılır.</summary>
    public int? EsikDegeri { get; set; }

    /// <summary>Anahtar kelime kuralları için virgülle ayrılmış anahtar kelime listesi.</summary>
    public string? AnahtarKelimeler { get; set; }

    /// <summary>Seçenek eşleşmesi kuralları için kritik kabul edilen seçeneğin kimliği.</summary>
    public int? HedefSecenekId { get; set; }

    /// <summary>Kuralın aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Kuralın uygulandığı anket (isteğe bağlı).</summary>
    public Anket? Anket { get; set; }

    /// <summary>Kuralın uygulandığı soru (isteğe bağlı).</summary>
    public AnketSorusu? Soru { get; set; }
}
