// PersonelBL.cs — İş Katmanı
// Login iş kuralları burada kontrol edilir.
// Boş alan kontrolü, hatalı giriş yönetimi bu katmanda yapılır.

using CampStore.DataAccessLayer;
using CampStore.Entities;
using System;
using System.Data;

namespace CampStore.BusinessLayer
{
    public class PersonelBL
    {
        private PersonelDAL personelDAL = new PersonelDAL();

        // ── LOGIN KONTROLÜ ────────────────────────────────────────────────
        // Giriş bilgilerini doğrular, geçerliyse Personel nesnesini döndürür.
        // UI katmanı dönen nesneyi static bir değişkende saklar (oturum yönetimi).
        public Personel LoginKontrol(string tc, string sifre, out string mesaj)
        {
            // out parametresi: metot hem nesne hem mesaj döndürsün diye kullanıyoruz
            mesaj = "";

            // İş kuralı 1: TC boş olamaz
            if (string.IsNullOrWhiteSpace(tc))
            {
                mesaj = "TC kimlik numarası boş bırakılamaz!";
                return null;
            }

            // İş kuralı 2: TC 11 haneli olmalı
            if (tc.Length != 11)
            {
                mesaj = "TC kimlik numarası 11 haneli olmalıdır!";
                return null;
            }

            // İş kuralı 3: Şifre boş olamaz
            if (string.IsNullOrWhiteSpace(sifre))
            {
                mesaj = "Şifre boş bırakılamaz!";
                return null;
            }

            try
            {
                // Tüm kurallar geçildi, DAL'ı çağır
                Personel personel = personelDAL.LoginKontrol(tc, sifre);

                if (personel == null)
                {
                    // Veritabanında eşleşme yok
                    mesaj = "TC veya şifre hatalı!";
                    return null;
                }

                // Giriş başarılı
                mesaj = "OK";
                return personel;
            }
            catch (Exception ex)
            {
                mesaj = "Bağlantı hatası: " + ex.Message;
                return null;
            }
        }
        // PersonelListele metodu — PersonelDAL'dan tüm personeli çeker
        public DataTable PersonelListele()
        {
            return personelDAL.PersonelListele();
        }
        // PersonelBL.cs'e eklenecek metotlar

        public string PersonelEkle(Personel p)
        {
            if (string.IsNullOrWhiteSpace(p.PerAd)) return "Ad boş bırakılamaz!";
            if (string.IsNullOrWhiteSpace(p.PerSoyad)) return "Soyad boş bırakılamaz!";
            if (p.TC.Length != 11) return "TC 11 haneli olmalıdır!";
            if (p.RolID <= 0) return "Lütfen rol seçiniz!";
            if (string.IsNullOrWhiteSpace(p.Sifre)) return "Şifre boş bırakılamaz!";

            try { personelDAL.PersonelEkle(p); return "OK"; }
            catch (Exception ex) { return "Hata: " + ex.Message; }
        }

        public string PersonelGuncelle(Personel p)
        {
            if (string.IsNullOrWhiteSpace(p.PerAd)) return "Ad boş bırakılamaz!";
            if (string.IsNullOrWhiteSpace(p.PerSoyad)) return "Soyad boş bırakılamaz!";
            if (p.TC.Length != 11) return "TC 11 haneli olmalıdır!";
            if (p.RolID <= 0) return "Lütfen rol seçiniz!";

            try { personelDAL.PersonelGuncelle(p); return "OK"; }
            catch (Exception ex) { return "Hata: " + ex.Message; }
        }

        public string PersonelSil(int perID)
        {
            if (perID <= 0) return "Geçersiz personel!";

            try { personelDAL.PersonelSil(perID); return "OK"; }
            catch { return "Bu personele ait kayıtlar var, silinemez!"; }
        }

        public DataTable RolListele()
        {
            return personelDAL.RolListele();
        }
    }
}