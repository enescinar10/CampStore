using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// StokDAL.cs — Veri Katmanı
// StokHareket tablosuna ait tüm işlemler stored procedure ile yapılır.
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

namespace CampStore.DataAccessLayer
{
    public class StokDAL
    {
        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_StokHareketEkle prosedürünü çağırır.
        // Stok girişi veya çıkışı olduğunda yeni bir hareket kaydı oluşturur.
        public void StokHareketEkle(StokHareket s)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_StokHareketEkle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@UrunID", s.UrunID);
                komut.Parameters.AddWithValue("@GirisMiktar", s.GirisMiktar);
                komut.Parameters.AddWithValue("@CikisMiktar", s.CikisMiktar);
                komut.Parameters.AddWithValue("@Tarih", s.Tarih);

                baglanti.Open();
                komut.ExecuteNonQuery();
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_StokHareketListele prosedürünü çağırır.
        // Ürün adıyla birlikte tüm stok hareketlerini döndürür.
        public DataTable StokHareketListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_StokHareketListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            return tablo;
        }

        // ── ÜRÜNE GÖRE LİSTELE ───────────────────────────────────────────
        // sp_StokHareketListeleByUrun prosedürünü çağırır.
        // Belirli bir ürünün stok geçmişini gösterir.
        public DataTable StokHareketListeleByUrun(int urunID)
        {
            DataTable tablo = new DataTable();
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                // sp_StokHareketListeleByUrun değil sp_StokHareketListele kullan
                // çünkü o SP veritabanında yok, filtreyi C# tarafında yapıyoruz
                SqlCommand komut = new SqlCommand("sp_StokHareketListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            // C# tarafında UrunID'ye göre filtrele
            DataView view = tablo.DefaultView;
            view.RowFilter = $"UrunID = {urunID}";
            return view.ToTable();
        }
        //public DataTable StokHareketListeleByUrun(int urunID)
        //{
        //    DataTable tablo = new DataTable();

        //    using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
        //    {
        //        SqlCommand komut = new SqlCommand("sp_StokHareketListeleByUrun", baglanti);
        //        komut.CommandType = CommandType.StoredProcedure;

        //        komut.Parameters.AddWithValue("@UrunID", urunID);

        //        SqlDataAdapter adapter = new SqlDataAdapter(komut);
        //        adapter.Fill(tablo);
        //    }

        //    return tablo;
        //}

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_StokHareketGetirById prosedürünü çağırır.
        public StokHareket StokHareketGetirById(int stokID)
        {
            StokHareket stok = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_StokHareketGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@StokID", stokID);

                baglanti.Open();
                SqlDataReader reader = komut.ExecuteReader();

                if (reader.Read())
                {
                    stok = new StokHareket
                    {
                        StokID = Convert.ToInt32(reader["StokID"]),
                        UrunID = Convert.ToInt32(reader["UrunID"]),
                        GirisMiktar = Convert.ToInt32(reader["GirisMiktar"]),
                        CikisMiktar = Convert.ToInt32(reader["CikisMiktar"]),
                        Tarih = Convert.ToDateTime(reader["Tarih"])
                    };
                }
            }

            return stok;
        }
    }
}