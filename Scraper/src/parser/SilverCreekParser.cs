using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

using Scraper.Model;
using Scraper.Util;

namespace Scraper.Parser
{
    class SilverCreekParser
    {
        static readonly int NUMBER_OF_DAYS_PER_REQUEST = 14;
        static readonly string SOLD = "Sold";
        static readonly string CLOSED = "Closed";

        public static void Parse(String page, HotelAvailability hotelAvailability, DateTime requestDate)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);

            HtmlNode rootDiv = htmlDocument.GetElementbyId("availTbl");
            HtmlNode table = rootDiv.ChildNodes.ElementAt(1);
            HtmlNode tHead = table.ChildNodes.ElementAt(1);
            HtmlNode datesTr = tHead.ChildNodes.ElementAt(1);

            List<DateTime> dates = GetDateRange(datesTr, requestDate);

            HtmlNode tBody = table.ChildNodes.ElementAt(3);
            for (int i=1; i<tBody.ChildNodes.Count; i+=2)
            {
                HtmlNode roomInfoTr = tBody.ChildNodes.ElementAt(i);

                RoomAvailability roomAvailability = GetRoomAvailability(roomInfoTr, dates);

                if (!hotelAvailability.RoomAvailabilities.ContainsKey(roomAvailability.RoomNumber))
                {
                    hotelAvailability.RoomAvailabilities.Add(roomAvailability.RoomNumber, roomAvailability);
                }
                else
                {
                    RoomAvailability existingRoomAvailability = hotelAvailability.RoomAvailabilities[roomAvailability.RoomNumber];
                    existingRoomAvailability.MergeWith(roomAvailability);
                }
            }
        }

        private static List<DateTime> GetDateRange(HtmlNode tr, DateTime requestDate)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime firstDateInRange = requestDate.AddDays(-6);
            dates.Add(firstDateInRange);
            DateTime currentDate = firstDateInRange.AddDays(1);

            for (int i = 1; i < NUMBER_OF_DAYS_PER_REQUEST; i++)
            {
                // Text content in an html element is considered to be a child node by the Html Agility Pack, so
                // we have to skip child elements two at a time
                int thIndex = 2 * i + 3;
                string thContent = tr.ChildNodes.ElementAt(thIndex).InnerHtml;
                string[] splitString = thContent.Split(new string[] { "<br>" }, StringSplitOptions.None);
                string[] dayMonth = splitString[1].Split('/');
                Validator.Assert(currentDate.Month.Equals(Int32.Parse(dayMonth[1])), "Error: the expected month in the sequence for index " + i +
                                                                                " did not match the actual month from the html table");
                Validator.Assert(currentDate.Day.Equals(Int32.Parse(dayMonth[0])), "Error: the expected day in the sequence for index " + i +
                                                                              " did not match the actual day from the html table");

                dates.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            Validator.Assert(dates.Count.Equals(NUMBER_OF_DAYS_PER_REQUEST), "The number of dates parsed out for the page was " + dates.Count +
                                                                        " instead of + " + NUMBER_OF_DAYS_PER_REQUEST);
            return dates;
        }

        private static RoomAvailability GetRoomAvailability(HtmlNode tr, List<DateTime> dates)
        {
            string roomDescription = tr.FirstChild.InnerText;
            HtmlNodeCollection trChildNodes = tr.ChildNodes;
            Dictionary<DateTime, AvailabilityType> availabilities = new Dictionary<DateTime, AvailabilityType>();

            for (int i=0; i<dates.Count; i++)
            {
                string availabilityContent = trChildNodes[i+1].InnerText;
                AvailabilityType availabilityType;
                if (availabilityContent.Equals(SOLD))
                {
                    availabilityType = AvailabilityType.UNAVAILABLE;
                }
                else if (availabilityContent.Equals(CLOSED))
                {
                    availabilityType = AvailabilityType.BLOCKED;
                }
                else if (availabilityContent.All(char.IsDigit))
                {
                    availabilityType = AvailabilityType.AVAILABLE;
                }
                else
                {
                    availabilityType = AvailabilityType.NOT_SET;
                }

                availabilities.Add(dates[i], availabilityType);
            }

            return new RoomAvailability(roomDescription, availabilities);
        }
    }
}
