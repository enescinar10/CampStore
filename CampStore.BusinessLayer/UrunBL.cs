using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// UrunBL.cs — İş Katmanı
// Ürün işlemleri için iş kuralları burada uygulanır.
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class UrunBL
    {
        private UrunDAL urunDAL = new UrunDAL();

        // ── EKLE ─────────────────────────────────────────────────────────
        public string UrunEkle(Urun u)
        {
            // İş kuralı 1: Ürün adı boş olamaz
            if (string.IsNullOrWhiteSpace(u.UrunAdi))
                return "Ürün adı boş bırakılamaz!";

            // İş kuralı 2: Fiyat sıfırdan büyük olmalı
            if (u.Fiyat <= 0)
                return "Fiyat sıfırdan büyük olmalıdır!";

            // İş kuralı 3: Stok negatif olamaz
            if (u.Stok < 0)
                return "Stok miktarı negatif olamaz!";

            // İş kuralı 4: Kategori seçilmeli
            if (u.KatID <= 0)
                return "Lütfen bir kategori seçiniz!";

            try
            {
                urunDAL.UrunEkle(u);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        public string UrunGuncelle(Urun u)
        {
            if (string.IsNullOrWhiteSpace(u.UrunAdi))
                return "Ürün adı boş bırakılamaz!";

            if (u.Fiyat <= 0)
                return "Fiyat sıfırdan büyük olmalıdır!";

            if (u.Stok < 0)
                return "Stok miktarı negatif olamaz!";

            if (u.KatID <= 0)
                return "Lütfen bir kategori seçiniz!";

            try
            {
                urunDAL.UrunGuncelle(u);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        public string UrunSil(int urunID)
        {
            if (urunID <= 0)
                return "Geçersiz ürün!";

            try
            {
                urunDAL.UrunSil(urunID);
                return "OK";
            }
            catch (Exception ex)
            {
                // Ürüne bağlı satış detayı varsa SQL hata fırlatır
                return "Bu ürüne ait satış kaydı var, silinemez!";
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable UrunListele()
        {
            return urunDAL.UrunListele();
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public Urun UrunGetirById(int urunID)
        {
            return urunDAL.UrunGetirById(urunID);
        }

        // ── KATEGORİYE GÖRE LİSTELE ──────────────────────────────────────
        public DataTable UrunListeleByKategori(int katID)
        {
            return urunDAL.UrunListeleByKategori(katID);
        }
        // UrunBL.cs'e ekle
        public DataTable KritikStokListele()
        {
            return urunDAL.KritikStokListele();
        }
    }
}
