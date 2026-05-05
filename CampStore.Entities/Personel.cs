using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class Personel
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
        public string Sifre { get; set; } // ← Bu satırı ekle

        // Personel.cs'e ekle
        public string KullaniciAdi { get; set; }
        public string RolAdi { get; set; }
    }
}
