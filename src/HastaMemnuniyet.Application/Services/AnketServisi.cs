using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using HastaMemnuniyet.Domain.Interfaces;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Anket, soru ve seçenek yönetimi iş kurallarını uygulayan servis.
/// </summary>
public class AnketServisi : IAnketServisi
{
    private readonly IAnketRepository _anketRepository;
    private readonly IGenericRepository<AnketSorusu> _soruRepository;
    private readonly IGenericRepository<AnketSoruSecenegi> _secenekRepository;

    /// <summary>Yeni bir <see cref="AnketServisi"/> örneği oluşturur.</summary>
    /// <param name="anketRepository">Anket veri erişim bileşeni.</param>
    /// <param name="soruRepository">Soru veri erişim bileşeni.</param>
    /// <param name="secenekRepository">Seçenek veri erişim bileşeni.</param>
    public AnketServisi(
        IAnketRepository anketRepository,
        IGenericRepository<AnketSorusu> soruRepository,
        IGenericRepository<AnketSoruSecenegi> secenekRepository)
    {
        _anketRepository = anketRepository;
        _soruRepository = soruRepository;
        _secenekRepository = secenekRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnketDto>> TumunuGetirAsync()
    {
        var anketler = await _anketRepository.GetAllAsync();
        return anketler.Select(a => HaritalaDto(a, sorulariDahilEt: false)).ToList();
    }

    /// <inheritdoc />
    public async Task<AnketDto?> GetirAsync(int id)
    {
        var anket = await _anketRepository.SorulariylaGetirAsync(id);
        return anket is null ? null : HaritalaDto(anket, sorulariDahilEt: true);
    }

    /// <inheritdoc />
    public async Task<AnketDto> OlusturAsync(AnketOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Ad))
            throw new ArgumentException("Anket adı boş olamaz.", nameof(dto));

        var anket = new Anket
        {
            Ad = dto.Ad.Trim(),
            Aciklama = dto.Aciklama,
            AnketTuru = dto.AnketTuru,
            GizlilikMetni = dto.GizlilikMetni,
            TahminiSureDakika = dto.TahminiSureDakika,
            AktifMi = dto.AktifMi,
            SurumNo = 1
        };

