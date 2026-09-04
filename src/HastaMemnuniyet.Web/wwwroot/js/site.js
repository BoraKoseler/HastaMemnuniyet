// ============================================================
// Hasta Memnuniyet Sistemi — istemci tarafı yardımcı betikler
// ============================================================

(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        cascadeDropdownBaslat();
        soruTipiAlanlariBaslat();
        karakterSayaclariBaslat();
        secenekYonetimiBaslat();
    });

    // --------------------------------------------------------
    // 1) Cascade dropdown (Davet oluşturma: hastane -> birim -> doktor)
    // --------------------------------------------------------
    function cascadeDropdownBaslat() {
        const hastaneSecim = document.getElementById("hastaneSecim");
        const birimSecim = document.getElementById("birimSecim");
        const doktorSecim = document.getElementById("doktorSecim");

        if (!hastaneSecim || !birimSecim) {
            return;
        }

        hastaneSecim.addEventListener("change", async function () {
            const hastaneId = this.value;
            temizleDropdown(birimSecim, "Birim seçiniz (isteğe bağlı)");
            if (doktorSecim) {
                temizleDropdown(doktorSecim, "Doktor seçiniz (isteğe bağlı)");
            }
            if (!hastaneId) {
                return;
            }
            try {
                const yanit = await fetch("/api/birimler?hastaneId=" + encodeURIComponent(hastaneId));
                if (!yanit.ok) return;
                const birimler = await yanit.json();
                birimler.forEach(function (b) {
                    const opt = document.createElement("option");
                    opt.value = b.id;
                    opt.textContent = b.ad;
                    birimSecim.appendChild(opt);
                });
            } catch (e) {
                console.error("Birimler yüklenemedi", e);
            }
        });

        if (birimSecim && doktorSecim) {
            birimSecim.addEventListener("change", async function () {
                const birimId = this.value;
                temizleDropdown(doktorSecim, "Doktor seçiniz (isteğe bağlı)");
                if (!birimId) {
                    return;
                }
                try {
                    const yanit = await fetch("/api/doktorlar?birimId=" + encodeURIComponent(birimId));
                    if (!yanit.ok) return;
                    const doktorlar = await yanit.json();
                    doktorlar.forEach(function (d) {
                        const opt = document.createElement("option");
                        opt.value = d.id;
                        opt.textContent = d.ad;
                        doktorSecim.appendChild(opt);
                    });
                } catch (e) {
                    console.error("Doktorlar yüklenemedi", e);
                }
            });
        }
    }

    function temizleDropdown(secim, varsayilanMetin) {
        secim.innerHTML = "";
        const opt = document.createElement("option");
        opt.value = "";
        opt.textContent = varsayilanMetin;
        secim.appendChild(opt);
    }

    // --------------------------------------------------------
    // 2) Soru tipi seçimine göre dinamik alan gösterme/gizleme
    //    (Anket soru yönetimi ekranı)
    // --------------------------------------------------------
    function soruTipiAlanlariBaslat() {
        const tipSecim = document.getElementById("soruTipiSecim");
        if (!tipSecim) {
            return;
        }
        tipSecim.addEventListener("change", function () {
            soruTipiAlanlariGuncelle(this.value);
        });
        // Sayfa açılışında mevcut değeri uygula
        soruTipiAlanlariGuncelle(tipSecim.value);
    }

    // SoruTipi enum değerleri: 1=Puanlama, 2=TekSecim, 3=CokluSecim, 4=EvetHayir, 5=AcikUclu
    function soruTipiAlanlariGuncelle(tipDeger) {
        const puanAlan = document.querySelector(".tip-alan-puanlama");
        const secenekAlan = document.querySelector(".tip-alan-secenek");
        const metinAlan = document.querySelector(".tip-alan-metin");

        gizle(puanAlan);
        gizle(secenekAlan);
        gizle(metinAlan);

        if (tipDeger === "1") {
            goster(puanAlan);
        } else if (tipDeger === "2" || tipDeger === "3") {
            goster(secenekAlan);
        } else if (tipDeger === "5") {
            goster(metinAlan);
        }
    }

    function goster(el) { if (el) el.style.display = "block"; }
    function gizle(el) { if (el) el.style.display = "none"; }

    // --------------------------------------------------------
    // 3) Seçenek ekleme/çıkarma (dinamik) — soru oluşturma formu
    // --------------------------------------------------------
    function secenekYonetimiBaslat() {
        const kap = document.getElementById("secenekKabi");
        const ekleBtn = document.getElementById("secenekEkleBtn");
        if (!kap || !ekleBtn) {
            return;
        }
        ekleBtn.addEventListener("click", function () {
            secenekSatiriEkle(kap);
        });
        // Silme (event delegation)
        kap.addEventListener("click", function (e) {
            const btn = e.target.closest(".secenek-sil");
            if (btn) {
                const satir = btn.closest(".secenek-satiri");
                if (satir) satir.remove();
            }
        });
    }

    function secenekSatiriEkle(kap, deger) {
        const satir = document.createElement("div");
        satir.className = "secenek-satiri";
        const input = document.createElement("input");
        input.type = "text";
        input.name = "Girdi.Secenekler";
        input.className = "form-control";
        input.placeholder = "Seçenek metni";
        if (deger) input.value = deger;
        const silBtn = document.createElement("button");
        silBtn.type = "button";
        silBtn.className = "btn btn-outline-danger secenek-sil";
        silBtn.innerHTML = '<i class="bi bi-trash"></i>';
        satir.appendChild(input);
        satir.appendChild(silBtn);
        kap.appendChild(satir);
    }

    // --------------------------------------------------------
    // 4) Karakter sayacı (açık uçlu sorular)
    // --------------------------------------------------------
    function karakterSayaclariBaslat() {
        const alanlar = document.querySelectorAll("[data-karakter-sayac]");
        alanlar.forEach(function (alan) {
            const hedefId = alan.getAttribute("data-karakter-sayac");
            const gosterge = document.getElementById(hedefId);
            if (!gosterge) return;
            const maks = alan.getAttribute("maxlength");
            const guncelle = function () {
                if (maks) {
                    gosterge.textContent = alan.value.length + " / " + maks + " karakter";
                } else {
                    gosterge.textContent = alan.value.length + " karakter";
                }
            };
            alan.addEventListener("input", guncelle);
            guncelle();
        });
    }
})();
