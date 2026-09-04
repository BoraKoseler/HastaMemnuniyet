using ClosedXML.Excel;
using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;

namespace HastaMemnuniyet.Infrastructure.Services;

/// <summary>
/// ClosedXML kullanarak anket yanıtlarını ve dashboard raporlarını
/// Excel (.xlsx) biçiminde dışa aktaran servis.
/// </summary>
public class ExcelDisaAktarmaServisi : IDisaAktarmaServisi
{
    /// <summary>
    /// Verilen anket yanıtlarını Excel çalışma kitabına yazar ve byte dizisi olarak döndürür.
    /// </summary>
    /// <param name="yanitlar">Dışa aktarılacak anket yanıtları.</param>
    /// <returns>Excel dosyasının byte içeriği.</returns>
    public byte[] YanitlariDisaAktar(IReadOnlyList<AnketYanitiDto> yanitlar)
    {
        using var calismaKitabi = new XLWorkbook();
        var sayfa = calismaKitabi.Worksheets.Add("Anket Yanıtları");

        // Başlık satırı
        var basliklar = new[]
        {
            "Yanıt No", "Anket", "Hastane", "Birim", "Doktor", "Kanal",
            "IP Adresi", "Başlama Tarihi", "Tamamlanma Tarihi", "Geçerli mi",
            "Ortalama Puan", "Cevap Sayısı"
        };

        for (var sutun = 0; sutun < basliklar.Length; sutun++)
        {
            sayfa.Cell(1, sutun + 1).Value = basliklar[sutun];
        }

        var baslikAraligi = sayfa.Range(1, 1, 1, basliklar.Length);
        baslikAraligi.Style.Font.Bold = true;
        baslikAraligi.Style.Fill.BackgroundColor = XLColor.LightBlue;

        // Veri satırları
        var satir = 2;
        foreach (var yanit in yanitlar)
        {
            var puanlar = yanit.Cevaplar
                .Where(c => c.PuanDegeri.HasValue)
                .Select(c => c.PuanDegeri!.Value)
                .ToList();
            var ortalamaPuan = puanlar.Count > 0 ? Math.Round(puanlar.Average(), 2) : (double?)null;

            sayfa.Cell(satir, 1).Value = yanit.Id;
            sayfa.Cell(satir, 2).Value = yanit.AnketAdi ?? string.Empty;
            sayfa.Cell(satir, 3).Value = yanit.HastaneAdi ?? string.Empty;
            sayfa.Cell(satir, 4).Value = yanit.BirimAdi ?? string.Empty;
            sayfa.Cell(satir, 5).Value = yanit.DoktorAdi ?? string.Empty;
            sayfa.Cell(satir, 6).Value = yanit.KanalAdi ?? string.Empty;
            sayfa.Cell(satir, 7).Value = yanit.IpAdresi ?? string.Empty;
            sayfa.Cell(satir, 8).Value = yanit.BaslamaTarihi;
            sayfa.Cell(satir, 8).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
            if (yanit.TamamlanmaTarihi.HasValue)
            {
                sayfa.Cell(satir, 9).Value = yanit.TamamlanmaTarihi.Value;
                sayfa.Cell(satir, 9).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
            }
            sayfa.Cell(satir, 10).Value = yanit.GecerliMi ? "Evet" : "Hayır";
            if (ortalamaPuan.HasValue)
            {
                sayfa.Cell(satir, 11).Value = ortalamaPuan.Value;
            }
            sayfa.Cell(satir, 12).Value = yanit.Cevaplar.Count;
            satir++;
        }

        sayfa.Columns().AdjustToContents();

        using var akis = new MemoryStream();
        calismaKitabi.SaveAs(akis);
        return akis.ToArray();
    }

