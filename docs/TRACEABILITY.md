# İzlenebilirlik Matrisi (Traceability Matrix)

Bu belge, Hasta Memnuniyet Anketi projesinin fonksiyonel (FR) ve teknik/fonksiyonel olmayan (NFR) gereksinimlerini karşılayan modül, servis ve sayfalarla eşleştirir.

## Fonksiyonel Gereksinimler (FR)

| ID    | Gereksinim                                                        | Durum        | Karşılayan Modül / Servis / Sayfa |
|-------|------------------------------------------------------------------|--------------|-----------------------------------|
| FR-01 | Kullanıcı kimlik doğrulama (giriş/çıkış)                          | Tamamlandı   | ASP.NET Core Identity, `Pages/Hesap/Giris`, `Pages/Hesap/Cikis` |
| FR-02 | Rol bazlı yetkilendirme (Admin, Kalite Birimi)                    | Tamamlandı   | Identity rolleri, `RolSabitleri`, `[Authorize]`, `Pages/Hesap/YetkisizErisim` |
| FR-03 | Hastane tanımlama ve yönetimi (CRUD)                              | Tamamlandı   | `HastaneServisi`, `Pages/Yonetim/Hastaneler` |
| FR-04 | Birim tanımlama ve yönetimi (CRUD)                               | Tamamlandı   | `BirimServisi`, `Pages/Yonetim/Birimler` |
| FR-05 | Doktor tanımlama ve yönetimi (CRUD)                              | Tamamlandı   | `DoktorServisi`, `Pages/Yonetim/Doktorlar` |
| FR-06 | Anket ve soru tanımlama/yönetimi                                 | Tamamlandı   | `AnketServisi`, `Pages/Yonetim/Anketler` |
| FR-07 | Anket daveti oluşturma ve benzersiz token üretimi                | Tamamlandı   | `DavetServisi`, `GuidTokenUretici`, `Pages/Yonetim/Davetler` |
| FR-08 | Davet bağlantısının SMS ile gönderimi                            | Tamamlandı   | `ISmsSender` / `FakeSmsGonderici`, `SmsGonderimKaydi` |
| FR-09 | Token geçerlilik ve süre kontrolü                                | Tamamlandı   | `DavetServisi`, `AnketDoldurmaServisi.TokenGecerliMiAsync` |
| FR-10 | Anonim hasta anket doldurma akışı                                | Tamamlandı   | `AnketDoldurmaServisi`, `Pages/Anket/Index`, `Pages/Anket/Doldur` |
| FR-11 | Anket tamamlama ve teşekkür / süresi dolmuş sayfaları            | Tamamlandı   | `Pages/Anket/Tesekkur`, `Pages/Anket/SuresiDolmus` |
| FR-12 | Yanıtların kaydı ve listelenmesi/filtrelenmesi                   | Tamamlandı   | `YanitServisi`, `Pages/Yonetim/Yanitlar` |
| FR-13 | Dashboard istatistikleri ve yanıt oranı hesaplama                | Tamamlandı   | `DashboardServisi`, `Pages/Yonetim/Dashboard` |
| FR-14 | Hastane/birim kırılımında ortalama puan raporlama                | Tamamlandı   | `DashboardServisi` (`HastanePuanlari`, `BirimPuanlari`) |
| FR-15 | Kullanıcı yönetimi (yönetici tarafından)                         | Tamamlandı   | `KullaniciServisi`, `Pages/Yonetim/Kullanicilar` |
| FR-16 | Üst Yönetim rolü ve stratejik özet panosu                        | Tamamlandı   | `RolSabitleri.UstYonetim`, `DashboardServisi` (`HastaneKarsilastirmaGetirAsync`, `DoktorPuanlariGetirAsync`, `AylikTrendGetirAsync`), `Pages/Yonetim/UstYonetimDashboard` |
| FR-17 | Anket sürümleme (yeni sürüm türetme, geçmiş sürümler)            | Tamamlandı   | `AnketServisi.SurumOlusturAsync` / `GecmisSurumleriGetirAsync`, `Anket.AnaSurumAnketId`, `Pages/Yonetim/Anketler/Surumler` |
| FR-18 | Koşullu (bağımlı) soru görünürlüğü                               | Tamamlandı   | `AnketSorusu.KosulBagliSoruId` + `KosulDegeri`, `Pages/Yonetim/Anketler/Sorular`, `Pages/Anket/Doldur` (JS) |
| FR-19 | Tekrar gönderim engeli ve hatırlatma gönderimi                   | Tamamlandı   | `DavetServisi.OlusturAsync` (aralık kontrolü) / `HatirlatmaGonderAsync`, `AnketDaveti.HatirlatmaSayisi`, `Pages/Yonetim/Davetler` |
| FR-20 | KVKK veri saklama politikası ve anonimleştirme                   | Tamamlandı   | `VeriSaklamaPolitikasiServisi`, `Pages/Yonetim/VeriYonetimi` |
| FR-21 | NPS (Net Tavsiye Skoru) sorusu, hesaplama ve dashboard kartı      | Tamamlandı   | `SoruTipi.Nps`, `DashboardServisi.NpsHesaplaAsync`, `NpsSonucuDto`, `Pages/Anket/Doldur`, `Pages/Yonetim/Dashboard` |
| FR-22 | Açık uçlu yanıtlarda anahtar kelime (frekans) analizi            | Tamamlandı   | `MetinAnaliziServisi` (saf C#, Türkçe etkisiz kelime ayıklama), `MetinAnaliziSonucuDto`, `Pages/Yonetim/Raporlar/AnahtarKelimeAnalizi` |
| FR-23 | İki dönem arası karşılaştırmalı analiz (yüzdesel değişim)         | Tamamlandı   | `DashboardServisi.DonemselKarsilastirAsync`, `DonemselKarsilastirmaDto`, `Pages/Yonetim/Raporlar/DonemselKarsilastirma` |
| FR-24 | Doktor performans karnesi (soru bazlı, güçlü/zayıf alan, trend, Excel) | Tamamlandı | `DoktorKarnesiServisi`, `DoktorKarnesiDto`, `ExcelDisaAktarmaServisi.DoktorKarnesiDisaAktar`, `Pages/Yonetim/Raporlar/DoktorKarnesi` |

## Teknik / Fonksiyonel Olmayan Gereksinimler (NFR)

| ID     | Gereksinim                                                       | Durum        | Karşılayan Modül / Servis / Sayfa |
|--------|-----------------------------------------------------------------|--------------|-----------------------------------|
| NFR-01 | Clean Architecture katmanlı yapı                                | Tamamlandı   | Domain / Application / Infrastructure / Web projeleri |
| NFR-02 | EF Core Code-First ile veri erişimi ve migration                | Tamamlandı   | `AppDbContext`, `Data/Migrations`, Repository'ler |
| NFR-03 | Kişisel veri gizliliği (telefon numarasının hash'lenmesi)       | Tamamlandı   | `TelefonHashleyici` (SHA-256, geri döndürülemez) |
| NFR-04 | Güvenli/benzersiz token üretimi                                  | Tamamlandı   | `GuidTokenUretici` (Guid + kriptografik rastgele bayt) |
| NFR-05 | Yapılandırılabilir sistem ayarları (seed ile varsayılanlar)      | Tamamlandı   | `SistemAyari`, `AyarAnahtarlari`, `SeedData` |
| NFR-06 | Otomatik birim testleri ile temel iş kuralı doğrulaması          | Tamamlandı   | `HastaMemnuniyet.Tests` (xUnit, Moq, EF InMemory) |
| NFR-07 | Türkçe kullanıcı arayüzü ve dokümantasyon                        | Tamamlandı   | Razor Pages (Bootstrap 5), `README.md`, bu belge |
| NFR-08 | KVKK uyumlu veri saklama/imha ve denetim kaydı                   | Tamamlandı   | `VeriSaklamaPolitikasiServisi` (geri alınamaz anonimleştirme + `IAuditLogger`), `AyarAnahtarlari` (saklama süreleri) |
| NFR-09 | Sorgu performansı (AsNoTracking) ve liste sayfalama              | Tamamlandı   | `GenericRepository` (`AsNoTracking`), `Pages/Yonetim/Yanitlar` (sayfalama) |
