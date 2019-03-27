using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblKategori")]
    public class Kategori
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kategoriID { get; set; }
        [StringLength(30)]
        public string kategoriAd { get; set; }

    }
}