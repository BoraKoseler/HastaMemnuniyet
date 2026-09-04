# Hasta Memnuniyet Anketi Yönetim Sistemi — Mimari Tasarım Dokümanı

## 1. Genel Bakış

Hastaların / refakatçilerin hastane, birim ve doktoru değerlendirebildiği; SMS bağlantısı veya QR kod ile ankete erişebildiği; sonuçların yetkili yöneticiler tarafından raporlanıp aksiyona dönüştürülebildiği bir web uygulamasıdır.

**Teknoloji yığını:** ASP.NET Core 8 Razor Pages · C# · EF Core 8 · SQL Server · ASP.NET Core Identity · Bootstrap 5

---

## 2. Katmanlı Mimari (Clean Architecture)

```
┌─────────────────────────────────────────────────┐
│                  Web (UI) Katmanı                │
│  Razor Pages · PageModel · ViewModel · Auth      │
│  Filters · TagHelpers · wwwroot                  │
├─────────────────────────────────────────────────┤
│              Application Katmanı                 │
│  Servis Arayüzleri · DTO · Use-Case Servisleri   │
│  Doğrulama (Validation) · Mapping                │
├─────────────────────────────────────────────────┤
│             Infrastructure Katmanı               │
│  EF Core DbContext · Repository Impl.            │
│  Identity Config · Fake SMS · QR Üretimi         │
│  Seed Data · Dışa Aktarma                        │
├─────────────────────────────────────────────────┤
│                Domain Katmanı                    │
│  Entity · Enum · Value Object · Domain Service   │
│  Repository Arayüzleri · Domain Event (hazırlık) │
└─────────────────────────────────────────────────┘
```

**Bağımlılık yönü:** Web → Application → Domain ← Infrastructure

### SOLID Uyum Notları

| Prensip | Uygulama |
|---------|----------|
| **SRP** | Her servis tek bir use-case grubuna odaklanır (AnketServisi, DavetServisi, RaporServisi vb.) |
| **OCP** | ISmsSender, ISoruTipiIsleyici, IGonderimKanali gibi arayüzler sayesinde yeni sağlayıcı/tip eklemek mevcut kodu değiştirmez |
| **LSP** | Tüm arayüz implementasyonları birbirinin yerine güvenle kullanılabilir |
| **ISP** | Büyük arayüzler yerine küçük odaklı arayüzler: IAnketOkumaServisi / IAnketYazmaServisi gibi |
| **DIP** | PageModel'ler ve Application servisleri sadece arayüzlere bağımlıdır; somut altyapı DI ile enjekte edilir |

---

## 3. Proje Klasör Yapısı

```
HastaMemnuniyet/
├── src/
│   ├── HastaMemnuniyet.Domain/              # Entity, Enum, Interface, Value Object
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Interfaces/
│   │   └── ValueObjects/
│   │
│   ├── HastaMemnuniyet.Application/         # Servis arayüzleri, DTO, Use-Case servisleri
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   │
│   ├── HastaMemnuniyet.Infrastructure/      # EF Core, Identity, Repository, SMS, QR
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/              # EF Fluent API
│   │   │   ├── Migrations/
│   │   │   └── Seed/
│   │   ├── Repositories/
│   │   ├── Services/                        # SMS, QR impl.
│   │   └── Identity/
│   │
│   └── HastaMemnuniyet.Web/                 # Razor Pages UI
│       ├── Pages/
│       │   ├── Anket/                       # Anonim anket doldurma
│       │   ├── Yonetim/
│       │   │   ├── Hastaneler/
│       │   │   ├── Birimler/
│       │   │   ├── Doktorlar/
│       │   │   ├── Anketler/
│       │   │   ├── Davetler/
│       │   │   ├── Kullanicilar/
│       │   │   └── SistemAyarlari/
│       │   ├── Dashboard/
│       │   ├── Raporlar/
│       │   ├── Hesap/                       # Login, Logout, Profil
│       │   └── Shared/
│       ├── ViewModels/
│       ├── Filters/
│       ├── TagHelpers/
│       ├── wwwroot/
│       └── Program.cs
│
├── tests/
│   └── HastaMemnuniyet.Tests/               # xUnit birim testleri
│
├── docs/
│   ├── ARCHITECTURE.md
│   └── TRACEABILITY.md
│
├── HastaMemnuniyet.sln
└── README.md
```

