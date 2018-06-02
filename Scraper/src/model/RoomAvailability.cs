using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
	namespace Model
	{
        class RoomAvailability
        {
            public string RoomNumber { get; }
            public Dictionary<DateTime, AvailabilityType> TotalAvailability { get; set; }

            public RoomAvailability(string roomNumber)
            {
                RoomNumber = roomNumber;
                TotalAvailability = new Dictionary<DateTime, AvailabilityType>();
            }

			public RoomAvailability(string roomNumber, Dictionary<DateTime, AvailabilityType> totalAvailability)
			{
				RoomNumber = roomNumber;
				TotalAvailability = totalAvailability;
			}

			public bool HasDataForDate(DateTime date)
			{
				return TotalAvailability.ContainsKey(date) && !TotalAvailability[date].Equals(AvailabilityType.NOT_SET);
			}

			public AvailabilityType GetAvailabilityForDate(DateTime date)
			{
				return TotalAvailability[date];
			}

			public bool IsAvailableOnDate(DateTime date)
			{
				return TotalAvailability[date].Equals(AvailabilityType.AVAILABLE);
			}

			// Note: once this is called, dates outside the given range will be permanently removed from this object
			public void TrimDateRange(DateTime earliestAllowedDate, DateTime latestAllowedDate)
			{
				foreach (DateTime key in new List<DateTime>(TotalAvailability.Keys))
				{
					if (key < earliestAllowedDate || key > latestAllowedDate)
					{
						TotalAvailability.Remove(key);
					}
				}
			}

			public DateTime GetEarliestKnownDate()
			{
				DateTime earliestDay = new DateTime(3000, 1, 1);
				foreach (DateTime day in TotalAvailability.Keys)
				{
					if (day < earliestDay)
					{
						earliestDay = day;
					}
				}
				return earliestDay;
			}

			public DateTime GetLatestKnownDate()
			{
				DateTime latestDay = new DateTime(1900, 1, 1);
				foreach (DateTime day in TotalAvailability.Keys)
				{
					if (day > latestDay)
					{
						latestDay = day;
					}
				}
				return latestDay;
			}

			public void MergeWith(RoomAvailability otherAvailability)
			{
				if (!otherAvailability.RoomNumber.Equals(RoomNumber))
				{
					throw new Exception("Error: Cannot merge availabilities for different rooms into one object");
				}
				foreach (KeyValuePair<DateTime, AvailabilityType> otherEntry in otherAvailability.TotalAvailability)
				{
					DateTime otherKey = otherEntry.Key;
					AvailabilityType otherValue = otherEntry.Value;
					if (TotalAvailability.ContainsKey(otherKey))
					{
						AvailabilityType existingValue = TotalAvailability[otherKey];
						if (otherValue.Equals(AvailabilityType.NOT_SET) ^ existingValue.Equals(AvailabilityType.NOT_SET))
						{
							throw new Exception("Error when merging room availabilities for date " +
									otherKey.ToLongDateString() + "; One value was set and the other was null");
						}
						else if (!otherValue.Equals(AvailabilityType.NOT_SET) && !existingValue.Equals(AvailabilityType.NOT_SET) && !otherValue.Equals(existingValue))
						{
							throw new Exception("Error when merging room availabilities for date " +
									otherKey.ToLongDateString() + "; One value was " + otherValue.ToString() + " and the other was " + existingValue.ToString());
						}
					}
					else
					{
						TotalAvailability.Add(otherKey, otherEntry.Value);
					}
				}
			}
		}
	}
}
