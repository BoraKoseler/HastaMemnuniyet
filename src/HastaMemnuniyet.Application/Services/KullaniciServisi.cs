using HastaMemnuniyet.Application.DTOs;
using HastaMemnuniyet.Application.Interfaces;
using HastaMemnuniyet.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HastaMemnuniyet.Application.Services;

/// <summary>
/// Kullanıcı ve rol yönetimi iş kurallarını ASP.NET Core Identity üzerinden uygulayan servis.
/// </summary>
public class KullaniciServisi : IKullaniciServisi
{
    private readonly UserManager<UygulamaKullanicisi> _kullaniciYoneticisi;
    private readonly RoleManager<IdentityRole> _rolYoneticisi;

    /// <summary>Yeni bir <see cref="KullaniciServisi"/> örneği oluşturur.</summary>
    /// <param name="kullaniciYoneticisi">Identity kullanıcı yöneticisi.</param>
    /// <param name="rolYoneticisi">Identity rol yöneticisi.</param>
    public KullaniciServisi(
        UserManager<UygulamaKullanicisi> kullaniciYoneticisi,
        RoleManager<IdentityRole> rolYoneticisi)
    {
        _kullaniciYoneticisi = kullaniciYoneticisi;
        _rolYoneticisi = rolYoneticisi;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<KullaniciDto>> TumunuGetirAsync()
    {
        var kullanicilar = _kullaniciYoneticisi.Users.ToList();
        var sonuc = new List<KullaniciDto>();
        foreach (var kullanici in kullanicilar)
        {
            sonuc.Add(await HaritalaDtoAsync(kullanici));
        }
        return sonuc;
    }

    /// <inheritdoc />
    public async Task<KullaniciDto?> GetirAsync(string id)
    {
        var kullanici = await _kullaniciYoneticisi.FindByIdAsync(id);
        return kullanici is null ? null : await HaritalaDtoAsync(kullanici);
    }

    /// <inheritdoc />
    public async Task<KullaniciDto> OlusturAsync(KullaniciOlusturDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var kullanici = new UygulamaKullanicisi
        {
            UserName = dto.Eposta,
            Email = dto.Eposta,
            Ad = dto.Ad.Trim(),
            Soyad = dto.Soyad.Trim(),
            AktifMi = dto.AktifMi,
            EmailConfirmed = true
        };

        var sonuc = await _kullaniciYoneticisi.CreateAsync(kullanici, dto.Parola);
        if (!sonuc.Succeeded)
        {
            var hatalar = string.Join("; ", sonuc.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Kullanıcı oluşturulamadı: {hatalar}");
        }

        await RolAtaAsync(kullanici.Id, dto.Rol);
        return await HaritalaDtoAsync(kullanici);
    }

    /// <inheritdoc />
    public async Task RolAtaAsync(string kullaniciId, string rol)
    {
        var kullanici = await _kullaniciYoneticisi.FindByIdAsync(kullaniciId)
            ?? throw new InvalidOperationException($"{kullaniciId} kimlikli kullanıcı bulunamadı.");

        if (!await _rolYoneticisi.RoleExistsAsync(rol))
            throw new InvalidOperationException($"'{rol}' rolü tanımlı değil.");

        if (!await _kullaniciYoneticisi.IsInRoleAsync(kullanici, rol))
        {
            var sonuc = await _kullaniciYoneticisi.AddToRoleAsync(kullanici, rol);
            if (!sonuc.Succeeded)
            {
                var hatalar = string.Join("; ", sonuc.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Rol atanamadı: {hatalar}");
            }
        }
    }

    /// <inheritdoc />
    public async Task RolDegistirAsync(string kullaniciId, string yeniRol)
    {
        var kullanici = await _kullaniciYoneticisi.FindByIdAsync(kullaniciId)
            ?? throw new InvalidOperationException($"{kullaniciId} kimlikli kullanıcı bulunamadı.");

        if (!await _rolYoneticisi.RoleExistsAsync(yeniRol))
            throw new InvalidOperationException($"'{yeniRol}' rolü tanımlı değil.");

        var mevcutRoller = await _kullaniciYoneticisi.GetRolesAsync(kullanici);
        if (mevcutRoller.Count > 0)
        {
            var kaldirmaSonucu = await _kullaniciYoneticisi.RemoveFromRolesAsync(kullanici, mevcutRoller);
            if (!kaldirmaSonucu.Succeeded)
            {
                var hatalar = string.Join("; ", kaldirmaSonucu.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Mevcut roller kaldırılamadı: {hatalar}");
            }
        }

        var eklemeSonucu = await _kullaniciYoneticisi.AddToRoleAsync(kullanici, yeniRol);
        if (!eklemeSonucu.Succeeded)
        {
            var hatalar = string.Join("; ", eklemeSonucu.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Rol atanamadı: {hatalar}");
        }
    }

    /// <inheritdoc />
    public async Task AktiflikDegistirAsync(string kullaniciId, bool aktifMi)
    {
        var kullanici = await _kullaniciYoneticisi.FindByIdAsync(kullaniciId)
            ?? throw new InvalidOperationException($"{kullaniciId} kimlikli kullanıcı bulunamadı.");
        kullanici.AktifMi = aktifMi;
        await _kullaniciYoneticisi.UpdateAsync(kullanici);
    }

    private async Task<KullaniciDto> HaritalaDtoAsync(UygulamaKullanicisi kullanici)
    {
        var roller = await _kullaniciYoneticisi.GetRolesAsync(kullanici);
        return new KullaniciDto
        {
            Id = kullanici.Id,
            Eposta = kullanici.Email ?? string.Empty,
            Ad = kullanici.Ad,
            Soyad = kullanici.Soyad,
            AktifMi = kullanici.AktifMi,
            Roller = roller.ToList()
        };
    }
}
