using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// UrunDAL.cs — Veri Katmanı
// Urun tablosuna ait tüm veritabanı işlemleri stored procedure ile yapılır.
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

namespace CampStore.DataAccessLayer
{
    public class UrunDAL
    {
        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_UrunEkle prosedürünü çağırır.
        public void UrunEkle(Urun u)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                // 1. Prosedür adını yeni transaction'lı prosedürünle değiştirdik
                SqlCommand komut = new SqlCommand("sp_UrunEkle_Guvenli", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatID", u.KatID);
                komut.Parameters.AddWithValue("@UrunAdi", u.UrunAdi);
                komut.Parameters.AddWithValue("@Fiyat", u.Fiyat);
                komut.Parameters.AddWithValue("@Stok", u.Stok);
                komut.Parameters.AddWithValue("@Durum", u.Durum);
                komut.Parameters.AddWithValue("@Aciklama", u.Aciklama);

                // 2. Olası ROLLBACK hatalarını yakalamak için Try-Catch ekledik
                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Eğer SQL tarafında hata çıkar ve işlem geri alınırsa uygulama çökmez,
                    // hata mesajını üst katmana (forma) düzgün bir şekilde fırlatırız.
                    throw new Exception("Ürün eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        // sp_UrunGuncelle prosedürünü çağırır.
        public void UrunGuncelle(Urun u)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                // 1. Prosedür adı transaction'lı olanla değiştirildi
                SqlCommand komut = new SqlCommand("sp_UrunGuncelle_Guvenli", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@UrunID", u.UrunID);
                komut.Parameters.AddWithValue("@KatID", u.KatID);
                komut.Parameters.AddWithValue("@UrunAdi", u.UrunAdi);
                komut.Parameters.AddWithValue("@Fiyat", u.Fiyat);
                komut.Parameters.AddWithValue("@Stok", u.Stok);
                komut.Parameters.AddWithValue("@Durum", u.Durum);
                komut.Parameters.AddWithValue("@Aciklama", u.Aciklama);

                // 2. Olası ROLLBACK hatalarını yakalamak için Try-Catch eklendi
                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Ürün güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        // sp_UrunSil prosedürünü çağırır.
        // ── SİL ──────────────────────────────────────────────────────────
        // spUrunSil_Guvenli prosedürünü çağırır.
        public void UrunSil(int urunID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                // 1. Prosedür adı transaction'lı olanla değiştirildi
                SqlCommand komut = new SqlCommand("sp_UrunSil_Guvenli", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@UrunID", urunID);

                // 2. Olası ROLLBACK hatalarını yakalamak için Try-Catch eklendi
                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Ürün silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_UrunListele prosedürünü çağırır.
        // Kategori adıyla birlikte tüm ürünleri DataTable olarak döndürür.
        public DataTable UrunListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_UrunListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            return tablo;
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_UrunGetirById prosedürünü çağırır.
        // Güncelleme formunda seçili ürünü doldurmak için kullanılır.
        public Urun UrunGetirById(int urunID)
        {
            Urun urun = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_UrunGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@UrunID", urunID);

                baglanti.Open();
                SqlDataReader reader = komut.ExecuteReader();

                if (reader.Read())
                {
                    urun = new Urun
                    {
                        UrunID = Convert.ToInt32(reader["UrunID"]),
                        KatID = Convert.ToInt32(reader["KatID"]),
                        UrunAdi = reader["UrunAdi"].ToString(),
                        Fiyat = Convert.ToDecimal(reader["Fiyat"]),
                        Stok = Convert.ToInt32(reader["Stok"]),
                        Durum = Convert.ToBoolean(reader["Durum"]),
                        Aciklama = reader["Aciklama"].ToString()
                    };
                }
            }

            return urun;
        }

        // ── KATEGORİYE GÖRE LİSTELE ──────────────────────────────────────
        // sp_UrunListeleByKategori prosedürünü çağırır.
        // Belirli kategorideki ürünleri filtreler.
        public DataTable UrunListeleByKategori(int katID)
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_UrunListeleByKategori", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@KatID", katID);

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            return tablo;
        }
        // UrunDAL.cs'e ekle — vw_StokDurumu view'ını kullanır
        public DataTable KritikStokListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                // View'dan direkt sorgula
                SqlCommand komut = new SqlCommand(
                    "SELECT * FROM vw_StokDurumu " +
                    "WHERE StokDurumu IN ('Kritik', 'Tükendi', 'Düşük') " +
                    "ORDER BY OncelikSirasi ASC",
                    baglanti);

                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                adapter.Fill(tablo);
            }

            return tablo;
        }
    }
}