---

## 4. Veri Modeli (ER — Metinsel)

### 4.1 Kurum Yapısı

```
Hospital (Hastane)
├── Id (int, PK)
├── Ad (string, zorunlu)
├── Kod (string, benzersiz)
├── Adres (string)
├── Telefon (string)
├── AktifMi (bool)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

Department (Birim)
├── Id (int, PK)
├── HastaneId (int, FK → Hospital)
├── Ad (string, zorunlu)
├── Kod (string)
├── AktifMi (bool)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

Doctor (Doktor)
├── Id (int, PK)
├── Ad (string, zorunlu)
├── Soyad (string, zorunlu)
├── Unvan (string)
├── AktifMi (bool)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

DoctorDepartment (Doktor-Birim İlişkisi, M:N)
├── DoktorId (int, FK → Doctor)
├── BirimId (int, FK → Department)
└── AktifMi (bool)
```

### 4.2 Kullanıcı ve Yetki

```
ApplicationUser (IdentityUser genişletmesi)
├── Id (string, PK)
├── Ad (string)
├── Soyad (string)
├── AktifMi (bool)
└── OlusturulmaTarihi (DateTime)

UserHospitalScope (Kullanıcı-Hastane Kapsamı)
├── Id (int, PK)
├── KullaniciId (string, FK → ApplicationUser)
└── HastaneId (int, FK → Hospital)

UserDepartmentScope (Kullanıcı-Birim Kapsamı)
├── Id (int, PK)
├── KullaniciId (string, FK → ApplicationUser)
└── BirimId (int, FK → Department)
```

### 4.3 Anket Yapısı

```
Survey (Anket)
├── Id (int, PK)
├── Ad (string, zorunlu)
├── Aciklama (string)
├── AnketTuru (enum: Ayaktan, Yatan, Acil, Taburculuk, Refakatci)
├── AktifMi (bool)
├── SurumaNo (int, varsayılan 1)
├── GizlilikMetni (string)
├── TahminiSureDakika (int)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

SurveyQuestion (Anket Sorusu)
├── Id (int, PK)
├── AnketId (int, FK → Survey)
├── SoruMetni (string, zorunlu)
├── SoruTipi (enum: Puanlama, TekSecim, CokluSecim, EvetHayir, AcikUclu)
├── SiraNo (int)
├── ZorunluMu (bool)
├── AktifMi (bool)
├── Kategori (string, nullable — soru kategorisi)
├── PuanlamaAltSinir (int?, Puanlama tipi için)
├── PuanlamaUstSinir (int?, Puanlama tipi için)
├── MaksimumKarakterSayisi (int?, AcikUclu tipi için)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

SurveyQuestionOption (Soru Seçeneği)
├── Id (int, PK)
├── SoruId (int, FK → SurveyQuestion)
├── MetinDegeri (string, zorunlu)
├── SiraNo (int)
└── AktifMi (bool)
```

### 4.4 Anket Davet ve Yanıt

