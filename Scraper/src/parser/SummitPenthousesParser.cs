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
            //Room IDs:
            //One bedroom Condo:
            //inventory_id_111140, inventory_id_104610, inventory_id_104719, inventory_id_121005, inventory_id_104718, inventory_id_113921

            //Two bedroom condo:
            //inventory_id_113926

            //Grotto Suites - 2 BDR + Den:
            //inventory_id_73032, inventory_id_73030

            //	Grotto Plus - 3 BDR :
            //inventory_id_87228, inventory_id_87267

            Dictionary<string, RoomAvailability> roomAvailabilities = new Dictionary<string, RoomAvailability>();
            int startIndex = response.IndexOf('{');
            int endIndex = response.Length - 1;
            string json = response.Substring(startIndex, endIndex - startIndex);

            //System.IO.File.WriteAllText(@"C:\Users\Chloe\source\repos\TESTFORJSON\SummitJson.txt", json);
      
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
            SortedList<DateTime, List<string>> totalIndividualUnitsAvailable = new SortedList<DateTime, List<string>>();  //ADDED. first fill this sorted list

            List<DateTime> dates = new List<DateTime>();  //this is to hold dates; added

            Dictionary<string, dynamic> availabilities = roomData["availability"].ToObject<Dictionary<string, dynamic>>();
            foreach (KeyValuePair<string, dynamic> availabilityForDate in availabilities)
            {
                string dateString = availabilityForDate.Key;
                DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                dates.Add(date);
                Dictionary<string, dynamic> availabilityInfoForDate = availabilityForDate.Value.ToObject<Dictionary<string, dynamic>>();
                Dictionary<string, dynamic> nightAvailabilityInfoForDate = availabilityInfoForDate["availability_night"].ToObject<Dictionary<string, dynamic>>();

                //get the list of individual units available on a given night:
                List<string> individualUnits = GetIndividualUnitsAvailableFromNightAvailabilityInfo(nightAvailabilityInfoForDate);

                //fill the sorted list with the list of individual units available:
                totalIndividualUnitsAvailable.Add(date, individualUnits); //add the key value pair

            }

            dates.Sort((x, y) => x.CompareTo(y));
           
            //at this point we have a sorted list of dates and lists of units available, sorted by date
            //also have a list of dates which was created in the above parse

            //corner case for first and last sorted list entries:
            List<string> firstDateAvailableUnits = totalIndividualUnitsAvailable.First().Value;

            //TESTING:
            //foreach (string s in firstDateAvailableUnits)
            //{
            //    Console.WriteLine("first night avail units: " + s);  //this isnt getting printed
            //}
            
            //Console.ReadLine();

            //start as default unavailable:
            totalAvailability[dates[0]] = AvailabilityType.UNAVAILABLE;
            foreach (string s in firstDateAvailableUnits)
            {
                if (totalIndividualUnitsAvailable.Values[1].Contains(s)) //make sure sorted list index starts at 0
                {
                    totalAvailability[dates[0]] = AvailabilityType.AVAILABLE;
                }
            }

            //FOR TESTING:
            //foreach (string s in firstDateAvailableUnits)
            //{
            //    Console.WriteLine(s);
            //}
          
            //Console.WriteLine(totalIndividualUnitsAvailable.Values[1]);
            //Console.ReadLine();

            List<string> lastDateAvailableUnits = totalIndividualUnitsAvailable.Last().Value;
            //start as default unavailable:
            totalAvailability[dates[dates.Count - 1]] = AvailabilityType.UNAVAILABLE;
            foreach (string s in lastDateAvailableUnits)
            {
                if (totalIndividualUnitsAvailable.Values[totalIndividualUnitsAvailable.Count - 1].Contains(s)) //make sure sorted list index starts at 0
                {
                    totalAvailability[dates[dates.Count - 1]] = AvailabilityType.AVAILABLE;
                }
            }

            //for all except the first and last date:
            for (int i = 1; i < totalIndividualUnitsAvailable.Count; i++)  //make sure index of sorted list starts at zero
            {
                DateTime currentDate = dates[i];

                //default is start as unavailable, until proven available:
                totalAvailability[currentDate] = AvailabilityType.UNAVAILABLE;
                foreach (string s in totalIndividualUnitsAvailable.Values[i])
                {
                    if (totalIndividualUnitsAvailable.Values[i-1].Contains(s) || totalIndividualUnitsAvailable.Values[i + 1].Contains(s))
                    {
                        //then it's truly available; the individual unit is available for at least 2 consecutive nights
                        totalAvailability[currentDate] = AvailabilityType.AVAILABLE;
                    }
                }
            }
            return new RoomAvailability(roomName, totalAvailability);
        }
              

        //get list of individual room numbers (units) that are available for a given night availability info (night and room type)
        private static List<string> GetIndividualUnitsAvailableFromNightAvailabilityInfo(Dictionary<string, dynamic> nightAvailabilityInfo)
        {
            List<string> roomNumbers = new List<string>();
            foreach (KeyValuePair<string, dynamic> itemInfo in nightAvailabilityInfo)
            {
                if (itemInfo.Value == "Yes")
                {
                    roomNumbers.Add(itemInfo.Key);
                }
            }
            return roomNumbers;
        }

    }
}
