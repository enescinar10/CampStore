using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonelController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_PersonelListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("roller")]
        public IActionResult GetRoller()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_RolListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] PersonelModel p)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_PersonelEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@PerAd", p.PerAd);
                    komut.Parameters.AddWithValue("@PerSoyad", p.PerSoyad);
                    komut.Parameters.AddWithValue("@RolID", p.RolID);
                    komut.Parameters.AddWithValue("@DogumTarihi", p.DogumTarihi);
                    komut.Parameters.AddWithValue("@TC", p.TC);
                    komut.Parameters.AddWithValue("@Telefon", p.Telefon);
                    komut.Parameters.AddWithValue("@Adres", p.Adres ?? "");
                    komut.Parameters.AddWithValue("@IseGirisTarihi", p.IseGirisTarihi);
                    komut.Parameters.AddWithValue("@IstenCikisTarihi",
                        p.IstenCikisTarihi.HasValue
                        ? (object)p.IstenCikisTarihi.Value : DBNull.Value);
                    komut.Parameters.AddWithValue("@Sifre", p.Sifre ?? "");
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Personel eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PersonelModel p)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_PersonelGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@PerID", id);
                    komut.Parameters.AddWithValue("@PerAd", p.PerAd);
                    komut.Parameters.AddWithValue("@PerSoyad", p.PerSoyad);
                    komut.Parameters.AddWithValue("@RolID", p.RolID);
                    komut.Parameters.AddWithValue("@DogumTarihi", p.DogumTarihi);
                    komut.Parameters.AddWithValue("@TC", p.TC);
                    komut.Parameters.AddWithValue("@Telefon", p.Telefon);
                    komut.Parameters.AddWithValue("@Adres", p.Adres ?? "");
                    komut.Parameters.AddWithValue("@IseGirisTarihi", p.IseGirisTarihi);
                    komut.Parameters.AddWithValue("@IstenCikisTarihi",
                        p.IstenCikisTarihi.HasValue
                        ? (object)p.IstenCikisTarihi.Value : DBNull.Value);
                    komut.Parameters.AddWithValue("@Sifre", p.Sifre ?? "");
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Personel güncellendi!" });
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
                    SqlCommand komut = new SqlCommand("sp_PersonelSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@PerID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Personel silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class PersonelModel
    {
        public int PerID { get; set; }
        public string PerAd { get; set; }
        public string PerSoyad { get; set; }
        public int RolID { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string TC { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public DateTime IseGirisTarihi { get; set; }
        public DateTime? IstenCikisTarihi { get; set; }
        public string Sifre { get; set; }
    }
}