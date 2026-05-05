using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

// KategoriDAL.cs — Veri Katmanı
// Kategori tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.
namespace CampStore.DataAccessLayer
{
    public class KategoriDAL
    {
        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_KategoriEkle prosedürünü çağırır.
        public void KategoriEkle(Kategori k)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_KategoriEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatAdi", k.KatAdi);
                // UstKategoriID null olabilir (ana kategori ise null gelir)
                komut.Parameters.AddWithValue("@UstKategoriID",
                    k.UstKategoriID.HasValue ? (object)k.UstKategoriID.Value : DBNull.Value);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Kategori eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        // sp_KategoriGuncelle prosedürünü çağırır.
        public void KategoriGuncelle(Kategori k)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_KategoriGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatID", k.KatID);
                komut.Parameters.AddWithValue("@KatAdi", k.KatAdi);
                komut.Parameters.AddWithValue("@UstKategoriID",
                    k.UstKategoriID.HasValue ? (object)k.UstKategoriID.Value : DBNull.Value);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Kategori güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        // sp_KategoriSil prosedürünü çağırır.
        public void KategoriSil(int katID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_KategoriSil", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatID", katID);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Kategori silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_KategoriListele prosedürünü çağırır.
        // Tüm kategorileri DataTable olarak döndürür.
        public DataTable KategoriListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_KategoriListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo); // Fill metodu bağlantıyı kendisi açıp kapatır.
                }
                catch (SqlException ex)
                {
                    throw new Exception("Kategoriler listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_KategoriGetirById prosedürünü çağırır.
        // Güncelleme formunda seçili kaydı doldurmak için kullanılır.
        public Kategori KategoriGetirById(int katID)
        {
            Kategori kategori = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_KategoriGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatID", katID);

                try
                {
                    baglanti.Open();
                    SqlDataReader reader = komut.ExecuteReader();

                    if (reader.Read())
                    {
                        kategori = new Kategori
                        {
                            KatID = (int)reader["KatID"],
                            KatAdi = reader["KatAdi"].ToString(),
                            // DBNull kontrolü: üst kategori yoksa null ata
                            UstKategoriID = reader["UstKategoriID"] == DBNull.Value
                                            ? (int?)null
                                            : (int)reader["UstKategoriID"]
                        };
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Kategori bilgisi getirilirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return kategori;
        }
    }
}