// UrunController.cs — API için yeniden yazıldı
// Direkt SP çağırır, DAL/BL kullanmaz

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrunController : ControllerBase
    {
        // ── GET api/urun ──────────────────────────────────────────────
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }

        // ── GET api/urun/{id} ─────────────────────────────────────────
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunGetirById", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@UrunID", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }

                if (tablo.Rows.Count == 0)
                    return NotFound(new { mesaj = "Ürün bulunamadı!" });

                return Ok(tablo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }

        // ── GET api/urun/kategori/{katId} ─────────────────────────────
        [HttpGet("kategori/{katId}")]
        public IActionResult GetByKategori(int katId)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunListeleByKategori", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatID", katId);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }

        // ── POST api/urun ─────────────────────────────────────────────
        [HttpPost]
        public IActionResult Post([FromBody] UrunModel urun)
        {
            try
            {
                if (urun == null)
                    return BadRequest(new { mesaj = "Geçersiz veri!" });

                if (string.IsNullOrWhiteSpace(urun.UrunAdi))
                    return BadRequest(new { mesaj = "Ürün adı boş olamaz!" });

                if (urun.Fiyat <= 0)
                    return BadRequest(new { mesaj = "Fiyat sıfırdan büyük olmalı!" });

                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatID", urun.KatID);
                    komut.Parameters.AddWithValue("@UrunAdi", urun.UrunAdi);
                    komut.Parameters.AddWithValue("@Fiyat", urun.Fiyat);
                    komut.Parameters.AddWithValue("@Stok", urun.Stok);
                    komut.Parameters.AddWithValue("@Durum", urun.Durum);
                    komut.Parameters.AddWithValue("@Aciklama", urun.Aciklama ?? "");
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }

                return Created("", new { mesaj = "Ürün eklendi!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }

        // ── PUT api/urun/{id} ─────────────────────────────────────────
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UrunModel urun)
        {
            try
            {
                if (urun == null)
                    return BadRequest(new { mesaj = "Geçersiz veri!" });

                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@UrunID", id);
                    komut.Parameters.AddWithValue("@KatID", urun.KatID);
                    komut.Parameters.AddWithValue("@UrunAdi", urun.UrunAdi);
                    komut.Parameters.AddWithValue("@Fiyat", urun.Fiyat);
                    komut.Parameters.AddWithValue("@Stok", urun.Stok);
                    komut.Parameters.AddWithValue("@Durum", urun.Durum);
                    komut.Parameters.AddWithValue("@Aciklama", urun.Aciklama ?? "");
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }

                return Ok(new { mesaj = "Ürün güncellendi!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }

        // ── DELETE api/urun/{id} ──────────────────────────────────────
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_UrunSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@UrunID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }

                return Ok(new { mesaj = "Ürün silindi!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }
    }

    // ── MODEL SINIFI ──────────────────────────────────────────────────
    public class UrunModel
    {
        public int UrunID { get; set; }
        public int KatID { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
        public string Aciklama { get; set; }
    }
}