    /// <summary>
    /// Dashboard istatistiklerini ve puan dağılımlarını Excel çalışma kitabına yazar
    /// ve byte dizisi olarak döndürür.
    /// </summary>
    /// <param name="dashboard">Dışa aktarılacak dashboard istatistikleri.</param>
    /// <returns>Excel dosyasının byte içeriği.</returns>
    public byte[] RaporuDisaAktar(DashboardDto dashboard)
    {
        using var calismaKitabi = new XLWorkbook();

        // Özet sayfası
        var ozet = calismaKitabi.Worksheets.Add("Özet");
        ozet.Cell(1, 1).Value = "Hasta Memnuniyet Raporu";
        ozet.Cell(1, 1).Style.Font.Bold = true;
        ozet.Cell(1, 1).Style.Font.FontSize = 14;

        ozet.Cell(2, 1).Value = "Dönem";
        ozet.Cell(2, 2).Value = $"{dashboard.BaslangicTarihi:dd.MM.yyyy} - {dashboard.BitisTarihi:dd.MM.yyyy}";

        var ozetSatirlari = new (string Etiket, object Deger)[]
        {
            ("Toplam Aktif Anket", dashboard.ToplamAnketSayisi),
            ("Toplam Davet", dashboard.ToplamDavetSayisi),
            ("Toplam Yanıt", dashboard.ToplamYanitSayisi),
            ("Yanıt Oranı (%)", Math.Round(dashboard.YanitOrani, 2)),
            ("Ortalama Puan", Math.Round(dashboard.OrtalamaPuan, 2)),
            ("Kritik Geri Bildirim", dashboard.KritikGeriBildirimSayisi),
            ("Açık Aksiyon", dashboard.AcikAksiyonSayisi),
            ("Gecikmiş Aksiyon", dashboard.GecikmisAksiyonSayisi)
        };

        var satir = 4;
        foreach (var (etiket, deger) in ozetSatirlari)
        {
            ozet.Cell(satir, 1).Value = etiket;
            ozet.Cell(satir, 1).Style.Font.Bold = true;
            ozet.Cell(satir, 2).Value = XLCellValue.FromObject(deger);
            satir++;
        }

        ozet.Columns().AdjustToContents();

        // Hastane puanları sayfası
        var hastaneSayfasi = calismaKitabi.Worksheets.Add("Hastane Puanları");
        YazBaslik(hastaneSayfasi, new[] { "Hastane", "Ortalama Puan", "Yanıt Sayısı" });
        var hSatir = 2;
        foreach (var hastane in dashboard.HastanePuanlari)
        {
            hastaneSayfasi.Cell(hSatir, 1).Value = hastane.HastaneAdi;
            hastaneSayfasi.Cell(hSatir, 2).Value = Math.Round(hastane.OrtalamaPuan, 2);
            hastaneSayfasi.Cell(hSatir, 3).Value = hastane.YanitSayisi;
            hSatir++;
        }
        hastaneSayfasi.Columns().AdjustToContents();

        // Birim puanları sayfası
        var birimSayfasi = calismaKitabi.Worksheets.Add("Birim Puanları");
        YazBaslik(birimSayfasi, new[] { "Birim", "Hastane", "Ortalama Puan", "Yanıt Sayısı" });
        var bSatir = 2;
        foreach (var birim in dashboard.BirimPuanlari)
        {
            birimSayfasi.Cell(bSatir, 1).Value = birim.BirimAdi;
            birimSayfasi.Cell(bSatir, 2).Value = birim.HastaneAdi ?? string.Empty;
            birimSayfasi.Cell(bSatir, 3).Value = Math.Round(birim.OrtalamaPuan, 2);
            birimSayfasi.Cell(bSatir, 4).Value = birim.YanitSayisi;
            bSatir++;
        }
        birimSayfasi.Columns().AdjustToContents();

        using var akis = new MemoryStream();
        calismaKitabi.SaveAs(akis);
        return akis.ToArray();
    }

