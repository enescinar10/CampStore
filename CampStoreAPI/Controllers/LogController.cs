using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_LogListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] LogModel log)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_LogEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@IslemTuru", log.IslemTuru);
                    komut.Parameters.AddWithValue("@Aciklama", log.Aciklama ?? "");
                    komut.Parameters.AddWithValue("@Tarih", DateTime.Now);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Log eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class LogModel
    {
        public string IslemTuru { get; set; }
        public string Aciklama { get; set; }
    }
}