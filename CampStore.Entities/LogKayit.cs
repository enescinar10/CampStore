using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class LogKayit
    {
        public int LogID { get; set; }
        public string IslemTuru { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
    }
}
