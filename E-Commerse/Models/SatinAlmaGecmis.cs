using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblSatinAlmaGecmis")]
    public class SatinAlmaGecmis
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int satinAlmaID { get; set; }
        public DateTime satinAlmaTarih { get; set; }
        public int Adet { get; set; }
        public decimal odenenTutar { get; set; }

       
        public virtual Urun urunID { get; set; }
     
        public virtual Kullanici kullaniciID { get; set; }
    }
}