using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

using Scraper.Model;
using Scraper.Parser;
using Scraper.Util;

namespace Scraper
{
    class SilverCreek
    {
        private static readonly string UTM_COOKIES = "__utmc=19053498; __utma=19053498.2068434907.1527465609.1527465609.1527467484.2; __utmb=19053498; __utmz=19053498.1527467484.2.2.utmccn=(referral)|utmcsr=google.ca|utmcct=/|utmcmd=referral";
        private static readonly string REQUESTS_ROOT_URL = "https://reservations.silvercreekcanmore.ca/iqreservations/";
        public static readonly DateTime START_DATE = new DateTime(2018, 7, 1);
        public static readonly DateTime END_DATE = new DateTime(2018, 9, 15);

        private static readonly HttpClientHandler httpClientHandler = new HttpClientHandler();
        private static HttpClient httpClient;

        public static void Run()
        {
            ResortAvailability resortAvailability = GetResortAvailability(START_DATE, END_DATE);

            foreach (KeyValuePair<string, RoomAvailability> roomAvailability in resortAvailability.HotelAvailabilities[HotelName.SILVER_CREEK].RoomAvailabilities)
            {
                foreach (KeyValuePair<DateTime, AvailabilityType> availability in roomAvailability.Value.TotalAvailability)
                {
                    Console.WriteLine(DateUtils.GetReadableDateFormat(availability.Key) + ": " + availability.Value);
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("SUCCESS");
        }

        public static ResortAvailability GetResortAvailability(DateTime startDate, DateTime endDate)
        {
            SetUpHttpClient();

            // TODO: Look at following location headers instead of hardcoding URLs?
            string cookieRequestsQueryString = string.Format("?CIM={0}&CID={1}&CIY={2}&COM={3}&COD={4}&COY={5}&checkInDate={1}-{6}-{2}&checkOutDate={4}-{7}-{5}&AD=2&CH=0&RMS=1&promoCode=&submit=Check+Availability",
                           startDate.Month,
                           startDate.ToString("dd"),
                           startDate.Year,
                           endDate.Month,
                           endDate.ToString("dd"),
                           endDate.Year,
                           startDate.ToString("MMM"),
                           endDate.ToString("MMM"));

            string initialCookieRequestUrl = REQUESTS_ROOT_URL + "default.asp" + cookieRequestsQueryString;
            string initialAspCookie = GetAspCookie(UTM_COOKIES, initialCookieRequestUrl);

            string mainCookieRequestUrl = REQUESTS_ROOT_URL + "asp/home.asp" + cookieRequestsQueryString;
            string mainAspCookie = GetAspCookie(UTM_COOKIES + "; " + initialAspCookie, mainCookieRequestUrl);

            string fullCookie = UTM_COOKIES + "; " + mainAspCookie;

            HotelAvailability hotelAvailability = GetHotelAvailability(startDate, endDate, fullCookie);
            hotelAvailability.TrimDateRange(startDate, endDate);

            return new ResortAvailability(ResortName.SILVER_CREEK, new Dictionary<HotelName, HotelAvailability> {{HotelName.SILVER_CREEK, hotelAvailability}});
        }

        private static HotelAvailability GetHotelAvailability(DateTime startDate, DateTime endDate, String fullCookie)
        {
            string iqHomePageUrl = REQUESTS_ROOT_URL + "asp/IQHome.asp";
            string availabilityPageUrl = REQUESTS_ROOT_URL + "asp/CheckAvailability.asp";

            HotelAvailability hotelAvailability = new HotelAvailability(HotelName.SILVER_CREEK);

            List<DateTime> datesToRequest = GetDatesToRequest(startDate, endDate);
            foreach (DateTime requestDate in datesToRequest)
            {
                Console.WriteLine("Getting availabilities for dates around " + DateUtils.GetReadableDateFormat(requestDate));
                GetPage(iqHomePageUrl, fullCookie);
                DateTime oneMoreThanRequestDate = requestDate.AddDays(1);
                string agentLoginPageQueryString = string.Format("?CheckInMonth={0}&CheckInDay={1}&CheckInYear={2}&CheckOutMonth={3}&CheckOutDay={4}&CheckOutYear={5}&txtAdults=2&txtChildren=0&txtNumRooms=1&txtPromoCode=&txtRateSelected=&ForcedUser1=&ForcedUser2=",
                                                  requestDate.Month,
                                                  requestDate.ToString("dd"),
                                                  requestDate.Year,
                                                  oneMoreThanRequestDate.Month,
                                                  oneMoreThanRequestDate.ToString("dd"),
                                                  oneMoreThanRequestDate.Year);
                string agentLoginPageUrl = REQUESTS_ROOT_URL + "asp/AgentLogin.asp" + agentLoginPageQueryString;
                GetPage(agentLoginPageUrl, fullCookie);
                string pageForCurrentDate = GetPage(availabilityPageUrl, fullCookie);

                SilverCreekParser.Parse(pageForCurrentDate, hotelAvailability, requestDate);
            }

            return hotelAvailability;
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

        private static string GetAspCookie(string requestCookie, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Cookie", requestCookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            string responseCookie = response.Headers.GetValues("Set-Cookie").ToList()[0];
            responseCookie = responseCookie.Split(';')[0];
            return responseCookie;
        }

        private static string GetPage(string url, string cookie)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Cookie", cookie);
            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        private static List<DateTime> GetDatesToRequest(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime firstRequestDate = startDate.AddDays(6);
            if (firstRequestDate > endDate)
            {
                dates.Add(startDate);
                return dates;
            }
            dates.Add(firstRequestDate);

            DateTime nextRequestDate = firstRequestDate;
            while (nextRequestDate.AddDays(7) < endDate)
            {
                nextRequestDate = nextRequestDate.AddDays(14);
                dates.Add(nextRequestDate);
            }
            return dates;
        }
    }
}
