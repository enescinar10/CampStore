using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturaController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_FaturaListele", baglanti);
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
                    SqlCommand komut = new SqlCommand("sp_FaturaGetirById", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@FaturaID", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                if (tablo.Rows.Count == 0)
                    return NotFound(new { mesaj = "Fatura bulunamadı!" });
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("otomatikno")]
        public IActionResult GetOtomatikNo()
        {
            try
            {
                string no = "FAT-" + DateTime.Now.ToString("yyyyMMdd") +
                            "-" + DateTime.Now.ToString("HHmmss");
                return Ok(new { faturaNo = no });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] FaturaModel f)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_FaturaEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", f.SatisID);
                    komut.Parameters.AddWithValue("@FaturaNo", f.FaturaNo);
                    komut.Parameters.AddWithValue("@Tutar", f.Tutar);
                    // @Tarih yok SP'de — kaldırıldı
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Fatura eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] FaturaModel f)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_FaturaGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@FaturaID", id);
                    komut.Parameters.AddWithValue("@SatisID", f.SatisID);
                    komut.Parameters.AddWithValue("@FaturaNo", f.FaturaNo);
                    komut.Parameters.AddWithValue("@Tutar", f.Tutar);
                    // @Tarih yok SP'de — kaldırıldı
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Fatura güncellendi!" });
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
                    SqlCommand komut = new SqlCommand("sp_FaturaSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@FaturaID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Fatura silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class FaturaModel
    {
        public int FaturaID { get; set; }
        public int SatisID { get; set; }
        public string FaturaNo { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
    }
}