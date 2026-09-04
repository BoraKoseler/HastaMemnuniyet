# Hasta Memnuniyet Anketi

Sağlık kuruluşlarında ayaktan ve yatan hastalardan anonim memnuniyet geri bildirimi toplamak için geliştirilmiş bir web uygulamasıdır. Hasta telefon numaralarına SMS ile benzersiz anket bağlantısı gönderilir; toplanan yanıtlar hastane, birim ve doktor kırılımında raporlanır. Kişisel veri gizliliği için telefon numaraları veritabanında ham olarak tutulmaz, yalnızca geri döndürülemez hash değerleri saklanır.

## Teknolojiler

- **.NET 8** / **ASP.NET Core Razor Pages** (C#)
- **Entity Framework Core 8** (Code-First, Migrations)
- **SQL Server**
- **ASP.NET Core Identity** (kimlik doğrulama ve rol yönetimi)
- **Clean Architecture** (Domain / Application / Infrastructure / Web katmanları)
- **Bootstrap 5** (arayüz)
- **xUnit** + **Moq** + **EF Core InMemory** (birim testleri)

## Katman Yapısı

```
src/
  HastaMemnuniyet.Domain          # Varlıklar, enum'lar, arayüzler (bağımsız çekirdek)
  HastaMemnuniyet.Application      # İş servisleri, DTO'lar, doğrulayıcılar
  HastaMemnuniyet.Infrastructure   # EF Core, repository'ler, Identity, SMS, seed
  HastaMemnuniyet.Web             # Razor Pages arayüzü (sunum katmanı)
tests/
  HastaMemnuniyet.Tests           # Birim ve smoke testleri
docs/
  TRACEABILITY.md                 # Gereksinim izlenebilirlik tablosu
```

## Ön Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express veya tam sürüm)
- `dotnet-ef` aracı (migration için):

  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Kurulum

1. **Depoyu klonlayın**

   ```bash
   git clone <repo-url>
   cd HastaMemnuniyet
   ```

2. **Bağımlılıkları geri yükleyin ve derleyin**

   ```bash
   dotnet restore
   dotnet build
   ```

3. **Bağlantı dizesini (connection string) ayarlayın**

   `src/HastaMemnuniyet.Web/appsettings.json` içindeki `DefaultConnection` değerini kendi SQL Server ortamınıza göre güncelleyin:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=HastaMemnuniyetDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
   }
   ```

4. **Veritabanını oluşturun (migration)**

   ```bash
   dotnet ef database update \
     --project src/HastaMemnuniyet.Infrastructure \
     --startup-project src/HastaMemnuniyet.Web
   ```

5. **Uygulamayı çalıştırın**

   ```bash
   dotnet run --project src/HastaMemnuniyet.Web
   ```

   Uygulama başlatıldığında konsolda gösterilen adrese (ör. `https://localhost:5001`) tarayıcıdan erişebilirsiniz.

## Seed (Örnek) Verisi

`appsettings.json` içindeki `"SeedVerisiniYukle": true` ayarı etkinken uygulama ilk çalıştırmada örnek verileri otomatik yükler. Yüklenen veriler:

**Varsayılan kullanıcılar**

| Rol           | E-posta                          | Parola        |
|---------------|----------------------------------|---------------|
| Admin         | `admin@hastamemnuniyet.com`      | `Admin123!`   |
| Kalite Birimi | `kalite@hastamemnuniyet.com`     | `Kalite123!`  |
| Üst Yönetim   | `ustyonetim@hastamemnuniyet.com` | `Yonetim123!` |

**Örnek kayıtlar**

- 2 hastane (Ankara Şehir Hastanesi, İstanbul Üniversite Hastanesi) ve bunlara bağlı birimler/doktorlar
- 1 örnek anket ("Ayaktan Hasta Memnuniyet Anketi") — puanlama, evet/hayır ve açık uçlu sorular
- Varsayılan sistem ayarları (aşağıdaki tabloya bakınız)

> **Not:** Parolalar yalnızca geliştirme/test amaçlıdır. Üretim ortamına geçmeden mutlaka değiştirin.

## Testleri Çalıştırma

```bash
dotnet test
```

