using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.Entities
{
    public class Fatura
    {
        public int FaturaID { get; set; }
        public int SatisID { get; set; }
        public string FaturaNo { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
    }
}
