using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CampStore.Entities;

// SatisDAL.cs — Veri Katmanı
// Satis ve SatisDetay tablosuna ait tüm işlemler stored procedure ile yapılır.
namespace CampStore.DataAccessLayer
{
    public class SatisDAL
    {
        // ═══════════════════════════════════════════════════════════════
        // SATIŞ İŞLEMLERİ
        // ═══════════════════════════════════════════════════════════════

        // ── EKLE ─────────────────────────────────────────────────────────
        // sp_SatisEkle prosedürünü çağırır.
        // Yeni satış kaydı oluşturur, oluşan SatisID'yi geri döndürür.
        // SatisDetay eklemek için bu ID'ye ihtiyaç var.
        public int SatisEkle(Satis s)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                baglanti.Open();
                SqlTransaction transaction = baglanti.BeginTransaction();

                try
                {
                    // 1. Satışı ekle
                    SqlCommand komut = new SqlCommand("sp_SatisEkle", baglanti, transaction);
                    komut.CommandType = CommandType.StoredProcedure;

                    komut.Parameters.AddWithValue("@MusteriID", s.MusteriID);
                    komut.Parameters.AddWithValue("@PerID", s.PerID);
                    komut.Parameters.AddWithValue("@ToplamTutar", s.ToplamTutar);
                    komut.Parameters.AddWithValue("@Durum", s.Durum);

                    SqlParameter outputParam = new SqlParameter("@YeniSatisID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    komut.Parameters.Add(outputParam);

                    komut.ExecuteNonQuery();
                    int yeniSatisID = Convert.ToInt32(outputParam.Value);

                    // Transaction başarılı — onayla
                    transaction.Commit();

                    return yeniSatisID;
                }
                catch (Exception ex)
                {
                    // Hata olursa geri al
                    transaction.Rollback();

                    if (ex is SqlException)
                        throw new Exception("Satış eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);

                    throw new Exception(ex.Message);
                }
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        // sp_SatisGuncelle prosedürünü çağırır.
        public void SatisGuncelle(Satis s)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", s.SatisID);
                komut.Parameters.AddWithValue("@MusteriID", s.MusteriID);
                komut.Parameters.AddWithValue("@PerID", s.PerID);
                komut.Parameters.AddWithValue("@ToplamTutar", s.ToplamTutar);
                komut.Parameters.AddWithValue("@Durum", s.Durum);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satış güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        // sp_SatisSil prosedürünü çağırır.
        public void SatisSil(int satisID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisSil", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", satisID);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satış silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        // sp_SatisListele prosedürünü çağırır.
        // Müşteri ve personel adıyla birlikte tüm satışları döndürür.
        public DataTable SatisListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satışlar listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        // sp_SatisGetirById prosedürünü çağırır.
        public Satis SatisGetirById(int satisID)
        {
            Satis satis = null;

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisGetirById", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", satisID);

                try
                {
                    baglanti.Open();
                    SqlDataReader reader = komut.ExecuteReader();

                    if (reader.Read())
                    {
                        satis = new Satis
                        {
                            SatisID = Convert.ToInt32(reader["SatisID"]),
                            MusteriID = Convert.ToInt32(reader["MusteriID"]),
                            PerID = Convert.ToInt32(reader["PerID"]),
                            SatisTarihi = Convert.ToDateTime(reader["SatisTarihi"]),
                            ToplamTutar = Convert.ToDecimal(reader["ToplamTutar"]),
                            Durum = reader["Durum"].ToString()
                        };
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satış bilgisi getirilirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return satis;
        }

        // ── DURUMA GÖRE LİSTELE ───────────────────────────────────────────
        // sp_SatisListeleByDurum prosedürünü çağırır.
        // "Hazırlanıyor", "Kargoda", "Teslim Edildi" gibi durumlara göre filtreler.
        public DataTable SatisListeleByDurum(string durum)
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisListeleByDurum", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@Durum", durum);

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satışlar duruma göre listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ═══════════════════════════════════════════════════════════════
        // SATIŞ DETAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════════════

        // ── DETAY EKLE ────────────────────────────────────────────────────
        // sp_SatisDetayEkle prosedürünü çağırır.
        // Bir satışa ait ürün satırı ekler.
        public void SatisDetayEkle(SatisDetay sd)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                baglanti.Open();
                SqlTransaction transaction = baglanti.BeginTransaction();

                try
                {
                    // 1. Detayı ekle — trigger otomatik stok düşer
                    SqlCommand komut = new SqlCommand("sp_SatisDetayEkle", baglanti, transaction);
                    komut.CommandType = CommandType.StoredProcedure;

                    komut.Parameters.AddWithValue("@SatisID", sd.SatisID);
                    komut.Parameters.AddWithValue("@UrunID", sd.UrunID);
                    komut.Parameters.AddWithValue("@Miktar", sd.Miktar);
                    komut.Parameters.AddWithValue("@BirimFiyat", sd.BirimFiyat);

                    komut.ExecuteNonQuery();

                    // 2. Stok kontrolü — negatife düştü mü?
                    SqlCommand stokKomut = new SqlCommand(
                        "SELECT Stok FROM Urun WHERE UrunID = @UrunID",
                        baglanti, transaction);
                    stokKomut.Parameters.AddWithValue("@UrunID", sd.UrunID);

                    int mevcutStok = Convert.ToInt32(stokKomut.ExecuteScalar());

                    if (mevcutStok < 0)
                    {
                        // Stok negatife düştü — geri al ve kendi hatamızı fırlat
                        transaction.Rollback();
                        throw new Exception("Yetersiz stok! Mevcut stok: " + (mevcutStok + sd.Miktar));
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        transaction.Rollback();
                    }

                    if (ex is SqlException)
                        throw new Exception("Satış detayı eklenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);

                    throw new Exception(ex.Message);
                }
            }
        }

        // ── DETAY GÜNCELLE ────────────────────────────────────────────────
        // sp_SatisDetayGuncelle prosedürünü çağırır.
        public void SatisDetayGuncelle(SatisDetay sd)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisDetayGuncelle", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@DetayID", sd.DetayID);
                komut.Parameters.AddWithValue("@SatisID", sd.SatisID);
                komut.Parameters.AddWithValue("@UrunID", sd.UrunID);
                komut.Parameters.AddWithValue("@Miktar", sd.Miktar);
                komut.Parameters.AddWithValue("@BirimFiyat", sd.BirimFiyat);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satış detayı güncellenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── DETAY SİL ─────────────────────────────────────────────────────
        // sp_SatisDetaySil prosedürünü çağırır.
        public void SatisDetaySil(int detayID)
        {
            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisDetaySil", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@DetayID", detayID);

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satış detayı silinirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ── DETAY LİSTELE (SATIŞA GÖRE) ──────────────────────────────────
        // sp_SatisDetayListeleBySatis prosedürünü çağırır.
        // Belirli bir satışa ait tüm ürün detaylarını döndürür.
        public DataTable SatisDetayListeleBySatis(int satisID)
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisDetayListeleBySatis", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                komut.Parameters.AddWithValue("@SatisID", satisID);

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Satışa ait detaylar listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }

        // ── TÜM DETAYLARI LİSTELE ─────────────────────────────────────────
        // sp_SatisDetayListele prosedürünü çağırır.
        public DataTable SatisDetayListele()
        {
            DataTable tablo = new DataTable();

            using (SqlConnection baglanti = DbBaglanti.BaglantiGetir())
            {
                SqlCommand komut = new SqlCommand("sp_SatisDetayListele", baglanti);
                komut.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Tüm satış detayları listelenirken veritabanı kaynaklı bir hata oluştu: " + ex.Message);
                }
            }

            return tablo;
        }
    }
}