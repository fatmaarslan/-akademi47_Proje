using Microsoft.AspNetCore.Mvc;
using iakademi47_proje.Models;
using İakademi47_Proje.Models;
using PagedList.Core; //nuget pakketten aldım paket paket getirmek için
using XAct;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace İakademi47_Proje.Controllers
{
    public class HomeController : Controller
    {

		Cls_Product p = new Cls_Product();
		MainPageModel mpm = new MainPageModel();
        iakademi47Context context = new iakademi47Context();
		Cls_Order cls_order = new Cls_Order();


        int maintpageCount;

        public HomeController()
        {
            this.maintpageCount = context.Settings.FirstOrDefault(s => s.SettingID == 1).MainPageCount;
        }
        public IActionResult Index()
		{
			mpm.SliderProducts = p.ProductSelect("Slider","",0);
			mpm.NewProducts = p.ProductSelect("New","",0);//new ana sayfa için parametre, "",  alt sayfa için parametre , 0=AJAX için parametre
            mpm.Productofday = p.ProductDetails();
			mpm.SpecialProducts = p.ProductSelect("Special","",0);//özel
			mpm.DiscountedProducts = p.ProductSelect("Discounted","", 0);//indirimli
			mpm.HighLightedProducts = p.ProductSelect("HighLighted","", 0);//öne çıkan
			mpm.TopsellerProducts = p.ProductSelect("TopSeller","", 0);//çok satan
			mpm.StarProducts = p.ProductSelect("Star","", 0);//yıldızlı
			mpm.FeaturedProducts = p.ProductSelect("Featured","", 0);//fırsat
			mpm.NotableProducts = p.ProductSelect("Notable","", 0);//dikkat çeken



			
			return View(mpm);
		}
		

		

		public IActionResult NewProducts() 
		{

			mpm.NewProducts = p.ProductSelect("New", "New",0);
			return View(mpm);
		}

		public PartialViewResult _partialNewProducts( string nextpagenumber) 
		{
			int pagenumber=Convert.ToInt32(nextpagenumber);
            mpm.NewProducts = p.ProductSelect("New", "New", pagenumber);
            return PartialView(mpm);
        }



        public IActionResult SpecialProducts()
        {

            mpm.SpecialProducts = p.ProductSelect("Special", "Special", 0);
            return View(mpm);
        }

        public PartialViewResult _partialSpecialProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.SpecialProducts = p.ProductSelect("Special", "Special", pagenumber);
            return PartialView(mpm);
        }






        public IActionResult DiscountedProducts()
        {

            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", 0);
            return View(mpm);
        }

        public PartialViewResult _partialDiscountedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", pagenumber);
            return PartialView(mpm);
        }



        public IActionResult HighlightedProducts()
        {

            mpm.HighLightedProducts = p.ProductSelect("HighLighted", "HighLighted", 0);
            return View(mpm);
        }

        public PartialViewResult _partialHighlightedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.HighLightedProducts = p.ProductSelect("HighLighted", "HighLighted", pagenumber);
            return PartialView(mpm);
        }

        public IActionResult TopsellerProducts(int page=1/*null olursa 1 olsun*/,int pageSize = 4) 
        {
            PagedList<Product> model = new PagedList<Product>(context.Products.OrderByDescending(p => p.TopSeller), page, pageSize);
            return View("TopsellerProducts",model);
        }


        public IActionResult Details(int id)
        {
            //efcore
            //mpm.ProductDetails = context.Products.FirstOrDefault(p => p.ProductID == id);

            //select * from Products where ProductID = id  ado.net , dapper

            //linq  - 4 nolu ürünün bütün kolon (sütün) bilgileri elimde
            mpm.ProductDetails = (from p in context.Products where p.ProductID == id select p).FirstOrDefault();

            //linq
            mpm.CategoryName = (from p in context.Products
                                join c in context.Categories
                              on p.CategoryID equals c.CategoryID
                                where p.ProductID == id
                                select c.CategoryName).FirstOrDefault();

            //linq
            mpm.BrandName = (from p in context.Products
                             join s in context.Suppliers
                           on p.SupplierID equals s.SupplierID
                             where p.ProductID == id
                             select s.BrandName).FirstOrDefault();

            //select * from Products where Related = 2 and ProductID != 4
            mpm.RelatedProducts = context.Products.Where(p => p.Related == mpm.ProductDetails!.Related && p.ProductID != id).ToList();

            Cls_Product.Highlighted_Incrase(id);

            return View(mpm);
        }



        public IActionResult CartProcess(int id)
        {
            Cls_Product.Highlighted_Incrase(id);

            cls_order.ProductID = id;
            cls_order.Quantity = 1;

            var cookieOptions = new CookieOptions();
            //tarayıcıdan okuma
            var cookie = Request.Cookies["sepetim"];
            if (cookie == null)
            {
                //sepet boşsa
                cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7);// 7 günlük çerez süresi
                cookieOptions.Path = "/";
                cls_order.MyCart = "";
                cls_order.AddToMyCart(id.ToString());
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                TempData["Message"] = "Ürün Sepetinize Eklendi";


            }

            else
            {
                //sepet doluysa
                cls_order.MyCart = cookie;

                if (cls_order.AddToMyCart(id.ToString()) == false)
                {
                    //sepet dolu ,aynı ürün değil
                    Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                    TempData["Message"] = "Ürün Sepetinize Eklendi";
                    //o an hangi sayfadaysam sayfanın linkini yakalıyorum

                }
                else
                {
                    HttpContext.Session.SetString("Message", "Ürün Sepetinizde Zaten Var");
                    TempData["Message"] = "Ürün Sepetinizde Zaten Var";


                }
            }

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }



        public IActionResult Cart()
        {
            List<Cls_Order> MyCart;

            if (HttpContext.Request.Query["scid"].ToString() != "")
            {
                int scid = Convert.ToInt32(HttpContext.Request.Query["scid"].ToString());
                cls_order.MyCart = Request.Cookies["sepetim"];
                cls_order.DeleteFromMyCart(scid.ToString());

                var cookieOptions = new CookieOptions();
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                TempData["Message"] = "Ürün Sepetinizden Silindi";
                MyCart = cls_order.SelectMyCart();
                ViewBag.MyCart = MyCart;
                ViewBag.MyCart_Table_Details = MyCart;
            }
            else
            {
                var cookie = Request.Cookies["sepetim"];


                if (cookie == null)
                {
                    //SEPETTE HİÇ ÜRÜN OLMAYABİLİR
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = "";
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;

                }
                else
                {
                    //SEPETTE ÜRÜN VAR
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = Request.Cookies["sepetim"];
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;

                }

            }

            if (MyCart.Count == 0)
            {
                ViewBag.MyCart = null;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Order() 
        {
            if (HttpContext.Session.GetString("Email")!=null)
            {
                User? user = Cls_User.SelectMemberInfo(HttpContext.Session.GetString("Email").ToString());
                return View(user);
                //eğer email null değilse adam giriş yapmış ta gelmiş
            }

            else 
            {
             
                return RedirectToAction("Login"); //giriş yap sayfasına yönlendiriyorum

            
            }
           

        }
        //metod overload = aynı parametre sırasıyla , aynı isimli metodu yazamayız
        //metod overlodad etmek parametre sırası farklı olmalı 
        [HttpPost]
        public IActionResult Order(IFormCollection frm) 
        {
            //string? kredikartno = Request.Form["kredikartno"];  public IActionResult Order(string a) IFormCollection olmadan
            string krediakrtno =frm["kredikartno"].ToString();//IFormCollection zorunlu 
            string kredikartay = frm["kredikartay"].ToString();
            string kredikartyil = frm["kredikartyil"].ToString();
            string kredikartcvs = frm["kredikartcvs"].ToString();

            //bankaya git , eğer true gelirse (onay gelirse ) 
            //order tablosuna kayıt acacagız
            //digital-planet e fatura bilgilerini göster
            //payu,iyzico bu kredi kart bilgilerini göndermek için kullanılan siteler

            string txt_tckimlikno= frm["txt_tckimlikno"].ToString();
            string txt_vergino = frm["txt_vergino"].ToString();

            if (txt_tckimlikno!="")
            {
                WebServiseController.tckimlikno = txt_tckimlikno; 
                //fatura bilgilerini digital-planet şirketine gönderirsiniz(xml formatında)
                //sizin e faturanızı oluşturucak
            }

            else
            {
                WebServiseController.vergino= txt_vergino;
                //burda da vergi no ile gönderiliyor
            }
            //.Net yazılırsa masa üstü windows form ile yazılmıs ir proje gönderiyor onun üzerinden yazılıyor
            NameValueCollection data=new NameValueCollection();
            string url = "https://fatmaarslan.com/backref";

            data.Add("BACK_REF", url); // back ref i url içine ekledim
            data.Add("CC_CCV",kredikartcvs);
            data.Add("CC_NUMBER",krediakrtno);
            data.Add("EXP_MONTH",kredikartay);
            data.Add("EXP_YEAR", kredikartyil);
            //YUKARIDAKİ BİLGİLERİ URL DE Kİ DATA YA TANIMLADIKK

            var deger = "";

            foreach (var item in data)
            {
                var value = item as string;
                var byteCount= Encoding.UTF8.GetBytes(data.Get(value));
                deger += byteCount + data.Get(value);
            }

            var signatureKey = "payu üyeliğinde size verilen SECRET_KEY burada olacak";
            var hash=HashWithSignature(deger, signatureKey);

            data.Add("ORDER_HASH", hash);

            var x = POSTFormPAYU("https://secure.payu.com.tr/order/...", data);

            //sanal kart için yakalıyoruz gelen xml de
            if (x.Contains("<STATUS>SUCCESS</STATUS>")&& x.Contains
                ("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
            {
                //sanal kart okey 
            }

            else
            {
                //gerçek kredi kartı

            }
            return RedirectToAction("backref");
        
        }


        public static string HashWithSignature(string deger, string signatureKey) 
        {
            return "";
        }

        public static string POSTFormPAYU(string url, NameValueCollection data) //yukarıda data yı u tipte çağırdığım için burda da string diye değil bu şekilde çağırdım
        {
            return "";
        }

        public IActionResult backref() 
        {
            Confirm_Order();
            return RedirectToAction("Confirm");
        
        }


        public static string OrderGroupGUID = "";

        public IActionResult Confirm_Order() 
        {
            //SİPARİŞ TABLOSUNA KAYDETME YAPICAZ 
            //COOKİE SEPETİNİ SİLECEĞİZ
            //E FATURA OLUŞTURACAĞIZ,E FATURAYI OLUŞTURAN XML METODU ÇAĞIRICAĞIZ 



            var cookieOptions = new CookieOptions();
            //tarayıcıdan okuma
            var cookie = Request.Cookies["sepetim"];
            if (cookie != null)
            {
                //sepet boşsa
                //sepet doluysa
                    cls_order.MyCart = cookie; // tarayıcıda sepet bilgilerini propertye koydum
                                               //sepet dolu ,aynı ürün değil
                    OrderGroupGUID=cls_order.WriteToOrderTable(HttpContext.Session.GetString("Email"));
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Delete("sepetim");

               bool result= Cls_User.SendSms(OrderGroupGUID);

                if (result==false)
                {
                    //orders tablosunda sms kolonuna false değerine basarız ,sonra admin panele menü yapılır ordaki menü dr ki orders tablosunda sms kolonu  = false olan siparişlri getir deriz o forma 
                }



                // Cls_User.SendEMail(OrderGroupGUID); email bilgisi

                //...  sitesinde müşteriden kredi kart bilgileri alınır , 2.Bu bilgiler payu yada iyzico  gönderiyor bu siteler banka ile haberleşir 
               //3.kredi kartı bilgileri bankaya geldiğinde bankalar kullanıcıta sms bilgisi gönderir
               //4. Banka backref metoduna geri dönüş yapar , banka okey verirse siz bir sms firmasıyla anlaştınız , SendSms metodu müşteriye siparişiniz onaylandı mesajı gönderir
               //biz sms i firmaya(netgsm) göndericeğiz (xml formatında ) o firma sms gönderme işlemi yapacak
               //digitalplanet müüşteriye fatura gönderir , buda şirket xml formatında 

             }


            return RedirectToAction("Confirm");

        }


        public IActionResult Confirm() 
        {
         ViewBag.OrderGroupGUID = OrderGroupGUID;
            return View();
        
        }



        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Register(User user)
        {
            if (Cls_User.loginEmailControl(user) == false)
            {
                bool answer = Cls_User.AddUser(user);

                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }
                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }
            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }
            return View();
        }

        public IActionResult Login() 
        {
          return View();
        }


        [HttpPost]
        public IActionResult Login(User user)
        {
            string answer = Cls_User.MemberControl(user);

            if (answer=="error")
            {
                TempData["Message"] = "Hata , Email yada Şifre yanlış tekrar deneyiniz";
            }
            else if (answer=="admin") 
            {
                //email şifre doğru , admin  olarak giriş yapıyor 
                HttpContext.Session.SetString("Admin", "Admin");
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                //email ve şifre doğru ,sitemizden alışveriş yapan norma kullanıcı
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

       


        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                List<vw_MyOrders> orders = cls_order.SelectMyOrders(HttpContext.Session.GetString("Email").ToString());
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public IActionResult DetailedSearch() 
        {

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Suppliers = context.Suppliers.ToList();
            return View();
        }



        public IActionResult DProducts(int CategoryID, string[] SupplierID, string price, string IsInStock)
        {
            price = price.Replace(" ", "");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0];
            string endprice = PriceArray[1];

            string sign = ">";
            if (IsInStock == "0")
            {
                sign = ">=";
            }

            int count = 0;
            string suppliervalue = ""; //1,2,4
            for (int i = 0; i < SupplierID.Length; i++)
            {
                if (count == 0)
                {
                    suppliervalue = "SupplierID =" + SupplierID[i];
                    count++;
                }
                else
                {
                    suppliervalue += " or SupplierID =" + SupplierID[i];
                }
            }

            string query = "select * from Products where  CategoryID = " + CategoryID +
                " and (" + suppliervalue + ") and (UnitPrice > " + startprice + " and UnitPrice < " + 
                endprice + ") and Stock " + sign + " 0 order by ProductName";

            ViewBag.Products = p.SelectProductsByDetails(query);
            return View();
        }


        public IActionResult ContactUs()
        {
            return View();
        }


		public IActionResult Logout()
		{
			HttpContext.Session.Remove("Email");
			HttpContext.Session.Remove("Admin");
			return RedirectToAction("Index");
		}


		public IActionResult CategoryPage(int id)
		{
			List<Product> products = p.ProductSelectWithCategoryID(id);
			return View(products);
		}

        //Bu işlemi  admin çıkıışa da yaparsın




        public IActionResult SupplierPage(int id)
        {
            List<Product> products = p.ProductSelectWithSupplierID(id);
            return View(products);
        }




        public IActionResult AboutUs()
        {
            return View();
        }



        public PartialViewResult gettingProducts(string id)
        {
            id = id.ToUpper(new System.Globalization.CultureInfo("tr-TR")); 
            List<sp_arama> ulist = Cls_Product.gettingSearchProducts(id);
            string json = JsonConvert.SerializeObject(ulist);
            var response = JsonConvert.DeserializeObject<List<Search>>(json);
            return PartialView(response);
        }


        public IActionResult PharmacyOnDuty()
        {
            /*
             https://openfiles.izmir.bel.tr/111324/docs/ibbapi-WebServisKullanimDokumani_1.0.pdf BUNLAR IP SERVİSLERİNİ KULLANILABİLECEK ADRESLERİ İÇERİYOR.DÖKÜMANLAR
             https://openapi.izmir.bel.tr/api/ibb/cbs/wizmirnetnoktalari
             https://acikveri.bizizmir.com/dataset/kablosuz-internet-baglanti-noktalari/resource/982875a4-2bb6-4178-8ee2-3f07641156bb
             https://acikveri.bizizmir.com/dataset/izban-banliyo-hareket-saatleri
            */



            //https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler



            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler");



            var pharmacy = JsonConvert.DeserializeObject<List<Pharmacy>>(json);



            return View(pharmacy);
        }



        public IActionResult ArtAndCulture()
        {
            //https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler



            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler");



            var activite = JsonConvert.DeserializeObject<List<Activite>>(json);



            return View(activite);
        }

    }
}