        await _anketRepository.AddAsync(anket);
        await _anketRepository.SaveChangesAsync();
        return HaritalaDto(anket, sorulariDahilEt: false);
    }

    /// <inheritdoc />
    public async Task<AnketDto> GuncelleAsync(AnketDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var anket = await _anketRepository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException($"{dto.Id} kimlikli anket bulunamadı.");

        anket.Ad = dto.Ad.Trim();
        anket.Aciklama = dto.Aciklama;
        anket.AnketTuru = dto.AnketTuru;
        anket.GizlilikMetni = dto.GizlilikMetni;
        anket.TahminiSureDakika = dto.TahminiSureDakika;
        anket.AktifMi = dto.AktifMi;
        anket.GuncellenmeTarihi = DateTime.UtcNow;

        _anketRepository.Update(anket);
        await _anketRepository.SaveChangesAsync();
        return HaritalaDto(anket, sorulariDahilEt: false);
    }

    /// <inheritdoc />
    public async Task SilAsync(int id)
    {
        var anket = await _anketRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"{id} kimlikli anket bulunamadı.");
        _anketRepository.Delete(anket);
        await _anketRepository.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<AnketDto> SurumOlusturAsync(int anketId)
    {
        var kaynak = await _anketRepository.SorulariylaGetirAsync(anketId)
            ?? throw new InvalidOperationException($"{anketId} kimlikli anket bulunamadı.");

        // Yeni sürüm, kaynak anketle aynı sürüm zincirine bağlanır (kök sürüm kimliği korunur).
        var yeniSurum = new Anket
        {
            Ad = kaynak.Ad,
            Aciklama = kaynak.Aciklama,
            AnketTuru = kaynak.AnketTuru,
            GizlilikMetni = kaynak.GizlilikMetni,
            TahminiSureDakika = kaynak.TahminiSureDakika,
            AktifMi = true,
            SurumNo = kaynak.SurumNo + 1,
            AnaSurumAnketId = kaynak.AnaSurumAnketId ?? kaynak.Id
        };

        // Koşullu soru bağlarını yeni sorulara taşıyabilmek için eski sorunun sıra numarasını
        // (anket içinde benzersiz) referans olarak kullanırız.
        var kaynakSoruById = kaynak.Sorular.ToDictionary(s => s.Id);
        var yeniSorular = new List<AnketSorusu>();

        foreach (var eskiSoru in kaynak.Sorular.OrderBy(s => s.SiraNo))
        {
            var yeniSoru = new AnketSorusu
            {
                SoruMetni = eskiSoru.SoruMetni,
                SoruTipi = eskiSoru.SoruTipi,
                SiraNo = eskiSoru.SiraNo,
                ZorunluMu = eskiSoru.ZorunluMu,
                Kategori = eskiSoru.Kategori,
                PuanlamaAltSinir = eskiSoru.PuanlamaAltSinir,
                PuanlamaUstSinir = eskiSoru.PuanlamaUstSinir,
                MaksimumKarakterSayisi = eskiSoru.MaksimumKarakterSayisi,
                KosulDegeri = eskiSoru.KosulDegeri,
                AktifMi = eskiSoru.AktifMi
            };

            foreach (var eskiSecenek in eskiSoru.Secenekler.OrderBy(x => x.SiraNo))
            {
                yeniSoru.Secenekler.Add(new AnketSoruSecenegi
                {
                    MetinDegeri = eskiSecenek.MetinDegeri,
                    SiraNo = eskiSecenek.SiraNo,
                    AktifMi = eskiSecenek.AktifMi
                });
            }

            yeniSurum.Sorular.Add(yeniSoru);
            yeniSorular.Add(yeniSoru);
        }

        kaynak.AktifMi = false;
        kaynak.GuncellenmeTarihi = DateTime.UtcNow;
        _anketRepository.Update(kaynak);

        await _anketRepository.AddAsync(yeniSurum);
        await _anketRepository.SaveChangesAsync();

        // Sorular kaydedildikten sonra yeni kimlikler oluştuğundan koşullu bağları
        // sıra numarası eşleştirmesiyle yeni sorulara yeniden bağlarız.
        var yeniSoruBySira = yeniSurum.Sorular.ToDictionary(s => s.SiraNo);
        var bagGuncellendi = false;
        foreach (var eskiSoru in kaynak.Sorular)
        {
            if (eskiSoru.KosulBagliSoruId is null)
                continue;
            if (!kaynakSoruById.TryGetValue(eskiSoru.KosulBagliSoruId.Value, out var eskiBagliSoru))
                continue;
            if (yeniSoruBySira.TryGetValue(eskiSoru.SiraNo, out var yeniSoru) &&
                yeniSoruBySira.TryGetValue(eskiBagliSoru.SiraNo, out var yeniBagliSoru))
            {
                yeniSoru.KosulBagliSoruId = yeniBagliSoru.Id;
                bagGuncellendi = true;
            }
        }

        if (bagGuncellendi)
            await _anketRepository.SaveChangesAsync();

        return HaritalaDto(yeniSurum, sorulariDahilEt: true);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnketDto>> GecmisSurumleriGetirAsync(int anketId)
    {
        var anket = await _anketRepository.GetByIdAsync(anketId)
            ?? throw new InvalidOperationException($"{anketId} kimlikli anket bulunamadı.");

        // Zincirin kök kimliği: kendi AnaSurumAnketId'si varsa o, yoksa kendi kimliği.
        var kokId = anket.AnaSurumAnketId ?? anket.Id;

        var tumu = await _anketRepository.GetAllAsync();
        var zincir = tumu
            .Where(a => a.Id == kokId || a.AnaSurumAnketId == kokId)
            .OrderByDescending(a => a.SurumNo)
            .Select(a => HaritalaDto(a, sorulariDahilEt: false))
            .ToList();

        return zincir;
    }

    /// <inheritdoc />
    public async Task<AnketSorusuDto> SoruEkleAsync(AnketSorusuOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var anket = await _anketRepository.GetByIdAsync(dto.AnketId)
            ?? throw new InvalidOperationException($"{dto.AnketId} kimlikli anket bulunamadı.");

        var soru = new AnketSorusu
        {
            AnketId = anket.Id,
            SoruMetni = dto.SoruMetni.Trim(),
            SoruTipi = dto.SoruTipi,
            SiraNo = dto.SiraNo,
            ZorunluMu = dto.ZorunluMu,
            Kategori = dto.Kategori,
            PuanlamaAltSinir = dto.PuanlamaAltSinir,
            PuanlamaUstSinir = dto.PuanlamaUstSinir,
            MaksimumKarakterSayisi = dto.MaksimumKarakterSayisi,
            KosulBagliSoruId = dto.KosulBagliSoruId,
            KosulDegeri = string.IsNullOrWhiteSpace(dto.KosulDegeri) ? null : dto.KosulDegeri.Trim(),
            AktifMi = true
        };

        var sira = 1;
        foreach (var secenekMetni in dto.Secenekler.Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            soru.Secenekler.Add(new AnketSoruSecenegi
            {
                MetinDegeri = secenekMetni.Trim(),
                SiraNo = sira++,
                AktifMi = true
            });
        }

        await _soruRepository.AddAsync(soru);
        await _soruRepository.SaveChangesAsync();
        return HaritalaSoruDto(soru);
    }

    /// <inheritdoc />
    public async Task<AnketSorusuDto> SoruGuncelleAsync(AnketSorusuDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var soru = await _soruRepository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException($"{dto.Id} kimlikli soru bulunamadı.");

        soru.SoruMetni = dto.SoruMetni.Trim();
        soru.SoruTipi = dto.SoruTipi;
        soru.SiraNo = dto.SiraNo;
        soru.ZorunluMu = dto.ZorunluMu;
        soru.Kategori = dto.Kategori;
        soru.PuanlamaAltSinir = dto.PuanlamaAltSinir;
        soru.PuanlamaUstSinir = dto.PuanlamaUstSinir;
        soru.MaksimumKarakterSayisi = dto.MaksimumKarakterSayisi;
        soru.KosulBagliSoruId = dto.KosulBagliSoruId;
        soru.KosulDegeri = string.IsNullOrWhiteSpace(dto.KosulDegeri) ? null : dto.KosulDegeri.Trim();
        soru.AktifMi = dto.AktifMi;
        soru.GuncellenmeTarihi = DateTime.UtcNow;

        _soruRepository.Update(soru);
        await _soruRepository.SaveChangesAsync();
        return HaritalaSoruDto(soru);
    }

    /// <inheritdoc />
    public async Task SoruSilAsync(int soruId)
    {
        var soru = await _soruRepository.GetByIdAsync(soruId)
            ?? throw new InvalidOperationException($"{soruId} kimlikli soru bulunamadı.");
        _soruRepository.Delete(soru);
        await _soruRepository.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<AnketSoruSecenegiDto> SecenekEkleAsync(int soruId, string metinDegeri)
    {
        if (string.IsNullOrWhiteSpace(metinDegeri))
            throw new ArgumentException("Seçenek metni boş olamaz.", nameof(metinDegeri));

        var soru = await _soruRepository.GetByIdAsync(soruId)
            ?? throw new InvalidOperationException($"{soruId} kimlikli soru bulunamadı.");

        var mevcutSecenekler = await _secenekRepository.BulAsync(s => s.SoruId == soruId);
        var yeniSira = mevcutSecenekler.Count == 0 ? 1 : mevcutSecenekler.Max(s => s.SiraNo) + 1;

        var secenek = new AnketSoruSecenegi
        {
            SoruId = soru.Id,
            MetinDegeri = metinDegeri.Trim(),
            SiraNo = yeniSira,
            AktifMi = true
        };

        await _secenekRepository.AddAsync(secenek);
        await _secenekRepository.SaveChangesAsync();
        return HaritalaSecenekDto(secenek);
    }

    /// <inheritdoc />
    public async Task SecenekSilAsync(int secenekId)
    {
        var secenek = await _secenekRepository.GetByIdAsync(secenekId)
            ?? throw new InvalidOperationException($"{secenekId} kimlikli seçenek bulunamadı.");
        _secenekRepository.Delete(secenek);
        await _secenekRepository.SaveChangesAsync();
    }

    private static AnketDto HaritalaDto(Anket anket, bool sorulariDahilEt) => new()
    {
        Id = anket.Id,
        Ad = anket.Ad,
        Aciklama = anket.Aciklama,
        AnketTuru = anket.AnketTuru,
        AktifMi = anket.AktifMi,
        SurumNo = anket.SurumNo,
        AnaSurumAnketId = anket.AnaSurumAnketId,
        GizlilikMetni = anket.GizlilikMetni,
        TahminiSureDakika = anket.TahminiSureDakika,
        SoruSayisi = anket.Sorular.Count,
        Sorular = sorulariDahilEt
            ? anket.Sorular.OrderBy(s => s.SiraNo).Select(HaritalaSoruDto).ToList()
            : new List<AnketSorusuDto>()
    };

    private static AnketSorusuDto HaritalaSoruDto(AnketSorusu soru) => new()
    {
        Id = soru.Id,
        AnketId = soru.AnketId,
        SoruMetni = soru.SoruMetni,
        SoruTipi = soru.SoruTipi,
        SiraNo = soru.SiraNo,
        ZorunluMu = soru.ZorunluMu,
        AktifMi = soru.AktifMi,
        Kategori = soru.Kategori,
        PuanlamaAltSinir = soru.PuanlamaAltSinir,
        PuanlamaUstSinir = soru.PuanlamaUstSinir,
        MaksimumKarakterSayisi = soru.MaksimumKarakterSayisi,
        KosulBagliSoruId = soru.KosulBagliSoruId,
        KosulDegeri = soru.KosulDegeri,
        Secenekler = soru.Secenekler.OrderBy(s => s.SiraNo).Select(HaritalaSecenekDto).ToList()
    };

    private static AnketSoruSecenegiDto HaritalaSecenekDto(AnketSoruSecenegi secenek) => new()
    {
        Id = secenek.Id,
        SoruId = secenek.SoruId,
        MetinDegeri = secenek.MetinDegeri,
        SiraNo = secenek.SiraNo,
        AktifMi = secenek.AktifMi
    };
}
