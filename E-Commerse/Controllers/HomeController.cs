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
    public class HomeController : Controller
    {

        public String html;
        public Uri url;

        public void VeriAl(String Url,String Xpath,List<String> list)
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

                throw;
            }
        }
        public void VeriAlDip(String Url, String Xpath,String dip, List<String> list)
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

                list.Add(url+doc.DocumentNode.SelectSingleNode(Xpath).Attributes[dip].Value);

            }
            catch (NullReferenceException)
            {

                throw;
            }
        }
        // GET: Register
        public ActionResult Cart()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }

            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        [HttpPost]
        public ActionResult Cart(string eposta,string sifre,string btnLogin,string btnLogout,string btnSil,string btnEkle,string btnCikar,string btnBuy)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            var loginDurum = Convert.ToInt32(Session["loginDurum"]);
            model.kategoriler = db.kategoriler.ToList();
            if (btnBuy!=null)
            {
                
                if (loginDurum == 1)
                {
                    var guncelleSepetEkle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m).ToList();
                    foreach (var item in guncelleSepetEkle)
                    {
                        decimal indirimliFiyatSepet = Decimal.Round(item.urunID.urunFiyat - (item.urunID.urunFiyat * (Convert.ToDecimal(item.urunID.indirimYuzde) / 100)), 2);
                        decimal total = indirimliFiyatSepet * item.urunAdet;
                        SatinAlmaGecmis satinAlmaGecmis = new SatinAlmaGecmis
                        {

                            Adet=item.urunAdet,
                            kullaniciID=item.kullaniciID,
                            odenenTutar= total,
                            satinAlmaTarih=DateTime.Now,
                            urunID=item.urunID

                        };

                        db.satinAlmaGecmis.Add(satinAlmaGecmis);
                        item.urunID.satisAdet += item.urunAdet;
                        item.urunID.urunStok -= item.urunAdet;
                        if (item.urunID.urunStok < 0)
                        {
                            item.urunID.urunStok = 0;
                        }

                    }

                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Register", "Home");
                }
               
                if (loginDurum == 1)
                {

                    var kullaniciSepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    var ziyareticiSepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    foreach (var item in kullaniciSepet)
                    {
                        db.sepet.Remove(item);
                    }
                    foreach (var item in ziyareticiSepet)
                    {
                        db.sepet.Remove(item);
                    }
                    db.SaveChanges();
                    model.sepet = db.sepet.ToList();

                }
                else
                {
                   var ziyareticiSepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    model.sepet = ziyareticiSepet;

                }
                
                
                Response.Redirect(Request.RawUrl);
            }
            if (btnEkle!=null)
            {               
                int sepetID= Convert.ToInt32(btnEkle);
                var guncelleSepetEkle = (from m in db.sepet
                                     where m.sepetID == sepetID
                                     select m).FirstOrDefault();
                guncelleSepetEkle.urunID.sepetAdet++;
                guncelleSepetEkle.urunAdet++;
                db.SaveChanges();
                if (kullaniciID==1)
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                }
                else
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }
                
                Response.Redirect(Request.RawUrl);
            }
            if (btnCikar!=null)
            {
                int sepetID = Convert.ToInt32(btnCikar);
                var guncelleSepetCikar = (from m in db.sepet
                                     where m.sepetID == sepetID
                                     select m).FirstOrDefault();
                if (guncelleSepetCikar.urunAdet==1)
                {

                    if (guncelleSepetCikar!=null)
                    {
                        db.sepet.Remove(guncelleSepetCikar);
                    }
                   
                  
                }
                else
                {
                    guncelleSepetCikar.urunAdet--;
                }
                
                
                db.SaveChanges();
                if (kullaniciID == 1)
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                }
                else
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }
                Response.Redirect(Request.RawUrl);


            }
            if (btnLogin != null)
            {
                var kullanici = (from m in db.kullanici
                                 where m.email == eposta && m.kullaniciSifre == sifre
                                 select m).FirstOrDefault();

                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                try
                {

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";
                    return RedirectToAction("Cart");

                }
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }


                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();
                        
                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            
            return View(model);
        }
        public ActionResult Register()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            if (TempData["errorRegister"]!=null)
            {

                ViewData["errorRegister"] = TempData["errorRegister"];

            }
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        // POST: Register
        [HttpPost]
        public ActionResult Register(string btnSil,string eposta, string sifre, string btnLogin, string firstname, string lastname, string email, string password, string address, string country, string btnRegister, int? isRegisterOk)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);

            if (btnRegister != null)
            {
                Kullanici isSame = null;
                isSame = db.kullanici.Where(x => x.email == email).FirstOrDefault();
                if (isSame == null)
                {
                    Kullanici kullanici = new Kullanici
                    {
                        kullaniciAd = firstname,
                        kullaniciSoyAd = lastname,
                        email = email,
                        kullaniciSifre = password,
                        Adres = address,
                        Country = country,
                        memberSince = DateTime.Now,
                    };
                    db.kullanici.Add(kullanici);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorRegister"] = 1;
                    return RedirectToAction("Register");
                }
            }
            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                model.urunler = db.urunler.ToList();
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }
            if (btnLogin != null)
            {
                try
                {
                    var kullanici = (from m in db.kullanici
                                     where m.email == eposta && m.kullaniciSifre == sifre
                                     select m).FirstOrDefault();

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    model.urunler = db.urunler.ToList();          //select * from urunler
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    var ziyaretciSepetList = (from m in db.sepet
                                              where m.kullaniciID == null
                                              select m).ToList();
                    foreach (var item in ziyaretciSepetList)
                    {
                        int adet = (from m in db.sepet
                                    where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                    select m.urunAdet).DefaultIfEmpty(0).Sum();
                        if (adet == 0)
                        {
                            item.kullaniciID = (from m in db.kullanici
                                                where m.kullaniciID == kullaniciID
                                                select m).FirstOrDefault();
                            db.sepet.Add(item);
                            db.SaveChanges();

                        }
                        else
                        {
                            var sepetGüncelle = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                                 select m).FirstOrDefault();

                            sepetGüncelle.urunAdet += item.urunAdet;
                            db.SaveChanges();
                        }

                        model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                        //last

                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    //KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";

                }

            }
            model.kategoriler = db.kategoriler.ToList();  
            return View(model);
        }
        // GET: Home
        public ActionResult Index()
        {
            
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (Convert.ToInt32(Session["ziyaret"])==0)
            {
                Istatistik ıstatistik = new Istatistik
                {
                    ziyaretTarih = DateTime.Now,
                };
                db.istatistik.Add(ıstatistik);
                db.SaveChanges();
                Session["ziyaret"] = 1;
            }

           
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            var enUcuzUrunler = db.urunler.Where(x => x.isCheap == 1).ToList();


            model.admin = db.admin.FirstOrDefault();
            if (Session["duyuruGoster"]==null)
            {
                Session["duyuruGoster"] = model.admin.duyuruGoster;
            }
            else
            {
                Session["duyuruGoster"] = 0;
            }

            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            model.enUcuzUrunler = enUcuzUrunler;
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string eposta,string sifre, string btnLogin, string btnLogout,string btnSil)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            var enUcuzUrunler = db.urunler.Where(x => x.isCheap == 1).ToList();
            model.enUcuzUrunler = enUcuzUrunler;
            model.admin = db.admin.FirstOrDefault();
            
            if (btnLogin != null)
            {
                var kullanici = (from m in db.kullanici
                                 where m.email == eposta && m.kullaniciSifre == sifre
                                 select m).FirstOrDefault();
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);

                    try
                    {

                        Session["kullaniciID"] = kullanici.kullaniciID;
                        Session["kullaniciAd"] = kullanici.kullaniciAd;
                        Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                        Session["kullaniciemail"] = kullanici.email;
                        Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                        Session["kullaniciAdres"] = kullanici.Adres;
                        Session["kullaniciUlke"] = kullanici.Country;
                        Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                        Session["loginDurum"] = 1;
                        Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                    }
                    catch (Exception)
                    {
                        //TODO BU KULLANICI BULUNAMADI.
                        model.urunler = db.urunler.ToList();
                        ViewData["alertLogin"] = "1";
                        return RedirectToAction("Index");

                     }


                    var ziyaretciSepetList = (from m in db.sepet
                                              where m.kullaniciID == null
                                              select m).ToList();
                    foreach (var item in ziyaretciSepetList)
                    {
                        int adet = (from m in db.sepet
                                    where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                    select m.urunAdet).DefaultIfEmpty(0).Sum();
                        if (adet == 0)
                        {
                            item.kullaniciID = (from m in db.kullanici
                                                where m.kullaniciID == kullaniciID
                                                select m).FirstOrDefault();
                            db.sepet.Add(item);
                            db.SaveChanges();

                        }
                        else
                        {
                            var sepetGüncelle = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                                 select m).FirstOrDefault();

                            sepetGüncelle.urunAdet += item.urunAdet;
                            db.SaveChanges();
                        }

                        model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                        //last

                    }

                    model.urunler = db.urunler.ToList();          //select * from urunler
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);

            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {
                try
                {
                    model.kategoriler = db.kategoriler.ToList();
                    model.urunler = db.urunler.ToList();
                    if (Convert.ToInt32(Session["loginDurum"]) == 1)
                    {
                        int btnSilint = Convert.ToInt32(btnSil);
                        int adet = (from m in db.sepet
                                    where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                    select m.urunAdet).DefaultIfEmpty(0).Sum();
                        if (adet > 1)
                        {
                            var sepetGüncelle = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                                 select m).FirstOrDefault();

                            sepetGüncelle.urunAdet--;
                            db.SaveChanges();
                        }
                        else
                        {
                            Sepet sepetvol1 = (from m in db.sepet
                                               where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                               select m).FirstOrDefault();
                            db.sepet.Remove(sepetvol1);
                            db.SaveChanges();
                        }

                        model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                        Response.Redirect(Request.RawUrl);
                    }
                    //ZİYATETÇİ
                    else
                    {
                        int btnSilint = Convert.ToInt32(btnSil);
                        int adet = (from m in db.sepet
                                    where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                    select m.urunAdet).DefaultIfEmpty(0).Sum();
                        if (adet > 1)
                        {
                            var sepetGüncelle = (from m in db.sepet
                                                 where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                                 select m).FirstOrDefault();

                            sepetGüncelle.urunAdet--;
                            db.SaveChanges();
                        }
                        else
                        {
                            Sepet sepetvol1 = (from m in db.sepet
                                               where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                               select m).FirstOrDefault();
                            db.sepet.Remove(sepetvol1);
                            db.SaveChanges();
                        }

                        model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                        Response.Redirect(Request.RawUrl);
                    }
                }
                catch (Exception)
                {

                    return RedirectToAction("Index");
                }
                


            }
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            model.kategoriler = db.kategoriler.ToList();
            model.urunler = db.urunler.ToList();
            return View(model);
        }
        //GET: Contact
        public ActionResult Contact()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        [HttpPost]
        public ActionResult Contact(string eposta, string sifre, string btnLogin, string btnLogout, string btnSil, string name, string email, string subject, string message, string btnSendMessage)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.kategoriler = db.kategoriler.ToList();
            if (btnSendMessage!=null)
            {
                if (Convert.ToInt32(Session["loginDurum"])==1)
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                }
                else
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }
                
                MailMessage mail = new MailMessage(); //yeni bir mail nesnesi Oluşturuldu.
                mail.IsBodyHtml = true; //mail içeriğinde html etiketleri kullanılsın mı?
                mail.To.Add("firat_yzm17@outlook.com"); //Kime mail gönderilecek.

                //mail kimden geliyor, hangi ifade görünsün?
                mail.From = new MailAddress("firatyzm18@gmail.com", subject, System.Text.Encoding.UTF8);
                mail.Subject = subject;//mailin konusu

                //mailin içeriği.. Bu alan isteğe göre genişletilip daraltılabilir.
                if (Convert.ToInt32(Session["loginDurum"])==1)
                {
                    mail.Body = "Kim:" + name + "</br>Kullanıcı ID:" + Session["kullaniciID"].ToString() + "</br>" + "E-Posta:" + email + "</br>" + "Konu:" + subject + "</br>" + "Içerik:" + message;
                }
                else
                {
                    mail.Body = "Kim:" + name + "</br>" + "E-Posta:" + email + "</br>" + "Konu:" + subject + "</br>" + "Içerik:" + message;
                }
                
                mail.IsBodyHtml = true;
                SmtpClient smp = new SmtpClient();

                //mailin gönderileceği adres ve şifresi
                smp.Credentials = new NetworkCredential("firatyzm18@gmail.com", "f29376479G");
                smp.Port = 587;
                smp.Host = "smtp.gmail.com";//gmail üzerinden gönderiliyor.
                smp.EnableSsl = true;
                smp.Send(mail);//mail isimli mail gönderiliyor.
                return RedirectToAction("Contact");
            }
            

            
            if (btnLogin != null)
            {

               
                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                try
                {

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    model.kategoriler = db.kategoriler.ToList();
                    ViewData["alertLogin"] = "1";
                    return RedirectToAction("Contact");

                }
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();

            return View(model);
        }

        //GET: About
        public ActionResult About()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        //POST:About
        [HttpPost]
        public ActionResult About(string eposta, string sifre, string btnLogin, string btnLogout, string btnSil)
        {

            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.kategoriler = db.kategoriler.ToList();
            if (btnLogin != null)
            {

                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                try
                {

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    model.kategoriler = db.kategoriler.ToList();
                    ViewData["alertLogin"] = "1";
                    return RedirectToAction("About");

                }
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }
           
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            return View (model);

        }
        //GET: Help
        public ActionResult Help()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        //POST:Help
        [HttpPost]
        public ActionResult Help(string eposta, string sifre, string btnLogin, string btnLogout, string btnSil)
        {


            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.kategoriler = db.kategoriler.ToList();
            if (btnLogin != null)
            {

                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                try
                {

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    model.kategoriler = db.kategoriler.ToList();
                    ViewData["alertLogin"] = "1";
                    return RedirectToAction("About");

                }
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {

                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }


            }

            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            return View(model);
        }
        //GET: About
        public ActionResult ErrorPage()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        //POST:About
        [HttpPost]
        public ActionResult ErrorPage(string eposta, string sifre, string btnLogin, string btnLogout, string btnSil)
        {

            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.kategoriler = db.kategoriler.ToList();
            if (btnLogin != null)
            {

                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                try
                {

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    model.kategoriler = db.kategoriler.ToList();
                    ViewData["alertLogin"] = "1";
                    return RedirectToAction("About");

                }
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }

            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }

            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            return View(model);

        }

    }
}