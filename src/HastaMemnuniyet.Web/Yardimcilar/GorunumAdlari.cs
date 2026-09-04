using HastaMemnuniyet.Domain.Enums;

namespace HastaMemnuniyet.Web.Yardimcilar;

/// <summary>
/// Enum değerleri için kullanıcıya gösterilecek Türkçe etiketleri üreten yardımcı sınıf.
/// </summary>
public static class GorunumAdlari
{
    /// <summary>Anket türü için Türkçe görünen ad döndürür.</summary>
    /// <param name="tur">Anket türü.</param>
    /// <returns>Görünen ad.</returns>
    public static string AnketTuruAdi(AnketTuru tur) => tur switch
    {
        AnketTuru.Ayaktan => "Ayaktan Hasta",
        AnketTuru.Yatan => "Yatan Hasta",
        AnketTuru.Acil => "Acil Servis",
        AnketTuru.Taburculuk => "Taburculuk",
        AnketTuru.Refakatci => "Refakatçi",
        _ => tur.ToString()
    };

    /// <summary>Soru tipi için Türkçe görünen ad döndürür.</summary>
    /// <param name="tip">Soru tipi.</param>
    /// <returns>Görünen ad.</returns>
    public static string SoruTipiAdi(SoruTipi tip) => tip switch
    {
        SoruTipi.Puanlama => "Puanlama",
        SoruTipi.TekSecim => "Tek Seçim",
        SoruTipi.CokluSecim => "Çoklu Seçim",
        SoruTipi.EvetHayir => "Evet / Hayır",
        SoruTipi.AcikUclu => "Açık Uçlu",
        SoruTipi.Nps => "Tavsiye Skoru (NPS)",
        _ => tip.ToString()
    };

    /// <summary>Davet durumu için Türkçe görünen ad döndürür.</summary>
    /// <param name="durum">Davet durumu.</param>
    /// <returns>Görünen ad.</returns>
    public static string DavetDurumuAdi(DavetDurumu durum) => durum switch
    {
        DavetDurumu.Olusturuldu => "Oluşturuldu",
        DavetDurumu.Gonderildi => "Gönderildi",
        DavetDurumu.Acildi => "Açıldı",
        DavetDurumu.KismiTamamlandi => "Kısmi Tamamlandı",
        DavetDurumu.Tamamlandi => "Tamamlandı",
        DavetDurumu.SuresiDoldu => "Süresi Doldu",
        DavetDurumu.Gecersiz => "Geçersiz",
        _ => durum.ToString()
    };

    /// <summary>Gönderim kanalı için Türkçe görünen ad döndürür.</summary>
    /// <param name="kanal">Gönderim kanalı.</param>
    /// <returns>Görünen ad.</returns>
    public static string KanalAdi(GonderimKanali kanal) => kanal switch
    {
        GonderimKanali.Sms => "SMS",
        GonderimKanali.Qr => "QR Kod",
        GonderimKanali.Manuel => "Manuel",
        _ => kanal.ToString()
    };

    /// <summary>Davet durumu için Bootstrap rozet sınıfı döndürür.</summary>
    /// <param name="durum">Davet durumu.</param>
    /// <returns>Bootstrap arka plan sınıfı.</returns>
    public static string DavetDurumuRozet(DavetDurumu durum) => durum switch
    {
        DavetDurumu.Olusturuldu => "bg-secondary",
        DavetDurumu.Gonderildi => "bg-info text-dark",
        DavetDurumu.Acildi => "bg-primary",
        DavetDurumu.KismiTamamlandi => "bg-warning text-dark",
        DavetDurumu.Tamamlandi => "bg-success",
        DavetDurumu.SuresiDoldu => "bg-dark",
        DavetDurumu.Gecersiz => "bg-danger",
        _ => "bg-secondary"
    };

    /// <summary>Kritik geri bildirim kural tipi için Türkçe görünen ad döndürür.</summary>
    /// <param name="tip">Kural tipi.</param>
    /// <returns>Görünen ad.</returns>
    public static string KuralTipiAdi(KuralTipi tip) => tip switch
    {
        KuralTipi.PuanAlti => "Puan Altı",
        KuralTipi.SecenekEslesmesi => "Seçenek Eşleşmesi",
        KuralTipi.AnahtarKelime => "Anahtar Kelime",
        _ => tip.ToString()
    };

    /// <summary>İyileştirme aksiyonu durumu için Türkçe görünen ad döndürür.</summary>
    /// <param name="durum">Aksiyon durumu.</param>
    /// <returns>Görünen ad.</returns>
    public static string AksiyonDurumuAdi(AksiyonDurumu durum) => durum switch
    {
        AksiyonDurumu.Acik => "Açık",
        AksiyonDurumu.DevamEdiyor => "Devam Ediyor",
        AksiyonDurumu.Tamamlandi => "Tamamlandı",
        AksiyonDurumu.Iptal => "İptal",
        _ => durum.ToString()
    };

    /// <summary>İyileştirme aksiyonu durumu için Bootstrap rozet sınıfı döndürür.</summary>
    /// <param name="durum">Aksiyon durumu.</param>
    /// <returns>Bootstrap arka plan sınıfı.</returns>
    public static string AksiyonDurumuRozet(AksiyonDurumu durum) => durum switch
    {
        AksiyonDurumu.Acik => "bg-secondary",
        AksiyonDurumu.DevamEdiyor => "bg-primary",
        AksiyonDurumu.Tamamlandi => "bg-success",
        AksiyonDurumu.Iptal => "bg-dark",
        _ => "bg-secondary"
    };

    /// <summary>İyileştirme aksiyonu önceliği için Türkçe görünen ad döndürür.</summary>
    /// <param name="oncelik">Aksiyon önceliği.</param>
    /// <returns>Görünen ad.</returns>
    public static string AksiyonOnceligiAdi(AksiyonOnceligi oncelik) => oncelik switch
    {
        AksiyonOnceligi.Dusuk => "Düşük",
        AksiyonOnceligi.Orta => "Orta",
        AksiyonOnceligi.Yuksek => "Yüksek",
        AksiyonOnceligi.Kritik => "Kritik",
        _ => oncelik.ToString()
    };

    /// <summary>İyileştirme aksiyonu önceliği için Bootstrap rozet sınıfı döndürür.</summary>
    /// <param name="oncelik">Aksiyon önceliği.</param>
    /// <returns>Bootstrap arka plan sınıfı.</returns>
    public static string AksiyonOnceligiRozet(AksiyonOnceligi oncelik) => oncelik switch
    {
        AksiyonOnceligi.Dusuk => "bg-secondary",
        AksiyonOnceligi.Orta => "bg-info text-dark",
        AksiyonOnceligi.Yuksek => "bg-warning text-dark",
        AksiyonOnceligi.Kritik => "bg-danger",
        _ => "bg-secondary"
    };
}
