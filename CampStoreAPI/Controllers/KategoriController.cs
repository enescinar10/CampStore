using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KategoriController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_KategoriListele", baglanti);
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
                    SqlCommand komut = new SqlCommand("sp_KategoriGetirById", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatID", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                if (tablo.Rows.Count == 0)
                    return NotFound(new { mesaj = "Kategori bulunamadı!" });
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] KategoriModel k)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(k.KatAdi))
                    return BadRequest(new { mesaj = "Kategori adı boş olamaz!" });

                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_KategoriEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatAdi", k.KatAdi);
                    komut.Parameters.AddWithValue("@UstKategoriID",
                        k.UstKategoriID.HasValue ? (object)k.UstKategoriID.Value : DBNull.Value);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Kategori eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] KategoriModel k)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_KategoriGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatID", id);
                    komut.Parameters.AddWithValue("@KatAdi", k.KatAdi);
                    komut.Parameters.AddWithValue("@UstKategoriID",
                        k.UstKategoriID.HasValue ? (object)k.UstKategoriID.Value : DBNull.Value);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Kategori güncellendi!" });
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
                    SqlCommand komut = new SqlCommand("sp_KategoriSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@KatID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Kategori silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class KategoriModel
    {
        public int KatID { get; set; }
        public string KatAdi { get; set; }
        public int? UstKategoriID { get; set; }
    }
}