    /// <summary>
    /// Bir doktorun performans karnesini Excel çalışma kitabına yazar ve byte dizisi olarak döndürür.
    /// Özet, soru bazlı ortalamalar ve güçlü/zayıf alanlar ayrı sayfalarda sunulur.
    /// </summary>
    /// <param name="karne">Dışa aktarılacak doktor karnesi.</param>
    /// <returns>Excel dosyasının byte içeriği.</returns>
    public byte[] DoktorKarnesiDisaAktar(DoktorKarnesiDto karne)
    {
        using var calismaKitabi = new XLWorkbook();

        // Özet sayfası
        var ozet = calismaKitabi.Worksheets.Add("Özet");
        ozet.Cell(1, 1).Value = "Doktor Performans Karnesi";
        ozet.Cell(1, 1).Style.Font.Bold = true;
        ozet.Cell(1, 1).Style.Font.FontSize = 14;

        var ozetSatirlari = new (string Etiket, object Deger)[]
        {
            ("Doktor", karne.DoktorAdSoyad),
            ("Unvan", karne.Unvan ?? "-"),
            ("Birimler", karne.Birimler.Count > 0 ? string.Join(", ", karne.Birimler) : "-"),
            ("Dönem", $"{karne.BaslangicTarihi:dd.MM.yyyy} - {karne.BitisTarihi:dd.MM.yyyy}"),
            ("Toplam Yanıt", karne.ToplamYanit),
            ("Ortalama Puan", Math.Round(karne.OrtalamaPuan, 2)),
            ("NPS Skoru", karne.NpsSkoru.HasValue ? karne.NpsSkoru.Value : "-"),
            ("Önceki Dönem Ortalama", karne.OncekiDonemOrtalamaPuan.HasValue ? Math.Round(karne.OncekiDonemOrtalamaPuan.Value, 2) : "-"),
            ("Trend (%)", karne.TrendYuzdesi.HasValue ? Math.Round(karne.TrendYuzdesi.Value, 2) : "-"),
            ("Düşük Örneklem Uyarısı", karne.DusukOrneklemUyarisi ? "Evet" : "Hayır")
        };

        var satir = 3;
        foreach (var (etiket, deger) in ozetSatirlari)
        {
            ozet.Cell(satir, 1).Value = etiket;
            ozet.Cell(satir, 1).Style.Font.Bold = true;
            ozet.Cell(satir, 2).Value = XLCellValue.FromObject(deger);
            satir++;
        }
        ozet.Columns().AdjustToContents();

        // Soru bazlı ortalamalar sayfası
        var soruSayfasi = calismaKitabi.Worksheets.Add("Soru Bazlı Ortalamalar");
        YazBaslik(soruSayfasi, new[] { "Soru", "Kategori", "Ortalama Puan", "Yanıt Sayısı" });
        var sSatir = 2;
        foreach (var soru in karne.SoruBazliOrtalamalar)
        {
            soruSayfasi.Cell(sSatir, 1).Value = soru.SoruMetni;
            soruSayfasi.Cell(sSatir, 2).Value = soru.Kategori ?? string.Empty;
            soruSayfasi.Cell(sSatir, 3).Value = Math.Round(soru.Ortalama, 2);
            soruSayfasi.Cell(sSatir, 4).Value = soru.YanitSayisi;
            sSatir++;
        }
        soruSayfasi.Columns().AdjustToContents();

        // Güçlü ve zayıf alanlar sayfası
        var alanSayfasi = calismaKitabi.Worksheets.Add("Güçlü ve Zayıf Alanlar");
        alanSayfasi.Cell(1, 1).Value = "Güçlü Alanlar";
        alanSayfasi.Cell(1, 1).Style.Font.Bold = true;
        alanSayfasi.Cell(2, 1).Value = "Soru";
        alanSayfasi.Cell(2, 2).Value = "Ortalama Puan";
        var gStil = alanSayfasi.Range(2, 1, 2, 2);
        gStil.Style.Font.Bold = true;
        gStil.Style.Fill.BackgroundColor = XLColor.LightGreen;
        var aSatir = 3;
        foreach (var alan in karne.GucluAlanlar)
        {
            alanSayfasi.Cell(aSatir, 1).Value = alan.SoruMetni;
            alanSayfasi.Cell(aSatir, 2).Value = Math.Round(alan.Ortalama, 2);
            aSatir++;
        }

        aSatir++;
        alanSayfasi.Cell(aSatir, 1).Value = "Zayıf Alanlar (İyileştirme Gerektiren)";
        alanSayfasi.Cell(aSatir, 1).Style.Font.Bold = true;
        aSatir++;
        alanSayfasi.Cell(aSatir, 1).Value = "Soru";
        alanSayfasi.Cell(aSatir, 2).Value = "Ortalama Puan";
        var zStil = alanSayfasi.Range(aSatir, 1, aSatir, 2);
        zStil.Style.Font.Bold = true;
        zStil.Style.Fill.BackgroundColor = XLColor.LightSalmon;
        aSatir++;
        foreach (var alan in karne.ZayifAlanlar)
        {
            alanSayfasi.Cell(aSatir, 1).Value = alan.SoruMetni;
            alanSayfasi.Cell(aSatir, 2).Value = Math.Round(alan.Ortalama, 2);
            aSatir++;
        }
        alanSayfasi.Columns().AdjustToContents();

        using var akis = new MemoryStream();
        calismaKitabi.SaveAs(akis);
        return akis.ToArray();
    }

    /// <summary>
    /// Verilen çalışma sayfasının ilk satırına kalın ve renkli başlık hücreleri yazar.
    /// </summary>
    /// <param name="sayfa">Başlık yazılacak çalışma sayfası.</param>
    /// <param name="basliklar">Başlık metinleri.</param>
    private static void YazBaslik(IXLWorksheet sayfa, string[] basliklar)
    {
        for (var sutun = 0; sutun < basliklar.Length; sutun++)
        {
            sayfa.Cell(1, sutun + 1).Value = basliklar[sutun];
        }
        var aralik = sayfa.Range(1, 1, 1, basliklar.Length);
        aralik.Style.Font.Bold = true;
        aralik.Style.Fill.BackgroundColor = XLColor.LightBlue;
    }
}
