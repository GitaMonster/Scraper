using Scraper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Scraper.parser;
using Scraper.src.parser;

namespace Scraper
{
    class SummitPenthouses
    {
        //NOTE THAT FOR SUMMIT PENTHOUSES, AVAILABILITY ON WEBSITE MAY APPEAR DIFFERENTLY THAN AVAILABILITY IN EXCEL SHEETS, ONLY BECAUSE THERE IS A 2 NIGHT MINIMUM; 
        //THE AVAILABILITY IN THE EXCEL SHEETS IS ACCURATE, AS LONG AS CIRRUS MAINTAINS THE SAME MINIMUM STAY (DEFAULT 2 NIGHTS) AS THE RESORT.

        //the rooms that are available on the website are particular unit numbers; so if one unit is available, the same one has
        //to be available the next night (2 night min) or the previous night as well, not a different room; room changes do not count as available.

        private static readonly string REQUEST_URL = "https://secure2.webrez.com/Bookings105/activity-edit.html?callback=jQuery2140809114576402336_1531835696610&mode=command&command=roomsearch_version2&table=hotels&transaction_id=-1&listing_id=1759&hotel_id=1759&date_from={0}-{1}-{2}&date_to={3}-{4}-{5}&access_code=&package_ids=&currency=CAD&_=1531835696611";

        public static readonly DateTime START_DATE = DateTime.Now;
        public static readonly DateTime END_DATE = new DateTime(2019, 10, 10);

        private static HttpClient httpClient = new HttpClient();

        public static void Run()
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            HotelAvailability hotelAvailability = SummitPenthousesParser.ParseHotelAvailability(response);
        }

        public static ResortAvailability GetResortAvailability(DateTime startDate, DateTime endDate)
        {
            HotelAvailability hotelAvailability = GetHotelAvailability(startDate, endDate);
            return new ResortAvailability(ResortName.SUMMIT_PENTHOUSES, new Dictionary<HotelName, HotelAvailability> { { HotelName.SUMMIT_PENTHOUSES, hotelAvailability } });
        }

        public static HotelAvailability GetHotelAvailability(DateTime startDate, DateTime endDate)
        {
            string response = GetAvailabilityResponse(START_DATE, END_DATE);
            return SummitPenthousesParser.ParseHotelAvailability(response);
        }

        private static string GetAvailabilityResponse(DateTime startDate, DateTime endDate)
        {
            string url = string.Format(REQUEST_URL, startDate.Year, startDate.ToString("MM"), startDate.ToString("dd"), endDate.Year, endDate.ToString("MM"), endDate.ToString("dd"));
            // TODO: Accept gzip encoding and unzip it on our side
            // httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
            //httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("referer", "https://book.webrez.com/v28/");
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");

            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

    }
}
