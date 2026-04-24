using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SatisBL.cs — İş Katmanı
// Satış işlemleri için iş kuralları burada uygulanır.
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class SatisBL
    {
        private SatisDAL satisDAL = new SatisDAL();

        // ═══════════════════════════════════════════════════════════════
        // SATIŞ İŞLEMLERİ
        // ═══════════════════════════════════════════════════════════════

        // ── EKLE ─────────────────────────────────────────────────────────
        public string SatisEkle(Satis s, out int yeniSatisID)
        {
            yeniSatisID = 0;

            // İş kuralı 1: Müşteri seçilmeli
            if (s.MusteriID <= 0)
                return "Lütfen müşteri seçiniz!";

            // İş kuralı 2: Personel otomatik atanır (oturum bilgisinden)
            if (s.PerID <= 0)
                return "Personel bilgisi alınamadı!";

            // İş kuralı 3: Tutar sıfırdan büyük olmalı
            if (s.ToplamTutar <= 0)
                return "Satış tutarı sıfırdan büyük olmalıdır!";

            try
            {
                yeniSatisID = satisDAL.SatisEkle(s);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        public string SatisGuncelle(Satis s)
        {
            if (s.MusteriID <= 0)
                return "Lütfen müşteri seçiniz!";

            if (s.ToplamTutar <= 0)
                return "Satış tutarı sıfırdan büyük olmalıdır!";

            if (string.IsNullOrWhiteSpace(s.Durum))
                return "Lütfen durum seçiniz!";

            try
            {
                satisDAL.SatisGuncelle(s);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        public string SatisSil(int satisID)
        {
            if (satisID <= 0)
                return "Geçersiz satış!";

            try
            {
                satisDAL.SatisSil(satisID);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Bu satışa ait fatura kaydı var, silinemez!";
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable SatisListele()
        {
            return satisDAL.SatisListele();
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public Satis SatisGetirById(int satisID)
        {
            return satisDAL.SatisGetirById(satisID);
        }

        // ── DURUMA GÖRE LİSTELE ───────────────────────────────────────────
        public DataTable SatisListeleByDurum(string durum)
        {
            return satisDAL.SatisListeleByDurum(durum);
        }

        // ═══════════════════════════════════════════════════════════════
        // SATIŞ DETAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════════════

        // ── DETAY EKLE ────────────────────────────────────────────────────
        public string SatisDetayEkle(SatisDetay sd)
        {
            // İş kuralı 1: Ürün seçilmeli
            if (sd.UrunID <= 0)
                return "Lütfen ürün seçiniz!";

            // İş kuralı 2: Miktar sıfırdan büyük olmalı
            if (sd.Miktar <= 0)
                return "Miktar sıfırdan büyük olmalıdır!";

            // İş kuralı 3: Birim fiyat sıfırdan büyük olmalı
            if (sd.BirimFiyat <= 0)
                return "Birim fiyat sıfırdan büyük olmalıdır!";

            try
            {
                satisDAL.SatisDetayEkle(sd);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── DETAY GÜNCELLE ────────────────────────────────────────────────
        public string SatisDetayGuncelle(SatisDetay sd)
        {
            if (sd.UrunID <= 0)
                return "Lütfen ürün seçiniz!";

            if (sd.Miktar <= 0)
                return "Miktar sıfırdan büyük olmalıdır!";

            if (sd.BirimFiyat <= 0)
                return "Birim fiyat sıfırdan büyük olmalıdır!";

            try
            {
                satisDAL.SatisDetayGuncelle(sd);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── DETAY SİL ─────────────────────────────────────────────────────
        public string SatisDetaySil(int detayID)
        {
            if (detayID <= 0)
                return "Geçersiz kayıt!";

            try
            {
                satisDAL.SatisDetaySil(detayID);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── DETAY LİSTELE (SATIŞA GÖRE) ──────────────────────────────────
        public DataTable SatisDetayListeleBySatis(int satisID)
        {
            return satisDAL.SatisDetayListeleBySatis(satisID);
        }

        // ── TÜM DETAYLARI LİSTELE ─────────────────────────────────────────
        public DataTable SatisDetayListele()
        {
            return satisDAL.SatisDetayListele();
        }
    }
}
