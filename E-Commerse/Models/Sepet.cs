using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblSepet")]
    public class Sepet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sepetID { get; set; }
        public int urunAdet { get; set; }


        public virtual Urun urunID { get; set; }

        public virtual Kullanici kullaniciID { get; set; }
    }
}