using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// DbBaglanti.cs — Veri Katmanı
// Tüm DAL sınıfları bu sınıftan bağlantı alır.
// Bağlantı string'ini sadece buradan yönetirsin,
// değişiklik gerekirse tek yerden düzeltirsin.

using System.Data.SqlClient;

namespace CampStore.DataAccessLayer
{
    public class DbBaglanti
    {
        // SQL Server bağlantı dizesi
        // "." yerine bilgisayar adını veya "localhost" yazabilirsin
        // Integrated Security = Windows kimlik doğrulaması kullan demek
        private static string baglantiDizesi =
            "Server=localhost;Database=eMarketingDB;Integrated Security=True;";

        // Dışarıya SqlConnection nesnesi döndürür
        // Her DAL metodu using bloğunda bunu çağırır
        public static SqlConnection BaglantiGetir()
        {
            return new SqlConnection(baglantiDizesi);
        }
    }
}
