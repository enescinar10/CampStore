using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StokController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_StokHareketListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("urun/{urunId}")]
        public IActionResult GetByUrun(int urunId)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_StokHareketListele", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                // C# tarafında filtrele
                DataView view = tablo.DefaultView;
                view.RowFilter = $"UrunID = {urunId}";
                return Ok(view.ToTable());
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] StokModel s)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
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
                return Created("", new { mesaj = "Stok hareketi eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class StokModel
    {
        public int StokID { get; set; }
        public int UrunID { get; set; }
        public int GirisMiktar { get; set; }
        public int CikisMiktar { get; set; }
        public DateTime Tarih { get; set; }
    }
}