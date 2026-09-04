namespace HastaMemnuniyet.Domain.Enums;

/// <summary>
/// Anket sorusunun cevaplanma biçimini belirtir.
/// </summary>
public enum SoruTipi
{
    /// <summary>Sayısal puanlama (örn. 1-5 arası).</summary>
    Puanlama = 1,

    /// <summary>Tek seçenek işaretlenebilen çoktan seçmeli soru.</summary>
    TekSecim = 2,

    /// <summary>Birden fazla seçenek işaretlenebilen çoktan seçmeli soru.</summary>
    CokluSecim = 3,

    /// <summary>Evet / Hayır tipinde ikili soru.</summary>
    EvetHayir = 4,

    /// <summary>Serbest metin girişi yapılan açık uçlu soru.</summary>
    AcikUclu = 5,

    /// <summary>Net Tavsiye Skoru (NPS) sorusu: 0-10 arası tavsiye etme olasılığı.</summary>
    Nps = 6
}
