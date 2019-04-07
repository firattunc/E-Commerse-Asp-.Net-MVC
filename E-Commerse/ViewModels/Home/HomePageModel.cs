using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Commerse.Models;
using System.Collections;

namespace E_Commerse.ViewModels.Home
{
    public class HomePageModel
    {
        
        public List<Urun> urunler { get; set; }
        public List<Kategori> kategoriler { get; set; }
        public Urun seciliUrun { get; set; }
        public Kategori kategori { get; set; }
        public Kullanici kullanici { get; set; }
        public Urun urunDropdown { get; set; }
        public List<Favorites> favorites { get; set; }
        public Favorites favori { get; set; }
        public List<Sepet> sepet { get; set; }
        public List<SatinAlmaGecmis> satinAlmaGecmis { get; set; }
        public Admin admin { get; set; }
        public int showModal { get; set; }
        public int ziyaretSayisiGünlük { get; set; }
        public int ziyaretSayisiHaftalik { get; set; }
        public int ziyaretSayisiToplam{ get; set; }
        public int urunSayisiGünlük { get; set; }
        public int urunSayisiHaftalik { get; set; }
        public int urunSayisiToplam { get; set; }
        public int uyeKayitSayisiGünlük { get; set; }
        public int uyeKayitSayisiHaftalik { get; set; }
        public int uyeKayitSayisiToplam { get; set; }
        public List<String> liste = new List<string>();
        public string enCokSatisKategori { get; set; }
        public List<string> enCokSatisUrun = new List<string>();
        public List<string> enCokSepetUrun = new List<string>();
        public List<Urun> enUcuzUrunler { get; set; }
    }
}