using CampStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampStore.UI
{
    public static class OturumBilgisi
    {
        // Giriş yapan personel burada saklanır
        public static Personel AktifPersonel { get; set; }

        // Çıkış yapılınca temizlenir
        public static void Temizle()
        {
            AktifPersonel = null;
        }
    }
}
