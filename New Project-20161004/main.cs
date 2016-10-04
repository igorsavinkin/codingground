using System.IO;
using System;
using System.Net; 
using System.Text;
using System.Windows.Forms;

class Recaptcha2captcha
{
    public string SendRequest()
    {
        //POST
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            var request = (HttpWebRequest)WebRequest.Create("http://2captcha.com/in.php");
            
            var captcha_service_API_KEY= "1069c3052adead147d1736d7802fabe2";
            var page_url = "http://testing-ground.scraping.pro/recaptcha";
            var site_key = "6Lf5CQkTAAAAAKA-kgNm9mV6sgqpGmRmRMFJYMz8";
            var postData = "key="+ captcha_service_API_KEY +"&method=userrecaptcha&googlekey=" + site_key + "&page_url=" + page_url;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //  GET
            if (responseString.Contains("OK|"))
            {
                return responseString;
            }
            else
            {
                return "___Error";
            }
        }
        catch (Exception e)
        { 
            return e.Message;
        }

    }
    
}

class Program
{
    static void Main()
    {
        Console.WriteLine("2captcha test");
        Recaptcha2captcha service = new Recaptcha2captcha();
        var resp = service.SendRequest();
        Console.WriteLine(resp.Substring( 3, resp.Length-3));
        if (resp.Contains("OK|")){
            // loop till the captcha is solved
            
            
        } else {
            Console.WriteLine("Error: " + resp);
            Application.Exit();
        }
    }
}
