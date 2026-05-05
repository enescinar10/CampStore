using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusteriController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_MusteriListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_MusteriGetirById", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@MusteriID", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                if (tablo.Rows.Count == 0)
                    return NotFound(new { mesaj = "Müşteri bulunamadı!" });
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("sehir")]
        public IActionResult GetSehirler()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SehirListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("ilce/{sehirId}")]
        public IActionResult GetIlceler(int sehirId)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_IlceListeleBySehir", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SehirID", sehirId);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] MusteriModel m)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(m.Ad))
                    return BadRequest(new { mesaj = "Ad boş olamaz!" });

                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_MusteriEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@Ad", m.Ad);
                    komut.Parameters.AddWithValue("@Soyad", m.Soyad);
                    komut.Parameters.AddWithValue("@Email", m.Email);
                    komut.Parameters.AddWithValue("@Sifre", m.Sifre);
                    komut.Parameters.AddWithValue("@Telefon", m.Telefon);
                    komut.Parameters.AddWithValue("@Adres", m.Adres ?? "");
                    komut.Parameters.AddWithValue("@SehirID", m.SehirID);
                    komut.Parameters.AddWithValue("@IlceID", m.IlceID);
                    komut.Parameters.AddWithValue("@Durum", m.Durum);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Müşteri eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MusteriModel m)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_MusteriGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@MusteriID", id);
                    komut.Parameters.AddWithValue("@Ad", m.Ad);
                    komut.Parameters.AddWithValue("@Soyad", m.Soyad);
                    komut.Parameters.AddWithValue("@Email", m.Email);
                    komut.Parameters.AddWithValue("@Sifre", m.Sifre);
                    komut.Parameters.AddWithValue("@Telefon", m.Telefon);
                    komut.Parameters.AddWithValue("@Adres", m.Adres ?? "");
                    komut.Parameters.AddWithValue("@SehirID", m.SehirID);
                    komut.Parameters.AddWithValue("@IlceID", m.IlceID);
                    komut.Parameters.AddWithValue("@Durum", m.Durum);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Müşteri güncellendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_MusteriSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@MusteriID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Müşteri silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class MusteriModel
    {
        public int MusteriID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public int SehirID { get; set; }
        public int IlceID { get; set; }
        public bool Durum { get; set; }
    }
}