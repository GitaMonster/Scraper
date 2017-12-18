using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Scraper.Parser;
using Scraper.Model;
using Scraper.Util;

namespace Scraper
{
	class BigWhite
	{
		private const string ROOM_NUMBERS_KEY = "rooms";
		private const string PROPERTY_CODE_KEY = "propertyCode";
		private const string ROOM_NUMBER_CODE_KEY = "roomNumberCode";
		private const string RESORT_CODE_KEY = "resortCode";
		private const int SINGLE_REQUEST_MONTH_RANGE = 4;

		static readonly HttpClient httpClient = new HttpClient();

		public static async void Run(DateTime startDate, DateTime endDate)
		{
			HotelAvailability hotelAvailability = await GetAvailabilityForHotel(HotelName.BIG_WHITE_BULLET_CREEK, startDate, endDate);
			Console.WriteLine("Success");
		}

		public static async Task<HotelAvailability> GetAvailabilityForHotel(HotelName hotelName, DateTime startDate, DateTime endDate)
		{
			Dictionary<string, Object> roomsData = ReadRoomDataFromFile(hotelName);

			HotelAvailability hotelAvailability = new HotelAvailability(hotelName);
			List<DateTime> requestDates = CalculateRequestDates(startDate, endDate);

			foreach (DateTime requestDate in requestDates)
			{
				await AddAvailabilityAroundDate(hotelAvailability, requestDate, roomsData);
			}
			hotelAvailability.TrimDateRange(startDate, endDate);

			Console.WriteLine("\nEarliest available date for " + hotelName.Name + ": " + hotelAvailability.GetEarliestKnownDate().ToLongDateString());
			Console.WriteLine("Latest available date for " + hotelName.Name + ": " + hotelAvailability.GetLatestKnownDate().ToLongDateString() + "\n");
			return hotelAvailability;
		}

		public static async Task AddAvailabilityAroundDate(HotelAvailability hotelAvailability, DateTime requestDate, Dictionary<string, Object> roomsData)
		{
			Dictionary<string, RoomAvailability> roomAvailabilities = hotelAvailability.RoomAvailabilities;
			List<string> fullRoomNumbers = (List<string>) roomsData[ROOM_NUMBERS_KEY];

			string requestDateString = DateUtils.GetMonthDayShortYearFormat(requestDate);
			// TODO: Randomize request order for rooms
			foreach (string fullRoomNumber in fullRoomNumbers) {
				Console.WriteLine("Getting data for room: " + fullRoomNumber + " - " + hotelAvailability.Name.Name);

				string roomNumber = fullRoomNumber.Split(new []{'-'})[1].Trim();
				string resortCode = (string) roomsData[RESORT_CODE_KEY];
				string roomNumberCode = (string) roomsData[ROOM_NUMBER_CODE_KEY];
				string url;

				if (roomsData.ContainsKey(PROPERTY_CODE_KEY))
				{
					url = String.Format("http://irmestore.bigwhite.com/irmnet/res/RoomDetailsPage.aspx?Resort={0}&PropertyCode={1}&RoomNum={2}{3}&Arrival={4}",
							resortCode, roomsData[PROPERTY_CODE_KEY], roomNumberCode, roomNumber, requestDateString);
				}
				else
				{
					url = String.Format("http://irmestore.bigwhite.com/irmnet/res/RoomDetailsPage.aspx?Resort={0}&RoomNum={1}{2}&Arrival={3}",
							resortCode, roomNumberCode, roomNumber, requestDateString);
				}

				Task<string> page = GetPage(url);
				string pageText = await page;
				RoomAvailability roomAvailability = BigWhiteParser.ParseSingleRoomAvailability(pageText, roomNumber);
				if (!roomAvailabilities.ContainsKey(fullRoomNumber))
				{
					roomAvailabilities.Add(fullRoomNumber, roomAvailability);
				}
				else
				{
					roomAvailabilities[fullRoomNumber].MergeWith(roomAvailability);
				}
			}
		}

		// The fetcher; sends a request to the Big White servers for one page of availability; gets response in form of string containing entire webpage html
		private static async Task<string> GetPage(string urlString)
		{
			// TODO: Add cancellation option
			HttpResponseMessage response = await httpClient.GetAsync(urlString);

			return await response.Content.ReadAsStringAsync();
		}

		private static List<DateTime> CalculateRequestDates(DateTime startDate, DateTime endDate)
		{
			if (startDate > endDate)
			{
				throw new Exception("Error: the given start date is after the given end date");
			}
			List<DateTime> requestDates = new List<DateTime>();
			DateTime startMonth = new DateTime(startDate.Year, startDate.Month, 1);
			DateTime endMonth = new DateTime(endDate.Year, endDate.Month, 1);
			if (DateTime.Compare(startMonth, endMonth) == 0 || DateTime.Compare(startMonth.AddMonths(1), endMonth) == 0 || DateTime.Compare(startMonth.AddMonths(2), endMonth) == 0)
			{
				requestDates.Add(startDate);
			}
			else
			{
				DateTime currentYearMonth = startMonth.AddMonths(1);
				DateTime currentDate = new DateTime(currentYearMonth.Year, currentYearMonth.Month, startDate.Day);
				DateTime lowerDateLimit = new DateTime(startDate.Year, startDate.Month, startDate.Day);
				while (lowerDateLimit <= endDate)
				{
					requestDates.Add(currentDate);
					currentYearMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(SINGLE_REQUEST_MONTH_RANGE);
					currentDate = new DateTime(currentYearMonth.Year, currentYearMonth.Month, currentDate.Day);
					lowerDateLimit = new DateTime(currentYearMonth.AddMonths(-1).Year, currentYearMonth.AddMonths(-1).Month, 1);
				}
			}
			return requestDates;
		}

		private static Dictionary<string, Object> ReadRoomDataFromFile(HotelName hotelName)
		{
			string fileName = hotelName.Name + ".json";
			Dictionary<string, Object> roomsData = new Dictionary<string, Object>();
			string fileText;

			Assembly _assembly = Assembly.GetExecutingAssembly();
			using (var streamReader = new StreamReader(_assembly.GetManifestResourceStream("Scraper.resources.resortData.BigWhite." + fileName)))
			{
				fileText = streamReader.ReadToEnd();
			}
			Dictionary<string, dynamic> root = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(fileText);

			if (root.ContainsKey(PROPERTY_CODE_KEY))
			{
				roomsData.Add(PROPERTY_CODE_KEY, root[PROPERTY_CODE_KEY].ToString());
			}

			string roomNumberCode = root[ROOM_NUMBER_CODE_KEY];
			string resortCode = root[RESORT_CODE_KEY];

			roomsData.Add(ROOM_NUMBER_CODE_KEY, roomNumberCode);
			roomsData.Add(RESORT_CODE_KEY, resortCode);

			Dictionary<string, List<string>> roomNumberData = root[ROOM_NUMBERS_KEY].ToObject<Dictionary<string, List<string>>>();

			List<string> fullRoomNumbers = new List<string>();
			foreach (KeyValuePair<string, List<string>> property in roomNumberData)
			{
				foreach (string roomNumber in property.Value)
				{
					fullRoomNumbers.Add(property.Key + " - " + roomNumber);
				}
			}
			roomsData.Add(ROOM_NUMBERS_KEY, fullRoomNumbers);
			return roomsData;
		}
	}
}
