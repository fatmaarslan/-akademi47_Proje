
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;
using XSystem.Security.Cryptography;

namespace İakademi47_Proje.Models
{
	public class Cls_User

	{

		iakademi47Context context= new iakademi47Context();

        public static byte[] Endcoding { get; private set; }

        public async Task<User>loginControl(User user) 
		{

			string md5sifrele = MD5Sifrele(user.Password);
		
		
			User?  usr =await context.Users.FirstOrDefaultAsync(u => u.Email==user.Email&& u.Password==md5sifrele && u.IsAdmin==true && u.Active==true );


				return usr;
		}


        public static string MD5Sifrele(string value)
        {
            //using XSystem.Security.Cryptography;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] btr = Encoding.UTF8.GetBytes(value);
            btr = md5.ComputeHash(btr);

            StringBuilder sb = new StringBuilder();
            foreach (byte item in btr)
            {
                sb.Append(item.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        public static User? SelectMemberInfo(string email)
        {
            using (iakademi47Context context = new iakademi47Context())
            {
                User? user = context.Users.FirstOrDefault(u => u.Email == email);
                return user;
            }
        }


        public static bool loginEmailControl(User user)
        {
            using (iakademi47Context context = new iakademi47Context())
            {
                User? usr = context.Users.FirstOrDefault(u => u.Email == user.Email);

                if (usr == null)
                {
                    return false;
                }
                return true;
            }
        }


        public static bool AddUser(User user)
        {
            using (iakademi47Context context = new iakademi47Context())
            {
                try
                {
                    user.Active = true;
                    user.IsAdmin = false;
                    user.Password = MD5Sifrele(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static string MemberControl(User user)
        {
            using (iakademi47Context context = new iakademi47Context()) //statik olursa context böyle çağırılıyor
            {
                string answer = "";

                try
                {
                    string md5Sifre = MD5Sifrele(user.Password);
                    User? usr = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == md5Sifre);

                    if (usr == null)
                    {
                        //kullanıcı yanlıs sifre veya email girdi
                        answer = "error";
                    }
                    else
                    {
                        //kullanıcı veritabanında kayıtlı.
                        if (usr.IsAdmin == true)
                        {
                            //admin yetkisi olan personel giriş yapıyor
                            answer = "admin";
                        }
                        else
                        {
                            answer = usr.Email;
                        }
                    }
                }
                catch (Exception)
                {
                    return "HATA";
                }
                return answer;
            }
        }



        public static bool SendSms(string OrderGroupGUID)
        {

            try
            {
                using (iakademi47Context context = new iakademi47Context()) 
                
                {
                    string ss = "";


                    ss += "<?xml versiyon='1.0' endcoding='UTF-8'>";
                    ss += "<mainbody>";
                    ss += "<header>";
                    ss += "<company dil='TR'>üyelikte size verilen şirket ismi buraya yazılacak</company>";
                    ss += "<usercode>üye olurken size verilen user code</usercode>";
                    ss += "<password>size verilen şifre</password>";
                    ss += "<startdate>ne zaman baslıcak </startdate>";
                    ss += "<stopdate>ne zaman  biticek</stopdate>";
                    ss += "<msgheader>mesaj aşlığı buraya</msgheader>";
                    ss += "</header>";
                    ss += "<body>";

                    int UserID = context.Orders.FirstOrDefault(o=>o.OrderGroupGUID==OrderGroupGUID).UserID;

                    User user = context.Users.FirstOrDefault(u => u.UserID == UserID); //user dan gelen bütün bilgiler

                    string içerik = "Sayın "+user.NameSurname+","+DateTime.Now+"tarihinde ," + OrderGroupGUID+" nolu siparişiniz alınmıştır.";
                    //dinamik kod yazdım.

                    ss += "<mp><msg><!CDATA["+içerik+"]></msg><no>"+user.Telephone+"</no></mp>";
                    ss += "</body>";
                    ss += "</mainbody>";


                    string result = XmlPost("https://api.netgsm.com.tr/xmlbulpost.asp", ss);

                    if (result !="-1")
                    {
                        //sms gitti , order tablosunda order kolonuna true bas
                    }

                    else
                    {
                        //sms gitmedi order tablosunda sms kolonuna false bas
                        //ilgili admin personeline sms gönder
                        //ilgili admin personeline ema,l gönder
                    }
                    return true;
                }

                
            }
            catch (Exception)
            {

                return false;
            }


        }



        public static string XmlPost(string url, string ss)
        {
            //bu metod sayesinde mesajlaşmaya yarıyor


            try
            {
                /*sms  gönderiyor */
                WebClient wUpload = new WebClient();

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest; //casting , dönüştürme yaptım


                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                Byte[] bPostArray = Encoding.UTF8.GetBytes(ss);
                Byte[] bRespponse = wUpload.UploadData(url, "POST", bPostArray);

                Char[] sReturnChar = Encoding.UTF8.GetChars(bRespponse);

                string sWebpage = new string(sReturnChar);

                return sWebpage;
            }
            catch (Exception)
            {

                return "-1";
            }

          
        }



        public static void SendEmail(string OrderGroupGUID)  //email yollamakk içinn genel kulanılabilecek önemli bir metod bir şey dönmüyor return a gerek yok
        {
            using (iakademi47Context context = new iakademi47Context())
            {
                Order order = context.Orders.FirstOrDefault(o => o.OrderGroupGUID == OrderGroupGUID);
                User user = context.Users.FirstOrDefault(u => u.UserID == order.UserID);

                string mail = "gonderen email buraya info@iakademi.com";
                string _mail = user.Email; //alıcı
                string subject = "";
                string content = "";

                content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

                subject = "Sayın " + user.NameSurname + " siparişiniz alınmıştır.";

                //bu bilgiler veri tabanından alınır normalde yoksa hardcode olur yönetim panelinden form yapıp ordan değiştirebilir
                string host = "smtp.iakademi.com"; 
                int port = 587;
                string login = "mailserver a baglanılan login buraya";
                string password = "mailserver a baglanılan şifre buraya";

                MailMessage e_posta = new MailMessage();
                e_posta.From = new MailAddress(mail, "iakademi bilgi"); //gönderen
                e_posta.To.Add(_mail); //alıcı
                e_posta.Subject = subject;
                e_posta.IsBodyHtml = true;
                e_posta.Body = content;

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential(login, password);
                smtp.Port = port;
                smtp.Host = host;

                try
                {
                    smtp.Send(e_posta);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }



    }
}
