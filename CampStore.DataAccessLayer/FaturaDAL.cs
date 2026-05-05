using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

// FaturaDAL.cs — Veri Katmanı
// Fatura tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.
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
               // komut.Parameters.AddWithValue("@Tarih", f.Tarih);
                komut.Parameters.AddWithValue("@Tutar", f.Tutar);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Fatura eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
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
                //komut.Parameters.AddWithValue("@Tarih", f.Tarih);
                komut.Parameters.AddWithValue("@Tutar", f.Tutar);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Fatura güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
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

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Fatura silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
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

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo); // Fill metodu bağlantıyı kendisi açıp kapatır.
                }
                catch (SqlException ex)
                {
                    throw new Exception("Faturalar listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
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

                try
                {
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
                catch (SqlException ex)
                {
                    throw new Exception("Fatura bilgisi getirilirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
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

                try
                {
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
                catch (SqlException ex)
                {
                    throw new Exception("Satışa ait fatura bilgisi getirilirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return fatura;
        }
    }
}