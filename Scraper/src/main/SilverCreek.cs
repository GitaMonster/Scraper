using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    class SilverCreek
    {
        private static readonly string UTM_COOKIES = "__utmc=19053498; __utma=19053498.2068434907.1527465609.1527465609.1527467484.2; __utmb=19053498; __utmz=19053498.1527467484.2.2.utmccn=(referral)|utmcsr=google.ca|utmcct=/|utmcmd=referral";

        static readonly HttpClientHandler httpClientHandler = new HttpClientHandler();
        static HttpClient httpClient;

        // TODO: Look at following location headers instead of hardcoding URLs?
        public static void Run()
        {
            SetUpHttpClient();

            string cookie = GetAspCookie();
            Console.WriteLine("Cookie = " + cookie);
            string secondCookie = GetSecondAspCookie(cookie);
            Console.WriteLine("Second cookie = " + secondCookie);
            GetIQHomePage(secondCookie);
            GetAgentLoginPage(secondCookie);
            string page = GetAvailabilityPage(secondCookie);
            Console.WriteLine("Sold = " + page.IndexOf("Sold"));
            Console.WriteLine("419 = " + page.IndexOf("419"));
            Console.WriteLine(page);
        }

        private static void SetUpHttpClient()
        {
            httpClientHandler.AllowAutoRedirect = false;
            httpClientHandler.UseCookies = false;
            httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Cache-control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Referrer", "http://silvercreekcanmore.ca/booking_engine.htm");
            httpClient.DefaultRequestHeaders.Add("Upgrade-insecure-requests", "1");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
        }

        private static string GetAspCookie()
        {
            string url = "https://reservations.silvercreekcanmore.ca/iqreservations/default.asp?CIM=6&CID=20&CIY=2018&COM=6&COD=21&COY=2018&checkInDate=20-Jun-2018&checkOutDate=21-Jun-2018&AD=2&CH=0&RMS=1&promoCode=&submit=Check+Availability";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", UTM_COOKIES);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            string responseCookie = response.Headers.GetValues("Set-Cookie").ToList()[0];
            responseCookie = responseCookie.Split(';')[0];
            return responseCookie;
        }

        private static string GetSecondAspCookie(string firstCookie)
        {
            string url = "https://reservations.silvercreekcanmore.ca/iqreservations/asp/home.asp?CIM=6&CID=20&CIY=2018&COM=6&COD=21&COY=2018&checkInDate=20-Jun-2018&checkOutDate=21-Jun-2018&AD=2&CH=0&RMS=1&promoCode=&submit=Check+Availability";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            string cookie = UTM_COOKIES + "; " + firstCookie;
            request.Headers.Add("Cookie", cookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            string responseCookie = response.Headers.GetValues("Set-Cookie").ToList()[0];
            responseCookie = responseCookie.Split(';')[0];
            return responseCookie;
        }

        private static void GetIQHomePage(string mainCookie)
        {
            string url = "https://reservations.silvercreekcanmore.ca/iqreservations/asp/IQHome.asp";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            string cookie = UTM_COOKIES + "; " + mainCookie;
            request.Headers.Add("Cookie", cookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;
        }

        private static void GetAgentLoginPage(string mainCookie)
        {
            string url = "https://reservations.silvercreekcanmore.ca/iqreservations/asp/AgentLogin.asp?CheckInMonth=6&CheckInDay=20&CheckInYear=2018&CheckOutMonth=6&CheckOutDay=21&CheckOutYear=2018&txtAdults=2&txtChildren=0&txtNumRooms=1&txtPromoCode=&txtRateSelected=&ForcedUser1=&ForcedUser2=";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            string cookie = UTM_COOKIES + "; " + mainCookie;
            request.Headers.Add("Cookie", cookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;
        }

        private static string GetAvailabilityPage(string mainCookie)
        {
            string url = "https://reservations.silvercreekcanmore.ca/iqreservations/asp/CheckAvailability.asp";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            string cookie = UTM_COOKIES + "; " + mainCookie;
            request.Headers.Add("Cookie", cookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
