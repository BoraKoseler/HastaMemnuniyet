using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Application.DTOs;

/// <summary>Anket sorusu bilgilerini taşıyan veri transfer nesnesi.</summary>
public class AnketSorusuDto
{
    /// <summary>Soru kimliği.</summary>
    public int Id { get; set; }
    /// <summary>Bağlı olduğu anketin kimliği.</summary>
    public int AnketId { get; set; }
    /// <summary>Soru metni.</summary>
    public string SoruMetni { get; set; } = string.Empty;
    /// <summary>Soru tipi.</summary>
    public SoruTipi SoruTipi { get; set; }
    /// <summary>Sıra numarası.</summary>
    public int SiraNo { get; set; }
    /// <summary>Zorunlu olup olmadığı.</summary>
    public bool ZorunluMu { get; set; }
    /// <summary>Aktif olup olmadığı.</summary>
    public bool AktifMi { get; set; }
    /// <summary>Soru kategorisi.</summary>
    public string? Kategori { get; set; }
    /// <summary>Puanlama alt sınırı.</summary>
    public int? PuanlamaAltSinir { get; set; }
    /// <summary>Puanlama üst sınırı.</summary>
    public int? PuanlamaUstSinir { get; set; }
    /// <summary>Açık uçlu sorular için maksimum karakter sayısı.</summary>
    public int? MaksimumKarakterSayisi { get; set; }
    /// <summary>Görünürlüğün bağlı olduğu sorunun kimliği (koşullu soru için).</summary>
    public int? KosulBagliSoruId { get; set; }
    /// <summary>Koşulun sağlanması için bağlı sorunun taşıması gereken değer.</summary>
    public string? KosulDegeri { get; set; }
    /// <summary>Soru seçenekleri.</summary>
    public List<AnketSoruSecenegiDto> Secenekler { get; set; } = new();
}
