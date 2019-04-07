using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Commerse.Models
{
    [Table("tblIstatistik")]
    public class Istatistik
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int istatistikID { get; set; }
        public DateTime ziyaretTarih { get; set; }
        
    }
}