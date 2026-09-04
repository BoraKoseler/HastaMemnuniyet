namespace HastaMemnuniyet.Application.DTOs;

/// <summary>
/// Açık uçlu (serbest metin) yanıtların anahtar kelime analizinin sonucunu taşıyan veri transfer nesnesi.
/// </summary>
public class MetinAnaliziSonucuDto
{
    /// <summary>Analize dâhil edilen toplam açık uçlu yorum sayısı.</summary>
    public int ToplamYorum { get; set; }

    /// <summary>Anlamlı (dolu) yorum sayısı.</summary>
    public int AnlamliYorumSayisi { get; set; }

    /// <summary>Frekansa göre azalan sırada en sık geçen anahtar kelimeler.</summary>
    public List<AnahtarKelimeDto> EnSikKelimeler { get; set; } = new();

    /// <summary>Analizin kapsadığı başlangıç tarihi.</summary>
    public DateTime BaslangicTarihi { get; set; }

    /// <summary>Analizin kapsadığı bitiş tarihi.</summary>
    public DateTime BitisTarihi { get; set; }

    /// <summary>Analize esas alınan yorumlardan örneklem (en güncel birkaç yorum).</summary>
    public List<string> OrnekYorumlar { get; set; } = new();

    /// <summary>Analiz için yeterli veri olup olmadığını belirtir.</summary>
    public bool VeriVarMi => AnlamliYorumSayisi > 0 && EnSikKelimeler.Count > 0;
}

/// <summary>
/// Tek bir anahtar kelimeyi ve geçiş sıklığını taşıyan veri transfer nesnesi.
/// </summary>
public class AnahtarKelimeDto
{
    /// <summary>Anahtar kelime (küçük harfe indirgenmiş).</summary>
    public string Kelime { get; set; } = string.Empty;

    /// <summary>Kelimenin tüm yorumlardaki toplam geçiş sayısı.</summary>
    public int Sayi { get; set; }
}
