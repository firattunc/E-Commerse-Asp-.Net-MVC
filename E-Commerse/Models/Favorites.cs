using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblFavorites")]
    public class Favorites
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int favoriID { get; set; }

        public virtual Kullanici kullaniciID { get; set; }
        public virtual Urun urunID { get; set; }
    }
}