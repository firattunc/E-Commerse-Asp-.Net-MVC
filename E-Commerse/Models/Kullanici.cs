using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblKullanici")]
    public class Kullanici
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kullaniciID { get; set; }
        [StringLength(40)]
        public string kullaniciAd { get; set; }
        [StringLength(40)]
        public string kullaniciSoyAd { get; set; }
        [StringLength(40)]
        public string email { get; set; }
        [StringLength(30)]
        public string kullaniciSifre  { get; set; }
        [StringLength(70)]
        public string Adres { get; set; }
        [StringLength(70)]
        public string Country { get; set; }
        public DateTime memberSince { get; set; }


    }
}