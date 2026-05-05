using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CampStore.API.Data;

namespace CampStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // ── POST api/auth/login ───────────────────────────────────────
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { mesaj = "Geçersiz istek!" });

                DataTable tablo = new DataTable();
                using (SqlConnection baglanti = ApiDbBaglanti.BaglantiGetir())
                {
                    SqlCommand komut = new SqlCommand("sp_PersonelLoginKontrol", baglanti);
                    komut.CommandType = CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@GirisBilgisi", model.GirisBilgisi);
                    komut.Parameters.AddWithValue("@Sifre", model.Sifre);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(tablo);
                }

                if (tablo.Rows.Count == 0)
                    return Unauthorized(new
                    {
                        basarili = false,
                        mesaj = "Kullanıcı bilgisi veya şifre hatalı!"
                    });

                DataRow row = tablo.Rows[0];
                return Ok(new
                {
                    basarili = true,
                    mesaj = "Giriş başarılı!",
                    personel = new
                    {
                        perID = Convert.ToInt32(row["PerID"]),
                        perAd = row["PerAd"].ToString(),
                        perSoyad = row["PerSoyad"].ToString(),
                        rolID = Convert.ToInt32(row["RolID"]),
                        rolAdi = row["RolAdi"].ToString(),
                        kullaniciAdi = row["KullaniciAdi"].ToString(),
                        tc = row["TC"].ToString(),
                        telefon = row["Telefon"].ToString(),
                        iseGirisTarihi = Convert.ToDateTime(row["IseGirisTarihi"])
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { hata = ex.Message });
            }
        }
    }

    public class LoginModel
    {
        public string GirisBilgisi { get; set; }
        public string Sifre { get; set; }
    }
}