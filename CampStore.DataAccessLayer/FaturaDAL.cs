using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// FaturaDAL.cs — Veri Katmanı
// Fatura tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

namespace CampStore.DataAccessLayer
{
    public class FaturaDAL
    {
        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_FaturaEkle prosedürünü çağırır.
        public void FaturaEkle(Fatura f)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", f.SatisID);
                komut.Parameters.AddWithValue("@FaturaNo", f.FaturaNo);
                komut.Parameters.AddWithValue("@Tarih", f.Tarih);
                komut.Parameters.AddWithValue("@Tutar", f.Tutar);

                baglanti.Open();
                komut.ExecuteNonQuery();
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        // sp_FaturaGuncelle prosedürünü çağırır.
        public void FaturaGuncelle(Fatura f)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@FaturaID", f.FaturaID);
                komut.Parameters.AddWithValue("@SatisID", f.SatisID);
                komut.Parameters.AddWithValue("@FaturaNo", f.FaturaNo);
                komut.Parameters.AddWithValue("@Tarih", f.Tarih);
                komut.Parameters.AddWithValue("@Tutar", f.Tutar);

                baglanti.Open();
                komut.ExecuteNonQuery();
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        // sp_FaturaSil prosedürünü çağırır.
        public void FaturaSil(int faturaID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaSil", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@FaturaID", faturaID);

                baglanti.Open();
                komut.ExecuteNonQuery();
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_FaturaListele prosedürünü çağırır.
        // Satış bilgisiyle birlikte tüm faturaları döndürür.
        public DataTable FaturaListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            return tablo;
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_FaturaGetirById prosedürünü çağırır.
        public Fatura FaturaGetirById(int faturaID)
        {
            Fatura fatura = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@FaturaID", faturaID);

                baglanti.Open();
                SqlDataReader reader = komut.ExecuteReader();

                if (reader.Read())
                {
                    fatura = new Fatura
                    {
                        FaturaID = Convert.ToInt32(reader["FaturaID"]),
                        SatisID = Convert.ToInt32(reader["SatisID"]),
                        FaturaNo = reader["FaturaNo"].ToString(),
                        Tarih = Convert.ToDateTime(reader["Tarih"]),
                        Tutar = Convert.ToDecimal(reader["Tutar"])
                    };
                }
            }

            return fatura;
        }

        // ── SATIŞA GÖRE GETİR ────────────────────────────────────────────
        // sp_FaturaGetirBySatis prosedürünü çağırır.
        // Satış tamamlanınca faturayı çekmek için kullanılır.
        public Fatura FaturaGetirBySatis(int satisID)
        {
            Fatura fatura = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_FaturaGetirBySatis", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", satisID);

                baglanti.Open();
                SqlDataReader reader = komut.ExecuteReader();

                if (reader.Read())
                {
                    fatura = new Fatura
                    {
                        FaturaID = Convert.ToInt32(reader["FaturaID"]),
                        SatisID = Convert.ToInt32(reader["SatisID"]),
                        FaturaNo = reader["FaturaNo"].ToString(),
                        Tarih = Convert.ToDateTime(reader["Tarih"]),
                        Tutar = Convert.ToDecimal(reader["Tutar"])
                    };
                }
            }

            return fatura;
        }
    }
}