Test projesi (`tests/HastaMemnuniyet.Tests`) altyapı smoke testlerini ve temel servis birim testlerini (token geçerlilik kontrolü, yanıt oranı hesaplama, telefon hash tutarlılığı, token benzersizliği, kritik geri bildirim kuralı, kapsam filtresi) içerir. Faz 5 kapsamında ayrıca **tekrar gönderim engeli** ve **anket sürüm oluşturma** iş kuralları için testler eklenmiştir. Testler InMemory veritabanı ve Moq ile çalışır; ayrı bir SQL Server bağlantısı gerektirmez.

## SMS Sağlayıcısı Entegrasyonu

Uygulama, SMS gönderimini `ISmsSender` arayüzü üzerinden soyutlar. Geliştirme ortamında varsayılan olarak gerçek SMS göndermeyen **`FakeSmsGonderici`** (Infrastructure katmanı) kullanılır; gönderim denemeleri `SmsGonderimKaydi` tablosuna kaydedilir.

Gerçek bir SMS sağlayıcısına (ör. Netgsm, Twilio, İletimerkezi vb.) geçmek için:

1. `Domain/Interfaces/ISmsSender.cs` arayüzünü uygulayan yeni bir sınıf yazın (ör. `NetgsmSmsGonderici`) ve `GonderAsync` içinde sağlayıcının API'sini çağırın.
2. Sağlayıcı bilgilerini (API anahtarı, başlık vb.) `appsettings.json` üzerinden yapılandırın.
3. `Web/Program.cs` içindeki bağımlılık kaydını güncelleyin:

   ```csharp
   // builder.Services.AddScoped<ISmsSender, FakeSmsGonderici>();
   builder.Services.AddScoped<ISmsSender, NetgsmSmsGonderici>();
   ```

Arayüz `GonderAsync`, `SmsGonderimSonucu` (başarı/başarısızlık + mesaj) döndürür; başarısız gönderimler davet durumunu ve `SmsGonderimKaydi` kaydını uygun şekilde etkiler.

## Varsayılan Sistem Ayarları

Yönetim panelinden düzenlenebilen, seed ile yüklenen varsayılan ayarlar:

| Anahtar                        | Varsayılan | Açıklama                                        |
|--------------------------------|:----------:|-------------------------------------------------|
| Davet Geçerlilik Saati         | `72`       | Anket bağlantısının geçerlilik süresi (saat).   |
| Minimum Gönderim Aralığı       | `30`       | Aynı telefona minimum gönderim aralığı (gün).   |
| Düşük Örneklem Eşiği           | `5`        | Düşük örneklem uyarı eşiği.                      |
| Açık Uçlu Karakter Limiti      | `1000`     | Açık uçlu cevaplar için karakter limiti.        |
| Dashboard Varsayılan Gün Sayısı| `30`       | Dashboard varsayılan gösterim gün sayısı.       |
| Maksimum Hatırlatma Sayısı     | `2`        | Bir davet için gönderilebilecek en fazla hatırlatma. |
| Kişisel Veri Saklama Süresi    | `730`      | Kişisel verilerin (IP, telefon hash) saklanma süresi (gün). |
| Anket Yanıtı Saklama Süresi    | `1825`     | Anket yanıtı içeriğinin saklanma süresi (gün).  |
| Otomatik Anonimleştir          | `false`    | Süresi dolan kişisel verilerin otomatik anonimleştirilmesi. |

## Faz 5 – Son Genişleme Özellikleri

Bu fazda eklenen özellikler mevcut yapıya (Clean Architecture, EF Core, Identity) uyumlu biçimde genişletilmiştir:

- **Üst Yönetim rolü ve stratejik pano:** Yeni `Üst Yönetim` rolü ve `/Yonetim/UstYonetimDashboard` sayfası. Hastane karşılaştırmaları, doktor puanları (asgari örneklem eşiği ile) ve aylık trend gibi toplulaştırılmış, kişisel veri içermeyen göstergeler sunar. Admin de erişebilir.
- **Anket sürümleme:** Bir anketten yeni bir sürüm türetilebilir (`Yeni Sürüm Oluştur`). Yeni sürümün numarası artar, eski sürüm pasifleştirilir ve sürüm zinciri (`AnaSurumAnketId`) korunur. Geçmiş sürümler `/Yonetim/Anketler/Surumler/{id}` sayfasından görüntülenir.
- **Koşullu soru görünürlüğü:** Bir soru, başka bir sorunun belirli bir cevabına (`KosulBagliSoruId` + `KosulDegeri`) bağlı olarak anket doldurma ekranında dinamik olarak gösterilir/gizlenir.
- **Tekrar gönderim engeli ve hatırlatma:** Aynı ankete, aynı telefon numarasına `Minimum Gönderim Aralığı` içinde ikinci bir davet gönderimi engellenir. Gönderilmiş davetlere, `Maksimum Hatırlatma Sayısı` sınırı dahilinde hatırlatma gönderilebilir.
- **KVKK veri saklama politikası:** `/Yonetim/VeriYonetimi` sayfasından, saklama süresi dolan kişisel veriler (IP, istemci bilgisi, telefon hash) tek tek veya toplu olarak **geri alınamaz** biçimde anonimleştirilir. Tüm anonimleştirme işlemleri denetim kaydına (audit log) yazılır.