```
SurveyInvitation (Anket Daveti)
├── Id (int, PK)
├── AnketId (int, FK → Survey)
├── HastaneId (int, FK → Hospital)
├── BirimId (int?, FK → Department)
├── DoktorId (int?, FK → Doctor)
├── Token (string, benzersiz, tahmin edilemez)
├── TelefonHash (string — telefon numarasının hash'i)
├── GonderimKanali (enum: SMS, QR, Manuel)
├── Durum (enum: Olusturuldu, Gonderildi, Acildi, KismiTamamlandi, Tamamlandi, SuresiDoldu, Gecersiz)
├── HizmetTarihi (DateTime?)
├── SonGecerlilikTarihi (DateTime)
├── OlusturanKullaniciId (string, FK → ApplicationUser)
├── OlusturulmaTarihi (DateTime)
└── GuncellenmeTarihi (DateTime?)

SurveyResponse (Anket Yanıtı)
├── Id (int, PK)
├── DavetId (int, FK → SurveyInvitation)
├── AnketId (int, FK → Survey)
├── HastaneId (int, FK → Hospital)
├── BirimId (int?, FK → Department)
├── DoktorId (int?, FK → Doctor)
├── BaslamaTarihi (DateTime)
├── TamamlanmaTarihi (DateTime?)
├── IpAdresi (string)
├── KullaniciAjan (string)
└── GecerliMi (bool)

SurveyAnswer (Anket Cevabı)
├── Id (int, PK)
├── YanitId (int, FK → SurveyResponse)
├── SoruId (int, FK → SurveyQuestion)
├── PuanDegeri (int?)
├── SecenekId (int?, FK → SurveyQuestionOption)
├── MetinDegeri (string?)
├── BoolDegeri (bool?)
├── YanitTarihi (DateTime)
└── SeciliSecenekIdleri (string? — çoklu seçim için virgüllü Id listesi)
```

### 4.5 SMS Kayıt

```
SmsDeliveryLog (SMS Gönderim Kaydı)
├── Id (int, PK)
├── DavetId (int, FK → SurveyInvitation)
├── TelefonHash (string)
├── SmsDurumu (enum: Kuyrukta, Gonderildi, Basarisiz, Ulastirildi)
├── SmsYaniti (string?)
├── GonderimTarihi (DateTime?)
└── OlusturulmaTarihi (DateTime)
```

### 4.6 Sistem

```
SystemSetting (Sistem Ayarı)
├── Id (int, PK)
├── Anahtar (string, benzersiz)
├── Deger (string)
├── Aciklama (string?)
├── Kategori (string?)
└── GuncellenmeTarihi (DateTime?)

AuditLog (Denetim Kaydı)
├── Id (long, PK)
├── KullaniciId (string?)
├── KullaniciAdi (string?)
├── Islem (string)
├── Tablo (string?)
├── KayitId (string?)
├── EskiDeger (string?, JSON)
├── YeniDeger (string?, JSON)
├── IpAdresi (string?)
├── Tarih (DateTime)
└── Detay (string?)
```

---

## 5. Kullanıcı Rolleri (Faz 1)

| Rol | Kod | Kapsam |
|-----|-----|--------|
| Sistem Yöneticisi | `Admin` | Tüm sistem; kullanıcı/rol/hastane/birim/doktor/ayar yönetimi |
| Kalite Birimi | `KaliteBirimi` | Tüm hastaneler; anket, soru, rapor, dashboard |

> **Faz 2+** ile eklenecek: `BirimYoneticisi`, `UstYonetim`, `Hasta/Refakatci` (anonim token erişimi — rol değil).

---

## 6. Ekran / Sayfa Listesi (Faz 1)

### 6.1 Anonim (Giriş Gerektirmeyen)
| # | Sayfa | Yol | Açıklama |
|---|-------|-----|----------|
| 1 | Anket Giriş | `/Anket/{token}` | Gizlilik metni + başlama butonu |
| 2 | Anket Doldurma | `/Anket/{token}/Doldur` | Soruları mobil uyumlu göster |
| 3 | Teşekkür | `/Anket/{token}/Tesekkur` | Tamamlandı bildirimi |
| 4 | Süresi Dolmuş | `/Anket/SuresiDolmus` | Geçersiz/süresi dolmuş token |

### 6.2 Hesap (Identity)
| # | Sayfa | Yol |
|---|-------|-----|
| 5 | Giriş Yap | `/Hesap/Giris` |
| 6 | Çıkış Yap | `/Hesap/Cikis` |

