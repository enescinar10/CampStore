// ApiDbBaglanti.cs — Sadece API projesi için bağlantı sınıfı
// .NET 10 uyumlu Microsoft.Data.SqlClient kullanır

using Microsoft.Data.SqlClient;

namespace CampStore.API.Data
{
    public class ApiDbBaglanti
    {
        private static string baglantiDizesi =
            "Server=localhost;Database=eMarketingDB;Integrated Security=True;" +
            "TrustServerCertificate=True;";

        public static SqlConnection BaglantiGetir()
        {
            return new SqlConnection(baglantiDizesi);
        }
    }
}