using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Commerse.Models;
using E_Commerse.ViewModels.Home;
using E_Commerse.Models.Managers;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.IO;
using System.Text;
using HtmlAgilityPack;
namespace E_Commerse.Controllers
{
    public class AdminController : Controller
    {
        public String html;
        public Uri url;

        public void VeriAl(String Url, String Xpath, List<String> list)
        {
            try
            {
                url = new Uri(Url);
            }
            catch (UriFormatException)
            {

                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            try
            {
                html = client.DownloadString(url);
            }
            catch (WebException)
            {

                throw;
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            try
            {
                list.Add(doc.DocumentNode.SelectSingleNode(Xpath).InnerText);

            }
            catch (NullReferenceException)
            {

                ViewBag.result = "İstek sonucu null döndü.";
                ViewBag.status = "danger";
            }
        }
        public void VeriAlDip(String Url, String Xpath, String dip, List<String> list)
        {
            try
            {
                url = new Uri(Url);
            }
            catch (UriFormatException)
            {

                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            try
            {
                html = client.DownloadString(url);
            }
            catch (WebException)
            {

                throw;
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            try
            {
                string url = "https://www.cimri.com";

                list.Add(url + doc.DocumentNode.SelectSingleNode(Xpath).Attributes[dip].Value);

            }
            catch (NullReferenceException)
            {

                throw;
            }
        }
        // GET: Admin
        public ActionResult AdminKategori()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            model.kategoriler = db.kategoriler.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminKategori(string btnEkle,string btnSil,string btnDüzenle,string btnLogout)
        {
            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            model.kategoriler = db.kategoriler.ToList();
            if (btnEkle!=null)
            {
                return RedirectToAction("AdminKategoriEkle");
            }
            if (btnSil != null)
            {
                int kategoriID = Convert.ToInt32(btnSil);
                var kategori = (from m in db.kategoriler
                                where m.kategoriID == kategoriID
                                select m).FirstOrDefault();
                var urunler = (from m in db.urunler
                               where m.kategoriID.kategoriID == kategoriID
                               select m).FirstOrDefault();
                if (urunler!=null)
                {
                    ViewData["sonuc"] = "Başarısız";
                    return RedirectToAction("AdminKategori");
                }
              
                
                db.kategoriler.Remove(kategori);
                int sonuc=db.SaveChanges();
                if (sonuc>1)
                {
                    ViewData["sonuc"] = "Başarılı";
                }
                else
                {
                    ViewData["sonuc"] = "Başarısız";
                }
                model.kategoriler = db.kategoriler.ToList();
                return RedirectToAction("AdminKategori");
            }
            if (btnDüzenle != null)
            {
                int kategoriID = Convert.ToInt32(btnDüzenle);
                var kategori = (from m in db.kategoriler
                                where m.kategoriID == kategoriID
                                select m).FirstOrDefault();
                var fullUrl = this.Url.Action("AdminKategoriEkle", "Admin", new { kategoriID = kategoriID });

                return Redirect(fullUrl);
            }
           
            return View(model);
        }
        // GET: Admin
        public ActionResult AdminPromosyon()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            List<SelectListItem> kategoriList =
           (from kategori in db.kategoriler.ToList()
            select new SelectListItem()
            {

                Text = kategori.kategoriAd,
                Value = kategori.kategoriID.ToString(),


            }).ToList();
            ViewBag.kategoriler = kategoriList;

            TempData["kategoriler"] = kategoriList;


            List<SelectListItem> urunList =
            (from urun in db.urunler.ToList()
             select new SelectListItem()
             {

                 Text = urun.urunAd,
                 Value = urun.urunID.ToString(),


             }).ToList();
            ViewBag.urunler = urunList;

            TempData["urunler"] = urunList;
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminPromosyon(string btnLogout,HomePageModel modelPromosyon,string btnKategoriBaz,string btnUrunBaz,string btnAll,string txttopluPromosyonMiktar, string txtKategoriPromosyonMiktar,string txtUrunPromosyonMiktar)
        {
            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            ViewBag.urunler = TempData["urunler"];
            ViewBag.kategoriler = TempData["kategoriler"];
            model.admin = db.admin.FirstOrDefault();
            if (btnKategoriBaz!=null)
            {
                var urun = db.urunler.Where(x => x.kategoriID.kategoriID == modelPromosyon.kategori.kategoriID).ToList();
                foreach (var item in urun)
                {
                    item.indirimYuzde = Convert.ToInt32(txtKategoriPromosyonMiktar);
                }
                db.SaveChanges();
                return RedirectToAction("AdminPromosyon");
            }
            if (btnUrunBaz != null)
            {
                var urun = db.urunler.Where(x => x.urunID == modelPromosyon.urunDropdown.urunID).FirstOrDefault();
                urun.indirimYuzde= Convert.ToInt32(txtUrunPromosyonMiktar);
                db.SaveChanges();
                return RedirectToAction("AdminPromosyon");
            }
            if (btnAll != null)
            {
                int miktar = Convert.ToInt32(txttopluPromosyonMiktar);
                foreach (var item in db.urunler)
                {
                    item.indirimYuzde = miktar;
                }
                db.SaveChanges();
                return RedirectToAction("AdminPromosyon");
            }
            
            return View(model);
        }
        // GET: Admin
        public ActionResult AdminUrun()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            model.urunler = db.urunler.ToList();
            model.kategoriler = db.kategoriler.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminUrun(string btnLogout,string btnEkle, string btnSil, string btnDüzenle)
        {
            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            model.kategoriler = db.kategoriler.ToList();
            model.urunler = db.urunler.ToList();
            if (btnEkle != null)
            {
                return RedirectToAction("AdminUrunEkle");
            }
            if (btnSil != null)
            {
                int urunID = Convert.ToInt32(btnSil);
                var urun= (from m in db.urunler
                                where m.urunID == urunID
                              select m).FirstOrDefault();
                db.urunler.Remove(urun);
                int sonuc = db.SaveChanges();
                if (sonuc > 1)
                {
                    ViewData["sonuc"] = "Başarılı";
                }
                else
                {
                    ViewData["sonuc"] = "Başarısız";
                }
                model.urunler = db.urunler.ToList();
                Response.Redirect(Request.RawUrl);
            }
            if (btnDüzenle != null)
            {
                int urunID = Convert.ToInt32(btnDüzenle);
                var urun = (from m in db.urunler
                                where m.urunID == urunID
                                select m).FirstOrDefault();
                var fullUrl = this.Url.Action("AdminUrunEkle", "Admin", new { urunID = urunID });

                return Redirect(fullUrl);
            }

            return View(model);
        }
        // GET: Admin
        public ActionResult AdminKategoriEkle()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminKategoriEkle(string btnLogout,string txtKategoriAd,int? kategoriID)
        {
            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            if (txtKategoriAd!=null&&kategoriID==null)
            {
                if (txtKategoriAd=="")
                {
                    ViewBag.result = "Başarısız";
                    ViewBag.status = "danger";
                }
                else
                {
                    Kategori kategori = new Kategori
                    {
                        kategoriAd = txtKategoriAd,
                    };
                    db.kategoriler.Add(kategori);
                    int sonuc = db.SaveChanges();
                    if (sonuc > 0)
                    {
                        ViewBag.result = "Başarılı";
                        ViewBag.status = "sucsess";
                    }
                    else
                    {

                        ViewBag.result = "Başarısız";
                        ViewBag.status = "danger";
                    }
                }
                return RedirectToAction("AdminKategoriEkle");
               
            }
            if (kategoriID!=null)
            {
                if (txtKategoriAd == "")
                {
                    ViewBag.result = "Başarısız";
                    ViewBag.status = "danger";
                }
                else
                {
                    var kategori = (from m in db.kategoriler
                                    where m.kategoriID == kategoriID
                                    select m).FirstOrDefault();
                    kategori.kategoriAd = txtKategoriAd;
                    int sonuc = db.SaveChanges();
                    if (sonuc > 0)
                    {
                        ViewBag.result = "Başarılı";
                        ViewBag.status = "sucsess";
                    }
                    else
                    {

                        ViewBag.result = "Başarısız";
                        ViewBag.status = "danger";
                    }
                }
                return RedirectToAction("AdminKategoriEkle");
            }
            
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        // GET: Admin
        public ActionResult AdminUrunEkle()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            model.urunler = db.urunler.ToList();
            model.admin = db.admin.FirstOrDefault();
            List<SelectListItem> kategorilerList =
            (from kategori in db.kategoriler.ToList()
             select new SelectListItem()
             {
                
                 Text = kategori.kategoriAd,
                 Value = kategori.kategoriID.ToString(),
                 
                 
             }).ToList();
            ViewBag.kategoriler = kategorilerList;
            
            TempData["kategoriler"] = kategorilerList;
            if (TempData["result"]!=null)
            {
                ViewBag.result = TempData["result"];
                ViewBag.status = TempData["status"];
            }
            

            return View(model);
        }
        [HttpPost]
        public ActionResult AdminUrunEkle(string btnLogout,string txtUrunAd,string txtUrunFiyat,string txtStok,string txtAciklama,string txtIndirimYuzde,string txtImageUrl,int? urunID,HomePageModel modelEklenen)
        {
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            ViewBag.kategoriler = TempData["kategoriler"];
            var kategori = db.kategoriler.Where(x => x.kategoriID == modelEklenen.kategori.kategoriID).FirstOrDefault();
            var urunVarmı = db.urunler.Where(x => x.urunAciklama == txtAciklama).FirstOrDefault();
            var kategoriUcuz = db.kategoriler.Where(x => x.kategoriAd == "Cheapest").FirstOrDefault();
            decimal enDusukFiyat = 0;
            model.admin = db.admin.FirstOrDefault();
            

            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            
           
            if (urunID == null)
            {
                if (urunVarmı != null)
                {
                    try
                    {
                        //veri çekme
                        List<String> listeVeri = new List<String>();
                        List<String> listeVeriDip = new List<String>();
                        List<String> listeVeriFiyat = new List<String>();
                        List<decimal> listeVeriFiyatInt = new List<decimal>();
                        string url = "https://www.cimri.com";
                        string ara = "/arama?q=";
                        string aranacakUrun = txtAciklama;
                        string urunAraUrl = url + ara + aranacakUrun;

                        VeriAl(urunAraUrl, "//*[@data-productorder='1']/article/a", listeVeri);

                        VeriAlDip(urunAraUrl, "//*[@data-productorder='1']/article/a", "href", listeVeriDip);


                        for (int j = 0; j < 3; j++)
                        {
                            VeriAl(listeVeriDip[0], "//*[@id='main_container']/div/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/div/table/tbody/tr[" + (j + 1) + "]/td[4]/a/div[2]/div", listeVeriFiyat);
                        }
                        foreach (var item in listeVeriFiyat)
                        {
                            decimal i = 0;
                            string[] Liste = item.Split(' ');
                            listeVeriFiyatInt.Add(Convert.ToDecimal(Liste[0]));

                        }
                        enDusukFiyat = listeVeriFiyatInt.Min();
                        //veri çekme end
                    }
                    catch (Exception)
                    {

                        ViewBag.result = "Başarısız";
                        ViewBag.status = "danger";
                    }
                    

                   
                }
                try
                {
                    Urun urun = new Urun();
                    urun.urunAd = txtUrunAd;
                    urun.urunStok = Convert.ToInt32(txtStok);
                    urun.urunAciklama = txtAciklama;
                    urun.indirimYuzde = Convert.ToInt32(txtIndirimYuzde);
                    urun.imageUrl = txtImageUrl;
                    urun.kategoriID = kategori;
                    urun.sepetAdet = 0;
                    urun.satisAdet = 0;
                    if (enDusukFiyat == 0)
                    {
                        urun.isCheap = 0;
                        urun.urunFiyat = Convert.ToInt32(txtUrunFiyat);

                    }
                    else
                    {
                        urun.urunFiyat = enDusukFiyat;
                        urun.kategoriID = kategoriUcuz;
                        urun.isCheap = 1;
                    }

                    db.urunler.Add(urun);
                    int sonuc = db.SaveChanges();
                    if (sonuc > 0)
                    {
                        if (urunVarmı != null)
                        {
                            TempData["result"] = "Aynı isimde bir ürün olduğu için urun fiyatı cimri.com sitesinden alınmıştır";
                            TempData["status"] = "succsess";
                        }
                        else
                        {
                            TempData["result"] = "Başarılı";
                            TempData["status"] = "succsess";
                        }

                            
                    }
                    else
                    {

                        TempData["result"] = "Başarısız";
                        TempData["status"] = "danger";
                    }
                }
                catch (Exception)
                {

                    TempData["result"] = "Başarısız";
                    TempData["status"] = "danger";
                }

                

                return RedirectToAction("AdminUrunEkle");


            }
            else
            {
                try
                {
                    var urun = (from m in db.urunler
                                where m.urunID == urunID
                                select m).FirstOrDefault();
                        urun.urunAd = txtUrunAd;
                        urun.urunFiyat = Convert.ToDecimal(txtUrunFiyat);
                        urun.urunStok = Convert.ToInt32(txtStok);
                        urun.urunAciklama = txtAciklama;
                        urun.indirimYuzde = Convert.ToInt32(txtIndirimYuzde);
                        urun.imageUrl = txtImageUrl;
                        urun.kategoriID = kategori;
                        urun.isCheap = 0;

                        int sonuc = db.SaveChanges();
                    if (sonuc > 0)
                    {
                        TempData["result"] = "Başarılı";
                        TempData["status"] = "success";
                    }
                    else
                    {

                        TempData["result"] = "Başarısız";
                        TempData["status"] = "danger";
                       

                    }   

                    
                }
                catch (Exception)
                {
                    TempData["result"] = "Başarısız";
                    TempData["status"] = "danger";
                    return RedirectToAction("AdminUrunEkle");
                }
               
            }

            return RedirectToAction("AdminUrunEkle");
        }
        // GET: Admin
        public ActionResult AdminIndex()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            int ziyaretSayisiHaftalik = 0;
            int urunSatisToplam = 0;
            int urunSatisHaftalik = 0;
            int uyeKayitSayisiHaftalik = 0;
            int haftaBaslangic = 0;
            int haftaBitis = 0;
            //Toplam kayıtlar
            var ziyaretciSayisi = db.istatistik.Count();
            var urunSatinAlmaSayisi = db.satinAlmaGecmis;

            urunSatisToplam += (from m in db.satinAlmaGecmis
                                select m.Adet).DefaultIfEmpty(0).Sum();
            var uyeKayitSayisiToplam = db.kullanici.Count();

            //Tarih İşlemleri
            var tarihGun = DateTime.Now.Day;
            var tarihHafta = DateTime.Now.DayOfWeek;
            System.Collections.ArrayList liste = new System.Collections.ArrayList();
            for (int i = 0; i < 7; i++)
            {
                if (tarihHafta.ToString() == "Monday")
                {
                    liste.Add("Monday");
                    haftaBaslangic = DateTime.Now.Day;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Tuesday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    haftaBaslangic = DateTime.Now.Day - 1;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Wednesday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    liste.Add("Wednesday");
                    haftaBaslangic = DateTime.Now.Day - 2;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Thursday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    liste.Add("Wednesday");
                    liste.Add("Thursday");
                    haftaBaslangic = DateTime.Now.Day - 3;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Friday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    liste.Add("Wednesday");
                    liste.Add("Thursday");
                    liste.Add("Friday");
                    haftaBaslangic = DateTime.Now.Day - 4;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Saturday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    liste.Add("Wednesday");
                    liste.Add("Thursday");
                    liste.Add("Friday");
                    liste.Add("Saturday");
                    haftaBaslangic = DateTime.Now.Day - 5;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
                else if (tarihHafta.ToString() == "Sunday")
                {
                    liste.Add("Monday");
                    liste.Add("Tuesday");
                    liste.Add("Wednesday");
                    liste.Add("Thursday");
                    liste.Add("Friday");
                    liste.Add("Saturday");
                    liste.Add("Sunday");
                    haftaBaslangic = DateTime.Now.Day - 6;
                    haftaBitis = haftaBaslangic + 7;
                    break;
                }
            }

            //Günlük Kayıtlar
            var ziyaretSayisiGunluk = db.istatistik.Where(x => x.ziyaretTarih.Day == DateTime.Now.Day).Count();
            var urunSayisiGunluk = db.satinAlmaGecmis.Where(x => x.satinAlmaTarih.Day == DateTime.Now.Day).Count();
            var uyeKayitSayisiGünlük = db.kullanici.Where(x => x.memberSince.Day == DateTime.Now.Day).Count();

            //Haftalık kayıtlar
            foreach (var item in liste)
            {

                var query1 = (from m in db.istatistik
                              select m).AsEnumerable();
                ziyaretSayisiHaftalik += (from m in query1
                                          where (m.ziyaretTarih.DayOfWeek.ToString() == item.ToString()) && (m.ziyaretTarih.Month == DateTime.Now.Month) && (m.ziyaretTarih.Day <= haftaBitis - 1 && m.ziyaretTarih.Day >= haftaBaslangic)
                                          select m).Count();
                var query2 = (from m in db.satinAlmaGecmis
                              select m).AsEnumerable();
                urunSatisHaftalik += (from m in query2
                                      where (m.satinAlmaTarih.DayOfWeek.ToString() == item.ToString()) && (m.satinAlmaTarih.Month == DateTime.Now.Month) && (m.satinAlmaTarih.Day <= haftaBitis - 1 && m.satinAlmaTarih.Day >= haftaBaslangic)
                                      select m).Count();
                var query3 = (from m in db.kullanici
                              select m).AsEnumerable();
                uyeKayitSayisiHaftalik += (from m in query3
                                           where (m.memberSince.DayOfWeek.ToString() == item.ToString()) && (m.memberSince.Month == DateTime.Now.Month) && (m.memberSince.Day <= haftaBitis - 1 && m.memberSince.Day >= haftaBaslangic)
                                           select m).Count();

            }

            //En çok 5 liler
            var urunler = db.urunler.GroupBy(x => x.satisAdet).ToList();
            var sepetUrunler = db.urunler.GroupBy(x => x.sepetAdet).ToList();
            foreach (var item in sepetUrunler)
            {
                model.enCokSepetUrun.Add(item.FirstOrDefault().urunAd);
            }
            foreach (var item in urunler)
            {

                    model.enCokSatisUrun.Add(item.FirstOrDefault().urunAd);
                             
                 
            }
            model.enCokSepetUrun.Reverse();
            if (model.enCokSepetUrun.Count<5)
            {
                model.enCokSepetUrun.RemoveAt(model.enCokSepetUrun.Count - 1);
            }

            model.enCokSatisUrun.Reverse();
            if (model.enCokSatisUrun.Count < 5)
            {
                model.enCokSatisUrun.RemoveAt(model.enCokSatisUrun.Count - 1);
            }
            //en çok satış yapan kategori   
            List<int> list = new List<int>();
            
           
                foreach (var item in db.kategoriler)
                {
                    var kategori = (from m in db.urunler
                                    where m.kategoriID.kategoriID==item.kategoriID
                                    select m.satisAdet).DefaultIfEmpty(0).Sum();
                    list.Add(kategori);
                    
                }
            
            int kategorimax=list.Max();
            int a= list.IndexOf(kategorimax);
            foreach (var item in db.kategoriler)
            {
                var urun=(from m in db.urunler
                                     where m.kategoriID.kategoriID == item.kategoriID
                                     select m.satisAdet).DefaultIfEmpty(0).Sum();

                if ((a+1)==item.kategoriID)
                {
                    if (urun>0)
                    {
                        model.enCokSatisKategori = item.kategoriAd;
                    }
                   
                }
                
            }
            //modeli sayfaya yollama kısmı
            model.ziyaretSayisiHaftalik = ziyaretSayisiHaftalik;
            model.ziyaretSayisiGünlük = ziyaretSayisiGunluk;
            model.ziyaretSayisiToplam = ziyaretciSayisi;
            model.urunSayisiToplam = urunSatisToplam;
            model.urunSayisiGünlük = urunSayisiGunluk;
            model.urunSayisiHaftalik = urunSatisHaftalik;
            model.uyeKayitSayisiGünlük = uyeKayitSayisiGünlük;
            model.uyeKayitSayisiHaftalik = uyeKayitSayisiHaftalik;
            model.uyeKayitSayisiToplam = uyeKayitSayisiToplam;


            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminIndex(string btnLogout)
        {
            if (btnLogout!=null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        // GET: Admin
        public ActionResult AdminLogin()
        {
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminLogin(string email,string sifre)
        {
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            var login = (from m in db.admin
                         where m.adminEmail == email && m.adminSifre == sifre
                         select m).FirstOrDefault();
            
            
            if (login!=null)
            {
                Session["adminID"] = login.adminID;
                model.admin = login;
            }
            else
            {
                ViewBag.result = "ID ya da şifre yanlış.";
                ViewBag.status = "danger";
                return View();
            }
            return RedirectToAction("AdminIndex");
        }
        // GET: Admin
        public ActionResult AdminNotification()
        {
            if (Session["adminID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            model.admin = db.admin.FirstOrDefault();
            return View(model);
        }
        // GET: Admin
        [HttpPost]
        public ActionResult AdminNotification(string btnLogout,string txtDuyuru,string btnDegistir,string btnDuyur)
        {
            if (btnLogout != null)
            {
                Session["adminID"] = null;
                return RedirectToAction("AdminLogin");
            }
            HomePageModel model = new HomePageModel();
            DatabaseContext db = new DatabaseContext();
            var adminID = Convert.ToInt32(Session["adminID"]);
            var admin = (from m in db.admin
                         where m.adminID == adminID
                         select m).FirstOrDefault();
            
            if (btnDuyur!=null)
            {
                admin.duyuru = txtDuyuru;
                db.SaveChanges();
                model.admin = db.admin.Where(x => x.adminID == adminID).FirstOrDefault();
                return RedirectToAction("AdminNotification");
            }
            if (btnDegistir!=null)
            {
                if (admin.duyuruGoster ==1)
                {
                    admin.duyuruGoster = 0;
                }
                else
                {
                    admin.duyuruGoster = 1;
                }
                db.SaveChanges();
                model.admin = db.admin.Where(x => x.adminID == adminID).FirstOrDefault();
                return RedirectToAction("AdminNotification");
            }
                                 
            
            return View(model);
        }
    }
}