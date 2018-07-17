using Newtonsoft.Json;
using Scraper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.src.parser
{
    class SummitPenthousesParser
    {
        public static HotelAvailability ParseHotelAvailability(string response)
        {
            Dictionary<string, RoomAvailability> roomAvailabilities = new Dictionary<string, RoomAvailability>();
            int startIndex = response.IndexOf('{');
            int endIndex = response.Length - 1;
            string json = response.Substring(startIndex, endIndex - startIndex);

            Dictionary<string, dynamic> root = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            List<dynamic> roomAvailabilityData = root["room_types"].ToObject<List<dynamic>>();

            foreach (dynamic room in roomAvailabilityData)
            {
                RoomAvailability roomAvailability = ParseSingleRoomAvailability(room.ToObject<Dictionary<string, dynamic>>());
                roomAvailabilities[roomAvailability.RoomNumber] = roomAvailability;
            }

            Console.WriteLine("Successfully parsed");
            return new HotelAvailability(HotelName.SUMMIT_PENTHOUSES, roomAvailabilities);
        }

        private static RoomAvailability ParseSingleRoomAvailability(Dictionary<string, dynamic> roomData)
        {
            string roomName = roomData["name"].ToString();

            Dictionary<DateTime, AvailabilityType> totalAvailability = new Dictionary<DateTime, AvailabilityType>();
            Dictionary<string, dynamic> availabilities = roomData["availability"].ToObject<Dictionary<string, dynamic>>();
            foreach (KeyValuePair<string, dynamic> availabilityForDate in availabilities)
            {
                string dateString = availabilityForDate.Key;
                DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                Dictionary<string, dynamic> availabilityInfoForDate = availabilityForDate.Value.ToObject<Dictionary<string, dynamic>>();
                Dictionary<string, dynamic> nightAvailabilityInfoForDate = availabilityInfoForDate["availability_night"].ToObject<Dictionary<string, dynamic>>();

                AvailabilityType availabilityType = GetAvailabilityTypeFromNightAvailabilityInfo(nightAvailabilityInfoForDate);

                totalAvailability[date] = availabilityType;
            }

            return new RoomAvailability(roomName, totalAvailability);
        }

        private static AvailabilityType GetAvailabilityTypeFromNightAvailabilityInfo(Dictionary<string, dynamic> nightAvailabilityInfo)
        {
            if (nightAvailabilityInfo.Count == 0)
            {
                return AvailabilityType.UNAVAILABLE;
            }
            else
            {
                foreach (KeyValuePair<string, dynamic> itemInfo in nightAvailabilityInfo)
                {
                    if (itemInfo.Value == "Yes")
                    {
                        return AvailabilityType.AVAILABLE;
                    }
                }
                return AvailabilityType.UNAVAILABLE;
            }
        }
    }
}
