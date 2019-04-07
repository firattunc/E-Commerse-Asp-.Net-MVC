using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblUrun")]
    public class Urun
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int urunID { get; set; }
        [StringLength(30)]
        public string urunAd { get; set; }
        public decimal urunFiyat { get; set; }
        public int urunStok { get; set; }
        [StringLength(150)]
        public string urunAciklama { get; set; }
        public int indirimYuzde { get; set; }
        public string imageUrl { get; set; }
        public int sepetAdet { get; set; }
        public int satisAdet { get; set; }
        public int isCheap { get; set; }

        public virtual Kategori kategoriID { get; set; }
        
    }
}