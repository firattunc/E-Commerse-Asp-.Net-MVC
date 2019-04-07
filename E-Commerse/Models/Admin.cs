using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblAdmin")]
    public class Admin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int adminID { get; set; }
        [StringLength(40)]
        public string adminEmail { get; set; }
        [StringLength(30)]
        public string adminSifre { get; set; }
        public string duyuru { get; set; }
        public string adminAd { get; set; }
        public int duyuruGoster { get; set; }
        public int uyeKayitSayi { get; set; }

    }
}