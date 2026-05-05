using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

// LogDAL.cs — Veri Katmanı
// LogKayit tablosuna ait tüm işlemler stored procedure ile yapılır.
// Sistemdeki önemli işlemler otomatik olarak loglanır.
namespace CampStore.DataAccessLayer
{
    public class LogDAL
    {
        // ── LOG EKLE ─────────────────────────────────────────────────────
        // sp_LogKayitEkle prosedürünü çağırır.
        // Giriş, ekleme, silme gibi önemli işlemlerde çağrılır.
        public void LogEkle(LogKayit log)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_LogEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@IslemTuru", log.IslemTuru);
                komut.Parameters.AddWithValue("@Aciklama", log.Aciklama);
                komut.Parameters.AddWithValue("@Tarih", log.Tarih);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Log eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_LogKayitListele prosedürünü çağırır.
        // Tüm log kayıtlarını tarihe göre sıralı döndürür.
        public DataTable LogListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_LogListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo); // Fill metodu bağlantıyı kendisi açıp kapatır.
                }
                catch (SqlException ex)
                {
                    throw new Exception("Log kayıtları listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }
    }
}