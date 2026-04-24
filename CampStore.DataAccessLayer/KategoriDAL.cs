using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// KategoriDAL.cs — Veri Katmanı
// Kategori tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.

using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

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

                baglanti.Open();
                komut.ExecuteNonQuery();
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

                baglanti.Open();
                komut.ExecuteNonQuery();
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

                baglanti.Open();
                komut.ExecuteNonQuery();
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

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
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

            return kategori;
        }
    }
}
