using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Scraper.Model;

namespace Scraper.Parser
{
    class BanffBoundaryParser
    {
        public static HotelAvailability ParseHotelAvailability(string response)
        {
            Dictionary<string, RoomAvailability> roomAvailabilities = new Dictionary<string, RoomAvailability>();
            int startIndex = response.IndexOf('{');
            int endIndex = response.Length - 1;
            string json = response.Substring(startIndex, endIndex - startIndex);

            Dictionary<string, dynamic> root = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            List<dynamic> roomAvailabilityData = root["room_types"].ToObject<List<dynamic>>();

            foreach(dynamic room in roomAvailabilityData)
            {
                RoomAvailability roomAvailability = ParseSingleRoomAvailability(room.ToObject<Dictionary<string, dynamic>>());
                roomAvailabilities[roomAvailability.RoomNumber] = roomAvailability;
            }

            Console.WriteLine("Successfully parsed");
            return new HotelAvailability(HotelName.BANFF_BOUNDARY, roomAvailabilities);
        }

        private static RoomAvailability ParseSingleRoomAvailability(Dictionary<string, dynamic> roomData)
        {
            string roomName = roomData["name"].ToString();

            Dictionary<DateTime, AvailabilityType> totalAvailability = new Dictionary<DateTime, AvailabilityType>();
            Dictionary<string, dynamic> availabilities = roomData["availability"].ToObject<Dictionary<string, dynamic>>();
            foreach(KeyValuePair<string, dynamic> availabilityForDate in availabilities)
            {
                string dateString = availabilityForDate.Key;
                DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                Dictionary<string, dynamic> availabilityInfoForDate = availabilityForDate.Value.ToObject<Dictionary<string, dynamic>>();
                Dictionary<string, dynamic> nightAvailabilityInfoForDate = availabilityInfoForDate["night"].ToObject<Dictionary<string, dynamic>>();
                dynamic numberOfUnitsAvailable = nightAvailabilityInfoForDate["num_units_available"];

                AvailabilityType availabilityType = GetAvailabilityTypeFromNumberOfUnitsAvailable(numberOfUnitsAvailable);

                totalAvailability[date] = availabilityType;
            }

            return new RoomAvailability(roomName, totalAvailability);
        }

        private static AvailabilityType GetAvailabilityTypeFromNumberOfUnitsAvailable(dynamic numberOfUnitsAvailable)
        {
            if (numberOfUnitsAvailable == null)
            {
                return AvailabilityType.UNAVAILABLE;
            }
            long convertedNumberOfUnitsAvailable = (long) numberOfUnitsAvailable;
            return convertedNumberOfUnitsAvailable > 0 ? AvailabilityType.AVAILABLE : AvailabilityType.UNAVAILABLE;
        }
    }
}
