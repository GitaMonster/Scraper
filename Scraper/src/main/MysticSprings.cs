using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading;

using Scraper.Model;
using Scraper.Parser;


namespace Scraper
{
    class MysticSprings
    {
        //the below is copied from fire mountain, but adjusted accordingly for mystic springs:

        private static readonly string REQUEST_URL = "http://reservation.worldweb.com/Bookings-nr105/activity-edit.html?command=roomsearch&table=hotels&mode=command&listing_id=2305&hotel_id=2305&language=english&exchange_rates=CAD&date_from={0}-{1}-{2}&date_to={3}-{4}-{5}&arr_date={2}-{6}-{0}&num_nights={7}&num_adults=2&access_code=";

        public static readonly DateTime START_DATE = DateTime.Now;
        public static readonly DateTime END_DATE = new DateTime(2019, 8, 15);

        private static HttpClient httpClient = new HttpClient();

        public static void Run()  //just for testing
        {
            SetHttpHeaders();
            DateTime currentDate = new DateTime(2018, 10, 16);
            HotelAvailability hotelAvailability = new HotelAvailability(HotelName.MYSTIC_SPRINGS);
            hotelAvailability.RoomAvailabilities.Add(MysticSpringsParser.TWO_BEDROOM_CHALET, new RoomAvailability(MysticSpringsParser.TWO_BEDROOM_CHALET));

            string response = GetAvailabilityResponse(currentDate, currentDate.AddDays(1));
            MysticSpringsParser.ParseHotelAvailability(hotelAvailability, response, currentDate);
            //    currentDate = currentDate.AddDays(1);


            bool test = hotelAvailability.RoomAvailabilities[MysticSpringsParser.TWO_BEDROOM_CHALET].IsAvailableOnDate(currentDate);
            Console.WriteLine(response);
            Console.WriteLine(test);
            Console.ReadLine();
            //HotelAvailability hotelAvailability = MysticSpringsParser.ParseHotelAvailability(response);
        }

        public static ResortAvailability GetResortAvailability(DateTime startDate, DateTime endDate)
        {
            HotelAvailability hotelAvailability = GetHotelAvailability(startDate, endDate);
            return new ResortAvailability(ResortName.MYSTIC_SPRINGS, new Dictionary<HotelName, HotelAvailability> { { HotelName.MYSTIC_SPRINGS, hotelAvailability } });
        }

        public static HotelAvailability GetHotelAvailability(DateTime startDate, DateTime endDate)
        {
            DateTime currentDate = startDate;
            HotelAvailability hotelAvailability = new HotelAvailability(HotelName.MYSTIC_SPRINGS);
            hotelAvailability.RoomAvailabilities.Add(MysticSpringsParser.TWO_BEDROOM_CHALET, new RoomAvailability(MysticSpringsParser.TWO_BEDROOM_CHALET));
            SetHttpHeaders();

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfYear == new DateTime(2018, 10, 16).DayOfYear)
                {
                    Console.WriteLine("...");
                }
                Console.WriteLine("Getting availability for " + currentDate);
                string response = GetAvailabilityResponse(currentDate, currentDate.AddDays(1));
                MysticSpringsParser.ParseHotelAvailability(hotelAvailability, response, currentDate);
                currentDate = currentDate.AddDays(1);
                Thread.Sleep(1000);
            }

            return hotelAvailability;
        }

        private static string GetAvailabilityResponse(DateTime startDate, DateTime endDate)
        {
            //also have min stay as 1 night currently:
            string url = string.Format(REQUEST_URL, startDate.Year, startDate.ToString("MM"), startDate.ToString("dd"), endDate.Year, endDate.ToString("MM"), endDate.ToString("dd"), startDate.ToString("MMM"), "1");

            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private static void SetHttpHeaders()
        {
            // TODO: Accept gzip encoding and unzip it on our side

            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");  //ADDED
            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate"); //ADDED
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0"); //ADDED
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive"); //ADDED
            //httpClient.DefaultRequestHeaders.Add("Cookie", "webrezpro_click_id2305=259435067"); //ADDED
            httpClient.DefaultRequestHeaders.Add("Host", "reservation.worldweb.com"); //ADDED
            httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1"); //ADDED

            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");  //THIS STAYS THE SAME
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.mysticsprings.ca/booking_engine.htm");  //CHANGED THIS
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");  //THIS STAYS THE SAME
        }

    }
}

