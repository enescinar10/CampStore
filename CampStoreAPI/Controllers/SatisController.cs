using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SatisController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisListele", baglanti);
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
                    SqlCommand komut = new SqlCommand("sp_SatisGetirById", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                if (tablo.Rows.Count == 0)
                    return NotFound(new { mesaj = "Satış bulunamadı!" });
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpGet("durum/{durum}")]
        public IActionResult GetByDurum(string durum)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisListeleByDurum", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@Durum", durum);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post([FromBody] SatisModel s)
        {
            try
            {
                int yeniSatisID = 0;
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("sp_SatisEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@MusteriID", s.MusteriID);
                    komut.Parameters.AddWithValue("@PerID", s.PerID);
                    komut.Parameters.AddWithValue("@ToplamTutar", s.ToplamTutar);
                    komut.Parameters.AddWithValue("@Durum", s.Durum ?? "Hazırlanıyor");

                    SqlParameter output = new SqlParameter("@YeniSatisID", SqlDbType.Int);
                    output.Direction = ParameterDirection.Output;
                    komut.Parameters.Add(output);

                    komut.ExecuteNonQuery();
                    yeniSatisID = Convert.ToInt32(output.Value);
                }
                return Created("", new { mesaj = "Satış oluşturuldu!", satisID = yeniSatisID });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SatisModel s)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisGuncelle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", id);
                    komut.Parameters.AddWithValue("@MusteriID", s.MusteriID);
                    komut.Parameters.AddWithValue("@PerID", s.PerID);
                    komut.Parameters.AddWithValue("@ToplamTutar", s.ToplamTutar);
                    komut.Parameters.AddWithValue("@Durum", s.Durum ?? "Hazırlanıyor");
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Satış güncellendi!" });
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
                    SqlCommand komut = new SqlCommand("sp_SatisSil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", id);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Satış silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        // ── Satış Detay ───────────────────────────────────────────────
        [HttpGet("{satisId}/detay")]
        public IActionResult GetDetay(int satisId)
        {
            try
            {
                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisDetayListeleBySatis", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", satisId);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }
                return Ok(tablo);
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpPost("detay")]
        public IActionResult PostDetay([FromBody] SatisDetayModel sd)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisDetayEkle", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@SatisID", sd.SatisID);
                    komut.Parameters.AddWithValue("@UrunID", sd.UrunID);
                    komut.Parameters.AddWithValue("@Miktar", sd.Miktar);
                    komut.Parameters.AddWithValue("@BirimFiyat", sd.BirimFiyat);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Created("", new { mesaj = "Detay eklendi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }

        [HttpDelete("detay/{detayId}")]
        public IActionResult DeleteDetay(int detayId)
        {
            try
            {
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_SatisDetaySil", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@DetayID", detayId);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
                return Ok(new { mesaj = "Detay silindi!" });
            }
            catch (Exception ex) { return StatusCode(500, new { hata = ex.Message }); }
        }
    }

    public class SatisModel
    {
        public int SatisID { get; set; }
        public int MusteriID { get; set; }
        public int PerID { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; }
    }

    public class SatisDetayModel
    {
        public int DetayID { get; set; }
        public int SatisID { get; set; }
        public int UrunID { get; set; }
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
    }
}