using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// LogBL.cs — İş Katmanı
// Log işlemleri için yardımcı metotlar burada tanımlanır.
// Tüm BL sınıfları önemli işlemlerden sonra LogEkle'yi çağırabilir.
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class LogBL
    {
        private LogDAL logDAL = new LogDAL();

        // ── LOG EKLE ─────────────────────────────────────────────────────
        // Herhangi bir BL sınıfından kolayca çağrılabilir.
        // Örnek: logBL.LogEkle("GİRİŞ", "Admin giriş yaptı");
        public void LogEkle(string islemTuru, string aciklama)
        {
            try
            {
                LogKayit log = new LogKayit
                {
                    IslemTuru = islemTuru,
                    Aciklama = aciklama,
                    Tarih = DateTime.Now // Tarih her zaman şimdiki zaman
                };

                logDAL.LogEkle(log);
            }
            catch
            {
                // Log hatası uygulamayı durdurmamalı, sessizce geçiyoruz
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable LogListele()
        {
            return logDAL.LogListele();
        }
    }
}