## Faz 6 – Gelişmiş Analitik Özellikler

Bu fazda dört analitik özellik, mevcut mimariye (Clean Architecture, EF Core, Identity) ek olarak ve birbiriyle uyumlu/optimize çalışacak biçimde eklenmiştir. Analiz servisleri veri erişimini `AsNoTracking` üzerinden yapar ve NPS puanları (0-10) genel memnuniyet ortalamasını (1-5) bozmayacak şekilde tüm puan hesaplarından ayrıştırılmıştır.

- **NPS (Net Tavsiye Skoru):** Yeni `Nps` soru tipi (0-10 ölçeği) ile hastalara tavsiye eğilimi sorulur. `DashboardServisi.NpsHesaplaAsync`, destekçi (9-10), pasif (7-8) ve kötüleyen (0-6) oranlarından NPS skorunu (-100..+100) ve kategorisini hesaplar; sonuç Dashboard'da özel bir kartta gösterilir. NPS cevapları genel ortalama puana dâhil edilmez.
- **Anahtar kelime analizi:** `MetinAnaliziServisi`, açık uçlu yanıtları **harici bir NLP kütüphanesi kullanmadan** saf C# ile çözümler. Türkçe kültüre uygun küçük harfe indirgeme, noktalama/rakam ayıklama ve Türkçe etkisiz kelime (stop-word) listesiyle en sık geçen anlamlı kelimeleri çıkarır. Sonuç `/Yonetim/Raporlar/AnahtarKelimeAnalizi` sayfasında kelime bulutu olarak sunulur (tarih/hastane filtresi).
- **Dönemsel karşılaştırma:** `DashboardServisi.DonemselKarsilastirAsync` iki tarih aralığının yanıt sayısı, ortalama puan, yanıt oranı ve NPS gibi metriklerini karşılaştırır; yüzdesel değişimleri hesaplar. `/Yonetim/Raporlar/DonemselKarsilastirma` sayfası varsayılan olarak bu ay ile geçen ayı karşılaştırır ve değişim rozetleri gösterir.
- **Doktor karnesi:** `DoktorKarnesiServisi` seçilen bir doktorun soru bazlı ortalamalarını, genel ortalamasını, NPS'ini, güçlü/zayıf alanlarını ve önceki döneme göre trendini üretir. Düşük örneklem (asgari eşik altı) durumunda uyarı verir. Karne `/Yonetim/Raporlar/DoktorKarnesi` sayfasında görüntülenir ve **Excel** (ClosedXML: Özet / Soru Bazlı / Güçlü-Zayıf sayfaları) olarak dışa aktarılabilir.

> Raporlar menüsü ve sayfaları `Admin`, `Kalite Birimi` ve `Üst Yönetim` rollerine açıktır.

### Üretim (production) notları

- **KVKK anonimleştirme geri alınamaz.** Toplu veya tekil anonimleştirme öncesinde kurumunuzun KVKK/hukuk birimine danışın ve düzenli veritabanı yedeği aldığınızdan emin olun.
- `Otomatik Anonimleştir` ayarı yalnızca politikanın durumunu belgeler; anonimleştirme işlemleri yönetici onayıyla `Veri Yönetimi` sayfasından tetiklenir. Zamanlanmış (otomatik) imha isteniyorsa, `IVeriSaklamaPolitikasiServisi.SuresiDolanlariAnonimlestirAsync` bir arka plan görevi (ör. hosted service) ile çağrılabilir.
- Saklama sürelerini (`Kişisel Veri Saklama Süresi`, `Anket Yanıtı Saklama Süresi`) kurumunuzun saklama ve imha politikasına göre yönetim panelinden güncelleyin.
- Listeleme sayfaları (ör. Yanıtlar) sayfalama ile sunulur; salt-okunur sorgular performans için `AsNoTracking` kullanır.
