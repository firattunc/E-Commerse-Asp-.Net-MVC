using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using E_Commerse.Models;

namespace E_Commerse.Models.Managers
{
    public class DatabaseContext:DbContext 
    {
        public DbSet<Kategori> kategoriler { get; set; }
        public DbSet<Urun> urunler { get; set; }
        public DbSet<SatinAlmaGecmis> satinAlmaGecmis { get; set; }
        public DbSet<Sepet> sepet { get; set; }
        public DbSet<Kullanici> kullanici { get; set; }
        public DbSet<Favorites> favorites { get; set; }
        public DbSet<Admin> admin{ get; set; }
        public DbSet<Istatistik> istatistik { get; set; }
        public DatabaseContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext,E_Commerse.Migrations.Configuration >());
        }
    }
    public class VeriTabaniOlusturucu : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {           

        }
    }
}