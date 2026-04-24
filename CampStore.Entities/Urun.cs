using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class Urun
    {
        public int UrunID { get; set; }
        public int KatID { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
        public string Aciklama { get; set; }
    }
}