### 6.3 Yönetim Paneli (Oturum Gerektiren)
| # | Sayfa | Yol | Rol |
|---|-------|-----|-----|
| 7 | Dashboard | `/Yonetim/Dashboard` | Admin, KaliteBirimi |
| 8 | Hastane Listesi | `/Yonetim/Hastaneler` | Admin |
| 9 | Hastane Ekle/Düzenle | `/Yonetim/Hastaneler/Ekle` | Admin |
| 10 | Birim Listesi | `/Yonetim/Birimler` | Admin |
| 11 | Birim Ekle/Düzenle | `/Yonetim/Birimler/Ekle` | Admin |
| 12 | Doktor Listesi | `/Yonetim/Doktorlar` | Admin |
| 13 | Doktor Ekle/Düzenle | `/Yonetim/Doktorlar/Ekle` | Admin |
| 14 | Anket Listesi | `/Yonetim/Anketler` | Admin, KaliteBirimi |
| 15 | Anket Ekle/Düzenle | `/Yonetim/Anketler/Ekle` | Admin, KaliteBirimi |
| 16 | Soru Yönetimi | `/Yonetim/Anketler/{id}/Sorular` | Admin, KaliteBirimi |
| 17 | Davet Listesi | `/Yonetim/Davetler` | Admin, KaliteBirimi |
| 18 | Davet Oluştur | `/Yonetim/Davetler/Olustur` | Admin, KaliteBirimi |
| 19 | Yanıt Listesi | `/Yonetim/Yanitlar` | Admin, KaliteBirimi |
| 20 | Yanıt Detay | `/Yonetim/Yanitlar/{id}` | Admin, KaliteBirimi |
| 21 | Kullanıcı Listesi | `/Yonetim/Kullanicilar` | Admin |
| 22 | Kullanıcı Ekle/Düzenle | `/Yonetim/Kullanicilar/Ekle` | Admin |

---

## 7. Varsayımlar ve Yapılandırılabilir Ayarlar

Aşağıdaki kurum kararları **sabit varsayım olarak dayatılmamıştır**; `SystemSetting` tablosunda veya `appsettings.json`'da yapılandırılabilir olarak tasarlanmıştır:

| Parametre | Varsayılan | Ayar Yeri |
|-----------|------------|-----------|
| Anket bağlantı geçerlilik süresi | 72 saat | SystemSetting |
| Puanlama alt sınır | 1 | Soru bazında |
| Puanlama üst sınır | 5 | Soru bazında |
| Açık uçlu metin karakter limiti | 1000 | Soru bazında / SystemSetting |
| Aynı telefona minimum gönderim aralığı | 30 gün | SystemSetting |
| SMS sağlayıcısı | FakeSms (log) | DI / appsettings |
| Dashboard varsayılan tarih aralığı | Son 30 gün | SystemSetting |
| Düşük örneklem eşiği | 5 | SystemSetting |

---

## 8. Geliştirme Fazları

### Faz 1 — Çekirdek Altyapı
- Solution yapısı ve proje referansları
- Domain katmanı: Tüm entity, enum, interface
- Application katmanı: Servis arayüzleri, DTO, servis implementasyonları
- Infrastructure katmanı: DbContext, EF configuration, repository, Identity, Fake SMS
- Migration ve seed data

### Faz 2 — Web UI
- Program.cs ve DI kayıtları
- Layout, navbar, Bootstrap tema
- Giriş/Çıkış sayfaları
- Yönetim CRUD sayfaları (Hastane, Birim, Doktor, Anket, Soru, Davet, Kullanıcı)
- Anonim anket doldurma sayfaları
- Dashboard sayfası

### Faz 3 — Test ve Dokümantasyon
- xUnit birim testleri (kritik iş kuralları)
- README (kurulum, migration, seed, çalıştırma)
- İzlenebilirlik tablosu

---

## 9. Genişleme Noktaları (Faz 2+ Hazırlık)

Faz 1'de aşağıdaki arayüzler/enum'lar tanımlanarak gelecek fazlarda mevcut kodu değiştirmeden ekleme yapılabilir:

- `ISmsSender` → Gerçek SMS sağlayıcısı (Netgsm, İleti Merkezi vb.)
- `SoruTipi` enum → Yeni soru tipleri
- `GonderimKanali` enum → QR kanal desteği
- `AnketTuru` enum → Yeni anket türleri
- `IQrKodUretici` → QR kod üretim servisi
- `IAuditLogger` → Denetim kaydı servisi
- `IDisaAktarmaServisi` → Excel/CSV dışa aktarma
- `IKritikGeriBildirimKuralMotoru` → Kritik geri bildirim tespit motoru
