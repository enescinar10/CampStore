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
        public Personel LoginKontrol(string girisBilgisi, string sifre, out string mesaj)
        {
            mesaj = "";

            // Boş kontrol
            if (string.IsNullOrWhiteSpace(girisBilgisi))
            {
                mesaj = "Kullanıcı bilgisi boş bırakılamaz!";
                return null;
            }

            if (string.IsNullOrWhiteSpace(sifre))
            {
                mesaj = "Şifre boş bırakılamaz!";
                return null;
            }

            try
            {
                Personel personel = personelDAL.LoginKontrol(girisBilgisi, sifre);

                if (personel == null)
                {
                    mesaj = "Kullanıcı bilgisi veya şifre hatalı!";
                    return null;
                }

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