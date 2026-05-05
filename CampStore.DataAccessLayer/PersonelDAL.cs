// PersonelDAL.cs — Veri Katmanı
// Tüm işlemler stored procedure üzerinden yapılır.
// Doğrudan SQL sorgusu yazılmaz.
using System;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

namespace CampStore.DataAccessLayer
{
    public class PersonelDAL
    {
        // ── LOGIN KONTROLÜ ────────────────────────────────────────────────
        // sp_PersonelLoginKontrol prosedürünü çağırır.
        // TC ve şifre eşleşirse Personel nesnesi döner, eşleşmezse null döner.
        public Personel LoginKontrol(string girisBilgisi, string sifre)
        {
            Personel personel = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_PersonelLoginKontrol", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                // Tek parametre — TC, kullanıcı adı veya email olabilir
                komut.Parameters.AddWithValue("@GirisBilgisi", girisBilgisi);
                komut.Parameters.AddWithValue("@Sifre", sifre);

                try
                {
                    baglanti.Open();
                    SqlDataReader reader = komut.ExecuteReader();

                    if (reader.Read())
                    {
                        personel = new Personel
                        {
                            PerID = Convert.ToInt32(reader["PerID"]),
                            PerAd = reader["PerAd"].ToString(),
                            PerSoyad = reader["PerSoyad"].ToString(),
                            RolID = Convert.ToInt32(reader["RolID"]),
                            RolAdi = reader["RolAdi"].ToString(),
                            KullaniciAdi = reader["KullaniciAdi"].ToString(),
                            TC = reader["TC"].ToString(),
                            Telefon = reader["Telefon"].ToString(),
                            Adres = reader["Adres"].ToString(),
                            IseGirisTarihi = Convert.ToDateTime(reader["IseGirisTarihi"])
                        };
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Giriş bilgileri doğrulanırken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return personel;
        }

        // ── PERSONEL EKLE ─────────────────────────────────────────────────
        // sp_PersonelEkle prosedürünü çağırır.
        public void PersonelEkle(Personel p)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_PersonelEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@PerAd", p.PerAd);
                komut.Parameters.AddWithValue("@PerSoyad", p.PerSoyad);
                komut.Parameters.AddWithValue("@RolID", p.RolID);
                komut.Parameters.AddWithValue("@DogumTarihi", p.DogumTarihi);
                komut.Parameters.AddWithValue("@TC", p.TC);
                komut.Parameters.AddWithValue("@Telefon", p.Telefon);
                komut.Parameters.AddWithValue("@Adres", p.Adres);
                komut.Parameters.AddWithValue("@IseGirisTarihi", p.IseGirisTarihi);
                // NULL olabilir, o yüzden DBNull kontrolü yapıyoruz
                komut.Parameters.AddWithValue("@IstenCikisTarihi",
                    p.IstenCikisTarihi.HasValue ? (object)p.IstenCikisTarihi.Value : DBNull.Value);
                komut.Parameters.AddWithValue("@Sifre", p.Sifre);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Personel eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── PERSONEL LİSTELE ──────────────────────────────────────────────
        // sp_PersonelListele prosedürünü çağırır.
        // Rol adıyla birlikte tüm personeli DataTable olarak döndürür.
        public DataTable PersonelListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_PersonelListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo); // Fill metodu bağlantıyı kendi açıp kapatabilir.
                }
                catch (SqlException ex)
                {
                    throw new Exception("Personeller listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── PERSONEL GÜNCELLE ─────────────────────────────────────────────
        // sp_PersonelGuncelle prosedürünü çağırır.
        public void PersonelGuncelle(Personel p)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_PersonelGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@PerID", p.PerID);
                komut.Parameters.AddWithValue("@PerAd", p.PerAd);
                komut.Parameters.AddWithValue("@PerSoyad", p.PerSoyad);
                komut.Parameters.AddWithValue("@RolID", p.RolID);
                komut.Parameters.AddWithValue("@DogumTarihi", p.DogumTarihi);
                komut.Parameters.AddWithValue("@TC", p.TC);
                komut.Parameters.AddWithValue("@Telefon", p.Telefon);
                komut.Parameters.AddWithValue("@Adres", p.Adres);
                komut.Parameters.AddWithValue("@IseGirisTarihi", p.IseGirisTarihi);
                komut.Parameters.AddWithValue("@IstenCikisTarihi",
                    p.IstenCikisTarihi.HasValue ? (object)p.IstenCikisTarihi.Value : DBNull.Value);
                // komut.Parameters.AddWithValue("@Sifre", p.Sifre);
                komut.Parameters.AddWithValue("@Sifre", p.Sifre);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Personel güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── PERSONEL SİL ──────────────────────────────────────────────────
        // sp_PersonelSil prosedürünü çağırır.
        public void PersonelSil(int perID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_PersonelSil", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@PerID", perID);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Personel silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── ROL LİSTELE ───────────────────────────────────────────────────
        public DataTable RolListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_RolListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Roller listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }
    }
}