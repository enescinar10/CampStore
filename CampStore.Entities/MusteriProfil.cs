using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class MusteriProfil
    {
        public int ProfilID { get; set; }
        public int MusteriID { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; } // NULL olabilir
    }
}
