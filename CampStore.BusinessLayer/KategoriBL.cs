using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// KategoriBL.cs — İş Katmanı
// Kategori işlemleri için iş kuralları burada uygulanır.
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class KategoriBL
    {
        private KategoriDAL kategoriDAL = new KategoriDAL();

        // ── EKLE ─────────────────────────────────────────────────────────
        public string KategoriEkle(Kategori k)
        {
            if (string.IsNullOrWhiteSpace(k.KatAdi))
                return "Kategori adı boş bırakılamaz!";

            if (k.KatAdi.Length > 50)
                return "Kategori adı en fazla 50 karakter olabilir!";

            try
            {
                kategoriDAL.KategoriEkle(k);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        public string KategoriGuncelle(Kategori k)
        {
            if (string.IsNullOrWhiteSpace(k.KatAdi))
                return "Kategori adı boş bırakılamaz!";

            if (k.KatAdi.Length > 50)
                return "Kategori adı en fazla 50 karakter olabilir!";

            // Bir kategori kendisinin üst kategorisi olamaz
            if (k.UstKategoriID.HasValue && k.UstKategoriID.Value == k.KatID)
                return "Bir kategori kendisinin alt kategorisi olamaz!";

            try
            {
                kategoriDAL.KategoriGuncelle(k);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        public string KategoriSil(int katID)
        {
            if (katID <= 0)
                return "Geçersiz kategori!";

            try
            {
                kategoriDAL.KategoriSil(katID);
                return "OK";
            }
            catch (Exception ex)
            {
                // Kategoriye bağlı ürün varsa SQL hata fırlatır, bunu yakalarız
                return "Bu kategoriye ait ürünler var, önce ürünleri silin!";
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable KategoriListele()
        {
            return kategoriDAL.KategoriListele();
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public Kategori KategoriGetirById(int katID)
        {
            return kategoriDAL.KategoriGetirById(katID);
        }
    }
}