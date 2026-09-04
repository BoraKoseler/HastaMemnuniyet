using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HastaMemnuniyet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IlkOlusturmaPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anketler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AnketTuru = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    SurumNo = table.Column<int>(type: "integer", nullable: false),
                    AnaSurumAnketId = table.Column<int>(type: "integer", nullable: true),
                    GizlilikMetni = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TahminiSureDakika = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anketler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DenetimKayitlari",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    KullaniciAdi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Islem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tablo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    KayitId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EskiDeger = table.Column<string>(type: "text", nullable: true),
                    YeniDeger = table.Column<string>(type: "text", nullable: true),
                    IpAdresi = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Detay = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenetimKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doktorlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unvan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doktorlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hastaneler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Kod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Adres = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Telefon = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hastaneler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SistemAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Anahtar = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Deger = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Kategori = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SistemAyarlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnketSorulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnketId = table.Column<int>(type: "integer", nullable: false),
                    SoruMetni = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SoruTipi = table.Column<int>(type: "integer", nullable: false),
                    SiraNo = table.Column<int>(type: "integer", nullable: false),
                    ZorunluMu = table.Column<bool>(type: "boolean", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    Kategori = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PuanlamaAltSinir = table.Column<int>(type: "integer", nullable: true),
                    PuanlamaUstSinir = table.Column<int>(type: "integer", nullable: true),
                    MaksimumKarakterSayisi = table.Column<int>(type: "integer", nullable: true),
                    KosulBagliSoruId = table.Column<int>(type: "integer", nullable: true),
                    KosulDegeri = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnketSorulari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnketSorulari_AnketSorulari_KosulBagliSoruId",
                        column: x => x.KosulBagliSoruId,
                        principalTable: "AnketSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketSorulari_Anketler_AnketId",
                        column: x => x.AnketId,
                        principalTable: "Anketler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Birimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Kod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Birimler_Hastaneler_HastaneId",
                        column: x => x.HastaneId,
                        principalTable: "Hastaneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciHastaneKapsamlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "text", nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciHastaneKapsamlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciHastaneKapsamlari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciHastaneKapsamlari_Hastaneler_HastaneId",
                        column: x => x.HastaneId,
                        principalTable: "Hastaneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnketSoruSecenekleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SoruId = table.Column<int>(type: "integer", nullable: false),
                    MetinDegeri = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SiraNo = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnketSoruSecenekleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnketSoruSecenekleri_AnketSorulari_SoruId",
                        column: x => x.SoruId,
                        principalTable: "AnketSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KritikGeriBildirimKurallari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnketId = table.Column<int>(type: "integer", nullable: true),
                    SoruId = table.Column<int>(type: "integer", nullable: true),
                    KuralTipi = table.Column<int>(type: "integer", nullable: false),
                    EsikDegeri = table.Column<int>(type: "integer", nullable: true),
                    AnahtarKelimeler = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HedefSecenekId = table.Column<int>(type: "integer", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KritikGeriBildirimKurallari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KritikGeriBildirimKurallari_AnketSorulari_SoruId",
                        column: x => x.SoruId,
                        principalTable: "AnketSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KritikGeriBildirimKurallari_Anketler_AnketId",
                        column: x => x.AnketId,
                        principalTable: "Anketler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnketDavetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnketId = table.Column<int>(type: "integer", nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: true),
                    DoktorId = table.Column<int>(type: "integer", nullable: true),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TelefonHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    GonderimKanali = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    HizmetTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SonGecerlilikTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OlusturanKullaniciId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    HatirlatmaSayisi = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnketDavetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnketDavetleri_Anketler_AnketId",
                        column: x => x.AnketId,
                        principalTable: "Anketler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketDavetleri_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketDavetleri_Doktorlar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "Doktorlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketDavetleri_Hastaneler_HastaneId",
                        column: x => x.HastaneId,
                        principalTable: "Hastaneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoktorBirimleri",
                columns: table => new
                {
                    DoktorId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoktorBirimleri", x => new { x.DoktorId, x.BirimId });
                    table.ForeignKey(
                        name: "FK_DoktorBirimleri_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoktorBirimleri_Doktorlar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "Doktorlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciBirimKapsamlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "text", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciBirimKapsamlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciBirimKapsamlari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciBirimKapsamlari_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QrAnketKampanyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AnketId = table.Column<int>(type: "integer", nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: true),
                    DoktorId = table.Column<int>(type: "integer", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    SonKullanmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    KullanimSayisi = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrAnketKampanyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrAnketKampanyalari_Anketler_AnketId",
                        column: x => x.AnketId,
                        principalTable: "Anketler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrAnketKampanyalari_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrAnketKampanyalari_Doktorlar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "Doktorlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrAnketKampanyalari_Hastaneler_HastaneId",
                        column: x => x.HastaneId,
                        principalTable: "Hastaneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnketYanitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DavetId = table.Column<int>(type: "integer", nullable: false),
                    AnketId = table.Column<int>(type: "integer", nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: true),
                    DoktorId = table.Column<int>(type: "integer", nullable: true),
                    BaslamaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IpAdresi = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    KullaniciAjan = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    GecerliMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnketYanitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnketYanitlari_AnketDavetleri_DavetId",
                        column: x => x.DavetId,
                        principalTable: "AnketDavetleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsGonderimKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DavetId = table.Column<int>(type: "integer", nullable: false),
                    TelefonHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SmsDurumu = table.Column<int>(type: "integer", nullable: false),
                    SmsYaniti = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GonderimTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsGonderimKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsGonderimKayitlari_AnketDavetleri_DavetId",
                        column: x => x.DavetId,
                        principalTable: "AnketDavetleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnketCevaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    YanitId = table.Column<int>(type: "integer", nullable: false),
                    SoruId = table.Column<int>(type: "integer", nullable: false),
                    PuanDegeri = table.Column<int>(type: "integer", nullable: true),
                    SecenekId = table.Column<int>(type: "integer", nullable: true),
                    MetinDegeri = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    BoolDegeri = table.Column<bool>(type: "boolean", nullable: true),
                    YanitTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SeciliSecenekIdleri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnketCevaplari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnketCevaplari_AnketSoruSecenekleri_SecenekId",
                        column: x => x.SecenekId,
                        principalTable: "AnketSoruSecenekleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketCevaplari_AnketSorulari_SoruId",
                        column: x => x.SoruId,
                        principalTable: "AnketSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnketCevaplari_AnketYanitlari_YanitId",
                        column: x => x.YanitId,
                        principalTable: "AnketYanitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KritikGeriBildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    YanitId = table.Column<int>(type: "integer", nullable: false),
                    CevapId = table.Column<int>(type: "integer", nullable: true),
                    KuralId = table.Column<int>(type: "integer", nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: true),
                    DoktorId = table.Column<int>(type: "integer", nullable: true),
                    Aciklama = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KritikGeriBildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KritikGeriBildirimler_AnketCevaplari_CevapId",
                        column: x => x.CevapId,
                        principalTable: "AnketCevaplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KritikGeriBildirimler_AnketYanitlari_YanitId",
                        column: x => x.YanitId,
                        principalTable: "AnketYanitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KritikGeriBildirimler_KritikGeriBildirimKurallari_KuralId",
                        column: x => x.KuralId,
                        principalTable: "KritikGeriBildirimKurallari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IyilestirmeAksiyonlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KritikGeriBildirimId = table.Column<int>(type: "integer", nullable: true),
                    Baslik = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    HastaneId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: true),
                    DoktorId = table.Column<int>(type: "integer", nullable: true),
                    SorumluKullaniciId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Oncelik = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    HedefTarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    KapanisNotu = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OlusturanKullaniciId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IyilestirmeAksiyonlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IyilestirmeAksiyonlari_AspNetUsers_SorumluKullaniciId",
                        column: x => x.SorumluKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IyilestirmeAksiyonlari_KritikGeriBildirimler_KritikGeriBild~",
                        column: x => x.KritikGeriBildirimId,
                        principalTable: "KritikGeriBildirimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AksiyonGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AksiyonId = table.Column<int>(type: "integer", nullable: false),
                    EskiDurum = table.Column<int>(type: "integer", nullable: false),
                    YeniDurum = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    KullaniciId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AksiyonGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AksiyonGecmisleri_IyilestirmeAksiyonlari_AksiyonId",
                        column: x => x.AksiyonId,
                        principalTable: "IyilestirmeAksiyonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AksiyonGecmisleri_AksiyonId",
                table: "AksiyonGecmisleri",
                column: "AksiyonId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketCevaplari_SecenekId",
                table: "AnketCevaplari",
                column: "SecenekId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketCevaplari_SoruId",
                table: "AnketCevaplari",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketCevaplari_YanitId",
                table: "AnketCevaplari",
                column: "YanitId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_AnketId",
                table: "AnketDavetleri",
                column: "AnketId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_BirimId",
                table: "AnketDavetleri",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_DoktorId",
                table: "AnketDavetleri",
                column: "DoktorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_Durum",
                table: "AnketDavetleri",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_HastaneId",
                table: "AnketDavetleri",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_TelefonHash",
                table: "AnketDavetleri",
                column: "TelefonHash");

            migrationBuilder.CreateIndex(
                name: "IX_AnketDavetleri_Token",
                table: "AnketDavetleri",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnketSorulari_AnketId_SiraNo",
                table: "AnketSorulari",
                columns: new[] { "AnketId", "SiraNo" });

            migrationBuilder.CreateIndex(
                name: "IX_AnketSorulari_KosulBagliSoruId",
                table: "AnketSorulari",
                column: "KosulBagliSoruId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketSoruSecenekleri_SoruId",
                table: "AnketSoruSecenekleri",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketYanitlari_AnketId",
                table: "AnketYanitlari",
                column: "AnketId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketYanitlari_DavetId",
                table: "AnketYanitlari",
                column: "DavetId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketYanitlari_HastaneId",
                table: "AnketYanitlari",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_AnketYanitlari_TamamlanmaTarihi",
                table: "AnketYanitlari",
                column: "TamamlanmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Birimler_HastaneId_Ad",
                table: "Birimler",
                columns: new[] { "HastaneId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_DenetimKayitlari_Tarih",
                table: "DenetimKayitlari",
                column: "Tarih");

            migrationBuilder.CreateIndex(
                name: "IX_DoktorBirimleri_BirimId",
                table: "DoktorBirimleri",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_Hastaneler_Kod",
                table: "Hastaneler",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IyilestirmeAksiyonlari_Durum",
                table: "IyilestirmeAksiyonlari",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_IyilestirmeAksiyonlari_HastaneId",
                table: "IyilestirmeAksiyonlari",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_IyilestirmeAksiyonlari_KritikGeriBildirimId",
                table: "IyilestirmeAksiyonlari",
                column: "KritikGeriBildirimId");

            migrationBuilder.CreateIndex(
                name: "IX_IyilestirmeAksiyonlari_SorumluKullaniciId",
                table: "IyilestirmeAksiyonlari",
                column: "SorumluKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimKurallari_AktifMi",
                table: "KritikGeriBildirimKurallari",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimKurallari_AnketId",
                table: "KritikGeriBildirimKurallari",
                column: "AnketId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimKurallari_SoruId",
                table: "KritikGeriBildirimKurallari",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimler_CevapId",
                table: "KritikGeriBildirimler",
                column: "CevapId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimler_HastaneId",
                table: "KritikGeriBildirimler",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimler_KuralId",
                table: "KritikGeriBildirimler",
                column: "KuralId");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimler_OlusturulmaTarihi",
                table: "KritikGeriBildirimler",
                column: "OlusturulmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_KritikGeriBildirimler_YanitId",
                table: "KritikGeriBildirimler",
                column: "YanitId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciBirimKapsamlari_BirimId",
                table: "KullaniciBirimKapsamlari",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciBirimKapsamlari_KullaniciId_BirimId",
                table: "KullaniciBirimKapsamlari",
                columns: new[] { "KullaniciId", "BirimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciHastaneKapsamlari_HastaneId",
                table: "KullaniciHastaneKapsamlari",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciHastaneKapsamlari_KullaniciId_HastaneId",
                table: "KullaniciHastaneKapsamlari",
                columns: new[] { "KullaniciId", "HastaneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QrAnketKampanyalari_AktifMi",
                table: "QrAnketKampanyalari",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_QrAnketKampanyalari_AnketId",
                table: "QrAnketKampanyalari",
                column: "AnketId");

            migrationBuilder.CreateIndex(
                name: "IX_QrAnketKampanyalari_BirimId",
                table: "QrAnketKampanyalari",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_QrAnketKampanyalari_DoktorId",
                table: "QrAnketKampanyalari",
                column: "DoktorId");

            migrationBuilder.CreateIndex(
                name: "IX_QrAnketKampanyalari_HastaneId",
                table: "QrAnketKampanyalari",
                column: "HastaneId");

            migrationBuilder.CreateIndex(
                name: "IX_SistemAyarlari_Anahtar",
                table: "SistemAyarlari",
                column: "Anahtar",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsGonderimKayitlari_DavetId",
                table: "SmsGonderimKayitlari",
                column: "DavetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AksiyonGecmisleri");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DenetimKayitlari");

            migrationBuilder.DropTable(
                name: "DoktorBirimleri");

            migrationBuilder.DropTable(
                name: "KullaniciBirimKapsamlari");

            migrationBuilder.DropTable(
                name: "KullaniciHastaneKapsamlari");

            migrationBuilder.DropTable(
                name: "QrAnketKampanyalari");

            migrationBuilder.DropTable(
                name: "SistemAyarlari");

            migrationBuilder.DropTable(
                name: "SmsGonderimKayitlari");

            migrationBuilder.DropTable(
                name: "IyilestirmeAksiyonlari");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "KritikGeriBildirimler");

            migrationBuilder.DropTable(
                name: "AnketCevaplari");

            migrationBuilder.DropTable(
                name: "KritikGeriBildirimKurallari");

            migrationBuilder.DropTable(
                name: "AnketSoruSecenekleri");

            migrationBuilder.DropTable(
                name: "AnketYanitlari");

            migrationBuilder.DropTable(
                name: "AnketSorulari");

            migrationBuilder.DropTable(
                name: "AnketDavetleri");

            migrationBuilder.DropTable(
                name: "Anketler");

            migrationBuilder.DropTable(
                name: "Birimler");

            migrationBuilder.DropTable(
                name: "Doktorlar");

            migrationBuilder.DropTable(
                name: "Hastaneler");
        }
    }
}
