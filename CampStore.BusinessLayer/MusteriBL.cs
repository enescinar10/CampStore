using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// MusteriBL.cs — İş Katmanı
// Müşteri işlemleri için iş kuralları burada uygulanır.
using System.Data;
using System.Text.RegularExpressions;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class MusteriBL
    {
        private MusteriDAL musteriDAL = new MusteriDAL();

        // ── EKLE ─────────────────────────────────────────────────────────
        public string MusteriEkle(Musteri m)
        {
            // İş kuralı 1: Ad boş olamaz
            if (string.IsNullOrWhiteSpace(m.Ad))
                return "Ad boş bırakılamaz!";

            // İş kuralı 2: Soyad boş olamaz
            if (string.IsNullOrWhiteSpace(m.Soyad))
                return "Soyad boş bırakılamaz!";

            // İş kuralı 3: Email formatı kontrolü
            if (!Regex.IsMatch(m.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Geçerli bir email adresi giriniz!";

            // İş kuralı 4: Telefon boş olamaz
            if (string.IsNullOrWhiteSpace(m.Telefon))
                return "Telefon boş bırakılamaz!";

            // İş kuralı 5: Şifre en az 4 karakter olmalı
            if (string.IsNullOrWhiteSpace(m.Sifre) || m.Sifre.Length < 4)
                return "Şifre en az 4 karakter olmalıdır!";

            // İş kuralı 6: Şehir seçilmeli
            if (m.SehirID <= 0)
                return "Lütfen şehir seçiniz!";

            // İş kuralı 7: İlçe seçilmeli
            if (m.IlceID <= 0)
                return "Lütfen ilçe seçiniz!";

            try
            {
                musteriDAL.MusteriEkle(m);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        public string MusteriGuncelle(Musteri m)
        {
            if (string.IsNullOrWhiteSpace(m.Ad))
                return "Ad boş bırakılamaz!";

            if (string.IsNullOrWhiteSpace(m.Soyad))
                return "Soyad boş bırakılamaz!";

            if (!Regex.IsMatch(m.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Geçerli bir email adresi giriniz!";

            if (string.IsNullOrWhiteSpace(m.Telefon))
                return "Telefon boş bırakılamaz!";

            if (m.SehirID <= 0)
                return "Lütfen şehir seçiniz!";

            if (m.IlceID <= 0)
                return "Lütfen ilçe seçiniz!";

            try
            {
                musteriDAL.MusteriGuncelle(m);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        public string MusteriSil(int musteriID)
        {
            if (musteriID <= 0)
                return "Geçersiz müşteri!";

            try
            {
                musteriDAL.MusteriSil(musteriID);
                return "OK";
            }
            catch (Exception ex)
            {
                // Müşteriye ait satış kaydı varsa SQL hata fırlatır
                return "Bu müşteriye ait satış kaydı var, silinemez!";
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable MusteriListele()
        {
            return musteriDAL.MusteriListele();
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public Musteri MusteriGetirById(int musteriID)
        {
            return musteriDAL.MusteriGetirById(musteriID);
        }

        // ── ŞEHİR LİSTELE ────────────────────────────────────────────────
        public DataTable SehirListele()
        {
            return musteriDAL.SehirListele();
        }

        // ── İLÇE LİSTELE ─────────────────────────────────────────────────
        public DataTable IlceListeleBySehir(int sehirID)
        {
            return musteriDAL.IlceListeleBySehir(sehirID);
        }
    }
}
