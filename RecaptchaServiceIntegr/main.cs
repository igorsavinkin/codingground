using System.IO;
using System;
using System.Net; 
using System.Text;
using System.Windows;

class Recaptcha2captcha
{
    static string site_key = "6Lf5CQkTAAAAAKA-kgNm9mV6sgqpGmRmRMFJYMz8";
    static string captcha_service_API_KEY= "1069c3052adead147d1736d7802fabe2";
    public string SendRequest()
    { 
        var page_url = "http://testing-ground.scraping.pro/recaptcha"; 
        //POST
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            var request = (HttpWebRequest)WebRequest.Create("http://2captcha.com/in.php");
           
            var postData = "key="+ Recaptcha2captcha.captcha_service_API_KEY +"&method=userrecaptcha&googlekey=" + Recaptcha2captcha.site_key + "&page_url=" + page_url;
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
    public string get_gcaptcha_response_token(string captcha_id)
    {
        //GET
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            var request = (HttpWebRequest)WebRequest.Create("http://2captcha.com/res.php");
               
            var postData = "key=" + Recaptcha2captcha.captcha_service_API_KEY + "&action=get&id=" + captcha_id;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "GET";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //  
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
            // loop till the service solves captcha
            var i=0;
            while (i++ < 20) 
            {
                System.Threading.Thread.Sleep(5000); // sleep 5 seconds
                Console.WriteLine("Captcha is being solved for {0} seconds", i*5);
                resp = service.get_gcaptcha_response_token(resp.Substring( 3, resp.Length-3));
                if (resp.Contains("OK|"))
                { 
                    break;
                }
            }
            if (resp.Contains("OK|"))
            { 
                var token = resp.Substring(3, resp.Length-3);
                Console.WriteLine("g-recaptcha-response token:  " + token );
            } else {
                Console.WriteLine("Captcha has not been solved. Error: " + resp);
                Environment.Exit(0);
            }
            
        } else {
            Console.WriteLine("Error: " + resp);
            Environment.Exit(0);
        }
    }
}
