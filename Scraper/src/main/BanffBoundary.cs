using System;
using System.Collections.Generic;
using System.Net.Http;

using Scraper.Parser;
using Scraper.Model;

namespace Scraper
{
    class BanffBoundary
    {
        private static readonly string REQUEST_URL = "https://secure2.webrez.com//Bookings105/activity-edit.html?callback=jQuery214028903079106275786_1528253322949&mode=command&command=roomsearch_version2&table=hotels&transaction_id=-1&listing_id=1551&hotel_id=1551&date_from={0}-{1}-{2}&date_to={3}-{4}-{5}&access_code=&package_ids=&currency=CAD&version=30&_=1528253322950";
        public static readonly DateTime START_DATE = new DateTime(2018, 7, 1);
        public static readonly DateTime END_DATE = new DateTime(2018, 9, 15);

        private static HttpClient httpClient = new HttpClient();

        public static void Run()
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            HotelAvailability hotelAvailability = BanffBoundaryParser.ParseHotelAvailability(response);
        }

        public static ResortAvailability GetResortAvailability(DateTime startDate, DateTime endDate)
        {
            HotelAvailability hotelAvailability = GetHotelAvailability(startDate, endDate);
            return new ResortAvailability(ResortName.BANFF_BOUNDARY, new Dictionary<HotelName, HotelAvailability> { { HotelName.BANFF_BOUNDARY, hotelAvailability } });
        }

        public static HotelAvailability GetHotelAvailability(DateTime startDate, DateTime endDate)
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            return BanffBoundaryParser.ParseHotelAvailability(response);
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
