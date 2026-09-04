namespace HastaMemnuniyet.Domain.Entities;

/// <summary>
/// Çoktan seçmeli bir soruya ait seçenek varlığı.
/// </summary>
public class AnketSoruSecenegi : BaseEntity
{
    /// <summary>Seçeneğin bağlı olduğu sorunun kimliği.</summary>
    public int SoruId { get; set; }

    /// <summary>Seçeneğin metin değeri.</summary>
    public string MetinDegeri { get; set; } = string.Empty;

    /// <summary>Seçeneğin soru içindeki sıra numarası.</summary>
    public int SiraNo { get; set; }

    /// <summary>Seçeneğin aktif olup olmadığını belirtir.</summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>Seçeneğin bağlı olduğu soru.</summary>
    public AnketSorusu? Soru { get; set; }
}
