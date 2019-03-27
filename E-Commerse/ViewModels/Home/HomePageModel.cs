using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Commerse.Models;

namespace E_Commerse.ViewModels.Home
{
    public class HomePageModel
    {
        public List<Urun> urunler { get; set; }
        public List<Kategori> kategoriler { get; set; }
        public Urun seciliUrun { get; set; }
        public Kullanici kullanici { get; set; }
        public List<Favorites> favorites { get; set; }
        public Favorites favori { get; set; }
        public List<Sepet> sepet { get; set; }

    }
}