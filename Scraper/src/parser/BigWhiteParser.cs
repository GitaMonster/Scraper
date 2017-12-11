using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper.Model;

namespace Scraper
{
	namespace Parser
	{
		class BigWhiteParser
		{
			private const string YEAR_STARTER = "20";
			private const int CSV_DATE_LENGTH = 10;
			private const string VACANT_IDENTIFIER = "CalendarBackgroundVacant";
			private const string OCCUPIED_IDENTIFIER = "CalendarBackgroundOccupied";
			private const string BLOCKED_IDENTIFIER = "CalendarBackgroundBlocked";

			public static RoomAvailability ParseSingleRoomAvailability(string pageText, string roomNumber)
			{
				Dictionary<DateTime, AvailabilityType> availability = new Dictionary<DateTime, AvailabilityType>();

				AddDates(pageText, availability, VACANT_IDENTIFIER, AvailabilityType.AVAILABLE);
				AddDates(pageText, availability, OCCUPIED_IDENTIFIER, AvailabilityType.UNAVAILABLE);
				AddDates(pageText, availability, BLOCKED_IDENTIFIER, AvailabilityType.BLOCKED);
				return new RoomAvailability(roomNumber, availability);
			}

			private static void AddDates(string pageText, Dictionary<DateTime, AvailabilityType> availability, string identifier, AvailabilityType availabilityType)
			{
				char[] splitChar = new[] {','};
				int currentIndex = pageText.IndexOf(identifier);
				while (currentIndex != -1)
				{
					string dateData = pageText.Substring(currentIndex - CSV_DATE_LENGTH - 2, 10);
					// Three possible lengths of csv date, depending on whether day or month values are single or double digit
					if (!dateData.StartsWith(YEAR_STARTER))
					{
						dateData = pageText.Substring(currentIndex - CSV_DATE_LENGTH - 1, 9);
					}
					if (!dateData.StartsWith(YEAR_STARTER))
					{
						dateData = pageText.Substring(currentIndex - CSV_DATE_LENGTH, 8);
					}
					if (!dateData.StartsWith(YEAR_STARTER))
					{
						currentIndex = pageText.IndexOf(identifier, currentIndex + 1);
						continue;
					}
					string[] date = dateData.Split(splitChar);
					int year = Int32.Parse(date[0]);
					int month = Int32.Parse(date[1]);
					int day = Int32.Parse(date[2]);

					currentIndex = pageText.IndexOf(identifier, currentIndex + 1);

					DateTime specificDate = new DateTime(year, month, day);
					availability.Add(specificDate, availabilityType);
				}
			}
		}
	}
}
