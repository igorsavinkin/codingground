using System.IO;
using System;
using System.Net; 
using System.Text; 

class Recaptcha2captcha
{
    static string site_key = "6Lf5CQkTAAAAAKA-kgNm9mV6sgqpGmRmRMFJYMz8";
    static string captcha_service_API_KEY= "1069c3052adead147d1736d7802fabe2";
    public string SendRequest() // HTTP POST
    { 
        var page_url = "http://testing-ground.scraping.pro/recaptcha"; 
        
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

           // responseString.Close();
            response.Close();

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
    public string SubmitForm(string RecaptchaResponseToken)  // HTTP POST
    { 
        var page_url = "http://testing-ground.scraping.pro/recaptcha"; 
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            var request = (HttpWebRequest)WebRequest.Create(page_url);
           
            var postData = "submit=submin&g-recaptcha-response="  + RecaptchaResponseToken;
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
 
            response.Close();
 
            return responseString;
        }
        catch (Exception e)
        { 
            return e.Message;
        }

    }
    
    // the request to retrieve g-recaptcha-response token from 2captcha service
    public string getToken(string captcha_id)  // HTTP GET
    {
        WebClient webClient = new WebClient();
        webClient.QueryString.Add("key", Recaptcha2captcha.captcha_service_API_KEY);
        webClient.QueryString.Add("action", "get");
        webClient.QueryString.Add("id", captcha_id);
        
        return webClient.DownloadString("http://2captcha.com/res.php");
    }
    // validate site with returned token thru proxy.php  
    public string getValidate(string token)
    {
        WebClient webClient = new WebClient();
        webClient.QueryString.Add("response", token); 
        return webClient.DownloadString("http://testing-ground.scraping.pro/proxy.php");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("2captcha test");
        Recaptcha2captcha service = new Recaptcha2captcha();
        var resp = service.SendRequest();
        var gcaptchaToken = "";
        Console.WriteLine(resp.Substring( 3, resp.Length-3));
        if (resp.Contains("OK|")){
            // loop till the service solves captcha, gets g-recaptcha-response token
            var i=0;
            while (i++ <= 20) 
            {
                System.Threading.Thread.Sleep(5000); // sleep 5 seconds
                Console.WriteLine("Captcha is being solved for {0} seconds", i*5);
                gcaptchaToken = service.getToken(resp.Substring( 3, resp.Length-3)); 
                if (gcaptchaToken.Contains("OK|"))
                { 
                    break;
                }
            }
            
            
            if (gcaptchaToken.Contains("OK|"))
            { 
                var RecaptchaResponseToken = gcaptchaToken.Substring(3, gcaptchaToken.Length-3);
                Console.WriteLine("g-recaptcha-response token:  " + RecaptchaResponseToken );
                
                // make google to validate g-recaptcha-response token 
                var iSvalid = service.getValidate(RecaptchaResponseToken); 
                Console.WriteLine("Token is validated by google: " + iSvalid );
                
                // submit form to the target site
                var SubmitFormResp = service.SubmitForm(RecaptchaResponseToken);
                Console.WriteLine("Submit form return: " + SubmitFormResp );
                
            } else {
                Console.WriteLine("Captcha has not been solved. Error code: " + gcaptchaToken);
                Environment.Exit(0);
            }
            
        } else {
            Console.WriteLine("Error: " + resp);
            Environment.Exit(0);
        }
    }
}
