using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class Satis
    {
        public int SatisID { get; set; }
        public int MusteriID { get; set; }
        public int PerID { get; set; }
        public DateTime SatisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } // "Hazırlanıyor", "Kargoda", "Teslim Edildi"
    }
}
