using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using Scraper.Model;

namespace Scraper.parser
{
    class BanffBoundaryParser
    {
        public static HotelAvailability ParseHotelAvailability(string response)
        {
            Dictionary<string, RoomAvailability> roomAvailabilities = new Dictionary<string, RoomAvailability>();
            int startIndex = response.IndexOf('{');
            int endIndex = response.Length - 1;
            string json = response.Substring(startIndex, endIndex - startIndex);

            //System.IO.File.WriteAllText(@"C:\Users\Chloe\source\repos\TESTFORJSON\BanffBoundaryJson.txt", json);

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
            SortedList<DateTime, List<string>> totalIndividualUnitsAvailable = new SortedList<DateTime, List<string>>();  
            List<DateTime> dates = new List<DateTime>();

            foreach (KeyValuePair<string, dynamic> availabilityForDate in availabilities)
            {
                string dateString = availabilityForDate.Key;
                DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                dates.Add(date);

                //availabilityInfoForDate is the entire child of the date:
                Dictionary<string, dynamic> availabilityInfoForDate = availabilityForDate.Value.ToObject<Dictionary<string, dynamic>>();
                Dictionary<string, dynamic> nightAvailabilityInfoForDate = availabilityInfoForDate["night"].ToObject<Dictionary<string, dynamic>>();

                dynamic numberOfUnitsAvailable = nightAvailabilityInfoForDate["num_units_available"];

                //get the list of individual units available on a given night:
                List<string> individualUnits = GetIndividualUnitsAvailableFromNightAvailabilityInfo(numberOfUnitsAvailable, nightAvailabilityInfoForDate);

                //fill the sorted list with the list of individual units available:
                totalIndividualUnitsAvailable.Add(date, individualUnits); //add the key value pair
            }

            dates.Sort((x, y) => x.CompareTo(y));

            //corner case for first and last sorted list entries:
            List<string> firstDateAvailableUnits = totalIndividualUnitsAvailable.First().Value;

            //start as default unavailable:
            totalAvailability[dates[0]] = AvailabilityType.UNAVAILABLE;
            foreach (string s in firstDateAvailableUnits)
            {
                if (totalIndividualUnitsAvailable.Values[1].Contains(s))
                {
                    totalAvailability[dates[0]] = AvailabilityType.AVAILABLE;
                }
            }

            List<string> lastDateAvailableUnits = totalIndividualUnitsAvailable.Last().Value;
            //start as default unavailable:
            totalAvailability[dates[dates.Count - 1]] = AvailabilityType.UNAVAILABLE;
            foreach (string s in lastDateAvailableUnits)
            {
                if (totalIndividualUnitsAvailable.Values[totalIndividualUnitsAvailable.Count - 1].Contains(s))
                {
                    totalAvailability[dates[dates.Count - 1]] = AvailabilityType.AVAILABLE;
                }
            }

            //for all except the first and last date:
            for (int i = 1; i < totalIndividualUnitsAvailable.Count; i++)
            {
                DateTime currentDate = dates[i];

                //default is start as unavailable, until proven available:
                totalAvailability[currentDate] = AvailabilityType.UNAVAILABLE;
                foreach (string s in totalIndividualUnitsAvailable.Values[i])
                {
                    if (totalIndividualUnitsAvailable.Values[i - 1].Contains(s) || totalIndividualUnitsAvailable.Values[i + 1].Contains(s))
                    {
                        //then it's truly available; the individual unit is available for at least 2 consecutive nights
                        totalAvailability[currentDate] = AvailabilityType.AVAILABLE;
                    }
                }
            }

            return new RoomAvailability(roomName, totalAvailability);
        }

        //get list of individual room numbers (units) that are available for a given night availability info (night and room type)
        private static List<string> GetIndividualUnitsAvailableFromNightAvailabilityInfo(dynamic numberOfUnitsAvailable, Dictionary<string, dynamic> nightAvailabilityInfoForDate)
        {
            List<string> roomNumbers = new List<string>();  //list of unique room ids

            if (numberOfUnitsAvailable > 0)  //if inventory_ids even exists
            {

                Dictionary<string, dynamic> inventoryIds = nightAvailabilityInfoForDate["inventory_ids"].ToObject<Dictionary<string, dynamic>>();

                foreach (KeyValuePair<string, dynamic> inventory in inventoryIds)
                {
                    if (inventory.Value >= 1)  //just in case the site lists room types of which there are 0 inventory
                    {
                        //Console.WriteLine("key: " + inventory.Key + " , value: " + inventory.Value);
                        roomNumbers.Add(inventory.Key);
                    }
                }
            }

            return roomNumbers;
        }
    }
}
