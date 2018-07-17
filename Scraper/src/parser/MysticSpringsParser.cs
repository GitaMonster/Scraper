using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Scraper.Model;

namespace Scraper.Parser
{
    class MysticSpringsParser
    {
        public static readonly string TWO_BEDROOM_CHALET = "2 Bedroom Chalet";

        //if the webpage, under table class="orrs_search_table1" Contains "2 Bedroom Chalet", then it's avail.
        public static void ParseHotelAvailability(HotelAvailability hotelAvailability, string response, DateTime date)
        {

            Dictionary<DateTime, AvailabilityType> totalAvailability = hotelAvailability.RoomAvailabilities[TWO_BEDROOM_CHALET].TotalAvailability;    //the string is the key, of the key-value pair of the only dictionary entry

            if (response.Contains(TWO_BEDROOM_CHALET))
            {
                totalAvailability.Add(date, AvailabilityType.AVAILABLE);
            }
            else
            {
                totalAvailability.Add(date, AvailabilityType.UNAVAILABLE);
            }
            
        }

    }
}
