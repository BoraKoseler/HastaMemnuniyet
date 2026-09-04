namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket soru seçeneği bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketSoruSecenegiDto
{
    /// <summary>Seçenek kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Bağlı olduğu sorunun kimliği.</summary>
    public int SoruId { get; set; }
    /// <summary>Seçenek metni.</summary>
    public string MetinDegeri { get; set; } = string.Empty;
    /// <summary>Sıra numarası.</summary>
    public int SiraNo { get; set; }
    /// <summary>Aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
}
