using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scraper.Model;
using Scraper.parser;

namespace Scraper
{
    class FireMountain
    {
        private static readonly string REQUEST_URL = "https://secure2.webrez.com/Bookings105/activity-edit.html?callback=jQuery214022616191772049077_1531065464591&mode=command&command=roomsearch_version2&table=hotels&transaction_id=-1&listing_id=1510&hotel_id=1510&date_from={0}-{1}-{2}&date_to={3}-{4}-{5}&access_code=&package_ids=&currency=CAD&_=1531065464592";
        public static readonly DateTime START_DATE = DateTime.Now;
        public static readonly DateTime END_DATE = new DateTime(2019, 8, 15);

        private static HttpClient httpClient = new HttpClient();

        public static void Run()
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            HotelAvailability hotelAvailability = FireMountainParser.ParseHotelAvailability(response);
        }

        public static ResortAvailability GetResortAvailability(DateTime startDate, DateTime endDate)
        {
            HotelAvailability hotelAvailability = GetHotelAvailability(startDate, endDate);
            return new ResortAvailability(ResortName.FIRE_MOUNTAIN, new Dictionary<HotelName, HotelAvailability> { { HotelName.FIRE_MOUNTAIN, hotelAvailability } });
        }

        public static HotelAvailability GetHotelAvailability(DateTime startDate, DateTime endDate)
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            return FireMountainParser.ParseHotelAvailability(response);
        }

        private static string GetAvailabilityResponse(DateTime startDate, DateTime endDate)
        {
            string url = string.Format(REQUEST_URL, startDate.Year, startDate.ToString("MM"), startDate.ToString("dd"), endDate.Year, endDate.ToString("MM"), endDate.ToString("dd"));
            // TODO: Accept gzip encoding and unzip it on our side
            // httpClient.DefaultRequestHeaders.Add("Accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://book.webrez.com/v30/");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");

            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
