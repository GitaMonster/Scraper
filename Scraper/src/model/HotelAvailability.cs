using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
	namespace Model
	{
		class HotelAvailability
		{
			public HotelName Name { get; }
			public Dictionary<string, RoomAvailability> RoomAvailabilities { get; set; }

			public HotelAvailability(HotelName name)
			{
				Name = name;
				RoomAvailabilities = new Dictionary<string, RoomAvailability>();
			}
		
			public HotelAvailability(HotelName name, Dictionary<string, RoomAvailability> roomAvailabilities)
			{
				Name = name;
				RoomAvailabilities = roomAvailabilities;
			}

			// Note: once this is called, dates outside the given range will be permanently removed from this object
			public void TrimDateRange(DateTime earliestAllowedDate, DateTime latestAllowedDate)
			{
				foreach (RoomAvailability roomAvailability in RoomAvailabilities.Values)
				{
					roomAvailability.TrimDateRange(earliestAllowedDate, latestAllowedDate);
				}
			}

			public DateTime GetEarliestKnownDate()
			{
				DateTime earliestKnownDate = new DateTime(3000, 1, 1);

				foreach (RoomAvailability roomAvailability in RoomAvailabilities.Values)
				{
					DateTime earliestDateForRoom = roomAvailability.GetEarliestKnownDate();
					if (earliestDateForRoom < earliestKnownDate)
					{
						earliestKnownDate = earliestDateForRoom;
					}
				}
				return earliestKnownDate;
			}

			public DateTime GetLatestKnownDate()
			{
				DateTime latestKnownDate = new DateTime(1900, 1, 1);

				foreach (RoomAvailability roomAvailability in RoomAvailabilities.Values)
				{
					DateTime latestDateForRoom = roomAvailability.GetLatestKnownDate();
					if (latestDateForRoom > latestKnownDate)
					{
						latestKnownDate = latestDateForRoom;
					}
				}
				return latestKnownDate;
			}

			public List<string> GetRoomNumbersAvailableOnDate(DateTime date)
			{
				List<string> availableRoomNumbers = new List<string>();
				foreach (RoomAvailability roomAvailability in RoomAvailabilities.Values)
				{
					if (roomAvailability.HasDataForDate(date) && roomAvailability.IsAvailableOnDate(date))
					{
						availableRoomNumbers.Add(roomAvailability.RoomNumber);
					}
				}
				return availableRoomNumbers;
			}

            public void MergeWith(HotelAvailability otherAvailabililty)
            {
                foreach (KeyValuePair<string, RoomAvailability> roomAvailabilityInfo in otherAvailabililty.RoomAvailabilities)
                {
                    RoomAvailability correspondingRoomAvailability = RoomAvailabilities[roomAvailabilityInfo.Key];
                    roomAvailabilityInfo.Value.MergeWith(correspondingRoomAvailability);
                }
            }
		}
	}
}
