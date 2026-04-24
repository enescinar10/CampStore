using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class StokHareket
    {
        public int StokID { get; set; }
        public int UrunID { get; set; }
        public int GirisMiktar { get; set; }
        public int CikisMiktar { get; set; }
        public DateTime Tarih { get; set; }
    }
}
