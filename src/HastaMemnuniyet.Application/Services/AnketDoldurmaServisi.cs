using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Enums;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Anonim anket doldurma sürecini (token doğrulama, anket getirme, yanıt kaydetme) uygulayan servis.
/// </summary>
public class AnketDoldurmaServisi : IAnketDoldurmaServisi
{
    private readonly IAnketDavetiRepository _davetRepository;
    private readonly IAnketRepository _anketRepository;
    private readonly IAnketYanitiRepository _yanitRepository;
    private readonly IKritikGeriBildirimMotoru _kritikMotoru;
    private readonly IKritikGeriBildirimRepository _kritikRepository;

    /// <summary>Yeni bir <see cref="AnketDoldurmaServisi"/> örneği oluşturur.</summary>
    /// <param name="davetRepository">Davet veri erişim bileşeni.</param>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="yanitRepository">Yanıt veri erişim bileşeni.</param>
    /// <param name="kritikMotoru">Kritik geri bildirim değerlendirme motoru.</param>
    /// <param name="kritikRepository">Kritik geri bildirim veri erişim bileşeni.</param>
    public AnketDoldurmaServisi(
        IAnketDavetiRepository davetRepository,
        IAnketRepository anketRepository,
        IAnketYanitiRepository yanitRepository,
        IKritikGeriBildirimMotoru kritikMotoru,
        IKritikGeriBildirimRepository kritikRepository)
    {
        _davetRepository = davetRepository;
        _anketRepository = anketRepository;
        _yanitRepository = yanitRepository;
        _kritikMotoru = kritikMotoru;
        _kritikRepository = kritikRepository;
    }

    /// <inheritdoc />
    public async Task<bool> TokenGecerliMiAsync(string token)
    {
        var davet = await _davetRepository.GetByTokenAsync(token);
        return DavetKullanilabilirMi(davet);
    }

    /// <inheritdoc />
    public async Task<AnketDto?> AnketBilgileriniGetirAsync(string token)
    {
        var davet = await _davetRepository.GetByTokenAsync(token);
        if (!DavetKullanilabilirMi(davet))
            return null;

        var anket = await _anketRepository.SorulariylaGetirAsync(davet!.AnketId);
        if (anket is null)
            return null;

        return new AnketDto
        {
            Id = anket.Id,
            Ad = anket.Ad,
            Aciklama = anket.Aciklama,
            AnketTuru = anket.AnketTuru,
            AktifMi = anket.AktifMi,
            SurumNo = anket.SurumNo,
            GizlilikMetni = anket.GizlilikMetni,
            TahminiSureDakika = anket.TahminiSureDakika,
            SoruSayisi = anket.Sorular.Count(s => s.AktifMi),
            Sorular = anket.Sorular
                .Where(s => s.AktifMi)
                .OrderBy(s => s.SiraNo)
                .Select(s => new AnketSorusuDto
                {
                    Id = s.Id,
                    AnketId = s.AnketId,
                    SoruMetni = s.SoruMetni,
                    SoruTipi = s.SoruTipi,
                    SiraNo = s.SiraNo,
                    ZorunluMu = s.ZorunluMu,
                    AktifMi = s.AktifMi,
                    Kategori = s.Kategori,
                    PuanlamaAltSinir = s.PuanlamaAltSinir,
                    PuanlamaUstSinir = s.PuanlamaUstSinir,
                    MaksimumKarakterSayisi = s.MaksimumKarakterSayisi,
                    KosulBagliSoruId = s.KosulBagliSoruId,
                    KosulDegeri = s.KosulDegeri,
                    Secenekler = s.Secenekler
                        .Where(o => o.AktifMi)
                        .OrderBy(o => o.SiraNo)
                        .Select(o => new AnketSoruSecenegiDto
                        {
                            Id = o.Id,
                            SoruId = o.SoruId,
                            MetinDegeri = o.MetinDegeri,
                            SiraNo = o.SiraNo,
                            AktifMi = o.AktifMi
                        }).ToList()
                }).ToList()
        };
    }

    /// <inheritdoc />
    public async Task<int> YanitKaydetAsync(AnketDoldurDto dto, string? ipAdresi, string? kullaniciAjan)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var davet = await _davetRepository.GetByTokenAsync(dto.Token);
        if (!DavetKullanilabilirMi(davet))
            throw new InvalidOperationException("Davet geçersiz veya süresi dolmuş.");

        var simdi = DateTime.UtcNow;
        var yanit = new AnketYaniti
        {
            DavetId = davet!.Id,
            AnketId = davet.AnketId,
            HastaneId = davet.HastaneId,
            BirimId = davet.BirimId,
            DoktorId = davet.DoktorId,
            BaslamaTarihi = simdi,
            TamamlanmaTarihi = simdi,
            IpAdresi = ipAdresi,
            KullaniciAjan = kullaniciAjan,
            GecerliMi = true
        };

        foreach (var cevap in dto.Cevaplar)
        {
            yanit.Cevaplar.Add(new AnketCevabi
            {
                SoruId = cevap.SoruId,
                PuanDegeri = cevap.PuanDegeri,
                SecenekId = cevap.SecenekId,
                MetinDegeri = cevap.MetinDegeri,
                BoolDegeri = cevap.BoolDegeri,
                SeciliSecenekIdleri = cevap.SeciliSecenekIdleri.Count > 0
                    ? string.Join(",", cevap.SeciliSecenekIdleri)
                    : null,
                YanitTarihi = simdi
            });
        }

        await _yanitRepository.AddAsync(yanit);
        await _yanitRepository.SaveChangesAsync();

        davet.Durum = DavetDurumu.Tamamlandi;
        davet.GuncellenmeTarihi = simdi;
        _davetRepository.Update(davet);
        await _davetRepository.SaveChangesAsync();

        // Yanıtı kritik geri bildirim kurallarına göre değerlendir ve tetiklenen kayıtları sakla.
        await KritikGeriBildirimleriDegerlendirAsync(yanit);

        return yanit.Id;
    }

    /// <summary>Kaydedilen yanıtı kritik geri bildirim motoruyla değerlendirip tetiklenen kayıtları veritabanına yazar.</summary>
    /// <param name="yanit">Değerlendirilecek, kaydedilmiş anket yanıtı.</param>
    private async Task KritikGeriBildirimleriDegerlendirAsync(AnketYaniti yanit)
    {
        var kritikler = await _kritikMotoru.DegerlendiAsync(yanit, yanit.Cevaplar.ToList());
        if (kritikler.Count == 0)
            return;

        foreach (var kritik in kritikler)
        {
            await _kritikRepository.AddAsync(kritik);
        }
        await _kritikRepository.SaveChangesAsync();
    }

    /// <summary>Davetin doldurulmaya uygun (mevcut, süresi dolmamış ve tamamlanmamış) olup olmadığını denetler.</summary>
    /// <param name="davet">Denetlenecek davet.</param>
    /// <returns>Davet kullanılabilir ise true.</returns>
    private static bool DavetKullanilabilirMi(AnketDaveti? davet)
    {
        if (davet is null)
            return false;
        if (davet.Durum is DavetDurumu.Tamamlandi or DavetDurumu.SuresiDoldu or DavetDurumu.Gecersiz)
            return false;
        return davet.SonGecerlilikTarihi >= DateTime.UtcNow;
    }
}
