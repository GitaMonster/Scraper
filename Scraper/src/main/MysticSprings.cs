using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading;

using Scraper.Model;
using Scraper.parser;


namespace Scraper
{
    class MysticSprings
    {
        private static readonly string REQUEST_URL = "http://reservation.worldweb.com/Bookings-nr105/activity-edit.html?command=roomsearch&table=hotels&mode=command&listing_id=2305&hotel_id=2305&language=english&exchange_rates=CAD&date_from={0}-{1}-{2}&date_to={3}-{4}-{5}&arr_date={2}-{6}-{0}&num_nights={7}&num_adults=2&access_code=";

        public static readonly DateTime START_DATE = DateTime.Now;
        public static readonly DateTime END_DATE = new DateTime(2019, 8, 15);

        private static HttpClient httpClient = new HttpClient();

        //public static void Run()  //just for testing
        //{
        //    SetHttpHeaders();
        //    DateTime currentDate = new DateTime(2018, 10, 16);
        //    HotelAvailability hotelAvailability = new HotelAvailability(HotelName.MYSTIC_SPRINGS);
        //    hotelAvailability.RoomAvailabilities.Add(MysticSpringsParser.TWO_BEDROOM_CHALET, new RoomAvailability(MysticSpringsParser.TWO_BEDROOM_CHALET));

        //    string response = GetAvailabilityResponse(currentDate, currentDate.AddDays(1));
        //    MysticSpringsParser.ParseHotelAvailability(hotelAvailability, response, currentDate);
        //    //    currentDate = currentDate.AddDays(1);

        //    bool test = hotelAvailability.RoomAvailabilities[MysticSpringsParser.TWO_BEDROOM_CHALET].IsAvailableOnDate(currentDate);
        //    Console.WriteLine(response);
        //    Console.WriteLine(test);
        //    Console.ReadLine();
        //    //HotelAvailability hotelAvailability = MysticSpringsParser.ParseHotelAvailability(response);
        //}

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

            //make randomized list of dates:
            TimeSpan totalNumOfDays = endDate.Subtract(startDate);
            int days = totalNumOfDays.Days;
            Random random = new Random();
            HashSet<int> daysToAdd = new HashSet<int>();

            //while currentDate has not yet been generated:
            while (daysToAdd.Count <= days)
            {
                currentDate = startDate;
                //generate random numbers until all dates have been randomly generated

                //return a random number within the specified range; inclusive lower bound, exclusive upper bound
                int randomCounter = random.Next(0, days + 1);  //+ because want to include the last day, and the upper bound is exclusive.
                //ensure that randomCounter is not a duplicate by adding it to the HashSet:
                if (daysToAdd.Add(randomCounter))
                {
                    currentDate = currentDate.AddDays(randomCounter);
                    Console.WriteLine("Getting availability for " + currentDate);
                    string response = GetAvailabilityResponse(currentDate, currentDate.AddDays(1));
                    MysticSpringsParser.ParseHotelAvailability(hotelAvailability, response, currentDate);
                    Thread.Sleep(1500);
                }
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
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"); 
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive"); 
            httpClient.DefaultRequestHeaders.Add("Host", "reservation.worldweb.com"); 
            httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1"); 
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9"); 
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.mysticsprings.ca/booking_engine.htm");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
        }

    }
}

