using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public  class Kategori
    {
        public int KatID { get; set; }
        public string KatAdi { get; set; }
        public int? UstKategoriID { get; set; } // NULL olabilir, o yüzden int?
    }
}
