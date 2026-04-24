using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// StokBL.cs — İş Katmanı
// Stok işlemleri için iş kuralları burada uygulanır.
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class StokBL
    {
        private StokDAL stokDAL = new StokDAL();

        // ── STOK HAREKETİ EKLE ────────────────────────────────────────────
        public string StokHareketEkle(StokHareket s)
        {
            // İş kuralı 1: Ürün seçilmeli
            if (s.UrunID <= 0)
                return "Lütfen ürün seçiniz!";

            // İş kuralı 2: Giriş ve çıkış aynı anda doldurulamaz
            if (s.GirisMiktar > 0 && s.CikisMiktar > 0)
                return "Aynı anda hem giriş hem çıkış yapılamaz!";

            // İş kuralı 3: İkisi de sıfır olamaz
            if (s.GirisMiktar == 0 && s.CikisMiktar == 0)
                return "Giriş veya çıkış miktarı girilmelidir!";

            // İş kuralı 4: Negatif değer girilemez
            if (s.GirisMiktar < 0 || s.CikisMiktar < 0)
                return "Miktar negatif olamaz!";

            try
            {
                stokDAL.StokHareketEkle(s);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable StokHareketListele()
        {
            return stokDAL.StokHareketListele();
        }

        // ── ÜRÜNE GÖRE LİSTELE ───────────────────────────────────────────
        public DataTable StokHareketListeleByUrun(int urunID)
        {
            return stokDAL.StokHareketListeleByUrun(urunID);
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public StokHareket StokHareketGetirById(int stokID)
        {
            return stokDAL.StokHareketGetirById(stokID);
        }
    }
}
