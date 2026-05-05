using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

// MusteriDAL.cs — Veri Katmanı
// Musteri tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.
namespace CampStore.DataAccessLayer
{
    public class MusteriDAL
    {
        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_MusteriEkle prosedürünü çağırır.
        public void MusteriEkle(Musteri m)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_MusteriEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@Ad", m.Ad);
                komut.Parameters.AddWithValue("@Soyad", m.Soyad);
                komut.Parameters.AddWithValue("@Email", m.Email);
                komut.Parameters.AddWithValue("@Sifre", m.Sifre);
                komut.Parameters.AddWithValue("@Telefon", m.Telefon);
                komut.Parameters.AddWithValue("@Adres", m.Adres);
                komut.Parameters.AddWithValue("@SehirID", m.SehirID);
                komut.Parameters.AddWithValue("@IlceID", m.IlceID);
                komut.Parameters.AddWithValue("@Durum", m.Durum);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Müşteri eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        // sp_MusteriGuncelle prosedürünü çağırır.
        public void MusteriGuncelle(Musteri m)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_MusteriGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@MusteriID", m.MusteriID);
                komut.Parameters.AddWithValue("@Ad", m.Ad);
                komut.Parameters.AddWithValue("@Soyad", m.Soyad);
                komut.Parameters.AddWithValue("@Email", m.Email);
                komut.Parameters.AddWithValue("@Sifre", m.Sifre);
                komut.Parameters.AddWithValue("@Telefon", m.Telefon);
                komut.Parameters.AddWithValue("@Adres", m.Adres);
                komut.Parameters.AddWithValue("@SehirID", m.SehirID);
                komut.Parameters.AddWithValue("@IlceID", m.IlceID);
                komut.Parameters.AddWithValue("@Durum", m.Durum);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Müşteri güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        // sp_MusteriSil prosedürünü çağırır.
        public void MusteriSil(int musteriID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                baglanti.Open();
                SqlTransaction transaction = baglanti.BeginTransaction();

                try
                {
                    // 1. Müşteriye ait satış var mı kontrol et
                    SqlCommand kontrol = new SqlCommand(
                        "SELECT COUNT(*) FROM Satis WHERE MusteriID = @MusteriID",
                        baglanti, transaction);
                    kontrol.Parameters.AddWithValue("@MusteriID", musteriID);

                    int satisSayisi = Convert.ToInt32(kontrol.ExecuteScalar());

                    if (satisSayisi > 0)
                        throw new Exception($"Bu müşteriye ait {satisSayisi} satış kaydı var, silinemez!");

                    // 2. Müşteri profilini sil
                    SqlCommand profilSil = new SqlCommand(
                        "DELETE FROM Musteri_Profil WHERE MusteriID = @MusteriID",
                        baglanti, transaction);
                    profilSil.Parameters.AddWithValue("@MusteriID", musteriID);
                    profilSil.ExecuteNonQuery();

                    // 3. Müşteriyi sil — trigger otomatik log yazar
                    SqlCommand komut = new SqlCommand("sp_MusteriSil", baglanti, transaction);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@MusteriID", musteriID);
                    komut.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Hata durumunda işlemi geri alıyoruz
                    transaction.Rollback();

                    // Eğer hata SQL'den geliyorsa (örneğin kısıtlama hatası)
                    if (ex is SqlException)
                        throw new Exception("Müşteri silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);

                    // Kendi fırlattığımız "satış kaydı var" gibi hatalar için
                    throw new Exception(ex.Message);
                }
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_MusteriListele prosedürünü çağırır.
        // Şehir ve ilçe adlarıyla birlikte tüm müşterileri döndürür.
        public DataTable MusteriListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_MusteriListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo); // Fill metodu bağlantıyı kendisi açıp kapatır
                }
                catch (SqlException ex)
                {
                    throw new Exception("Müşteriler listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_MusteriGetirById prosedürünü çağırır.
        // Güncelleme formunda seçili müşteriyi doldurmak için kullanılır.
        public Musteri MusteriGetirById(int musteriID)
        {
            Musteri musteri = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_MusteriGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@MusteriID", musteriID);

                try
                {
                    baglanti.Open();
                    SqlDataReader reader = komut.ExecuteReader();

                    if (reader.Read())
                    {
                        musteri = new Musteri
                        {
                            MusteriID = Convert.ToInt32(reader["MusteriID"]),
                            Ad = reader["Ad"].ToString(),
                            Soyad = reader["Soyad"].ToString(),
                            Email = reader["Email"].ToString(),
                            Sifre = reader["Sifre"].ToString(),
                            Telefon = reader["Telefon"].ToString(),
                            Adres = reader["Adres"].ToString(),
                            SehirID = Convert.ToInt32(reader["SehirID"]),
                            IlceID = Convert.ToInt32(reader["IlceID"]),
                            Durum = Convert.ToBoolean(reader["Durum"])
                        };
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Müşteri bilgisi getirilirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return musteri;
        }

        // ── ŞEHİR LİSTELE ────────────────────────────────────────────────
        // sp_SehirListele prosedürünü çağırır.
        // Müşteri formundaki şehir combobox'ını doldurmak için kullanılır.
        public DataTable SehirListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SehirListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Şehirler listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── İLÇE LİSTELE ─────────────────────────────────────────────────
        // sp_IlceListeleBySehir prosedürünü çağırır.
        // Seçilen şehre göre ilçe combobox'ını filtreler.
        public DataTable IlceListeleBySehir(int sehirID)
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_IlceListeleBySehir", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SehirID", sehirID);

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("İlçeler listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }
    }
}