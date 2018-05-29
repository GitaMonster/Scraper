using System;
using System.IO;
using System.Collections.Generic;
using Scraper.Model;
using Scraper.Util;

namespace Scraper
{
	class Program
	{
		static readonly string EXCEL_OUTPUT_PATH = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\resources\ExcelOutput\";
		static readonly bool AGGREGATE_ROOM_TYPES = false;
		static readonly DateTime START_DATE = new DateTime(2018, 2, 11);
		static readonly DateTime END_DATE = new DateTime(2018, 4, 7);
		static readonly List<HotelName> HOTEL_NAMES = new List<HotelName>{
			//HotelName.BIG_WHITE_STONEBRIDGE,
			HotelName.BIG_WHITE_BEARS_PAW,
			HotelName.BIG_WHITE_BLACK_BEAR
			/*HotelName.BIG_WHITE_BULLET_CREEK,
			HotelName.BIG_WHITE_CHATEAU_RIDGE,
			HotelName.BIG_WHITE_COPPER_KETTLE,
			HotelName.BIG_WHITE_EAGLES,
			HotelName.BIG_WHITE_GRIZZLY,
			HotelName.BIG_WHITE_PLAZA_RIDGE,
			HotelName.BIG_WHITE_PTARMIGAN,
			HotelName.BIG_WHITE_SNOWY_CREEK,
			HotelName.BIG_WHITE_STONEGATE,
			HotelName.BIG_WHITE_SUNDANCE,
			HotelName.BIG_WHITE_TOWERING_PINES,
			HotelName.BIG_WHITE_TRAPPERS_CROSSING,
			HotelName.BIG_WHITE_WHITEFOOT*/
		};

		public static void Main(string[] args)
		{
			// Run();
			SilverCreek.Run();
			Console.ReadKey();
		}

		private static async void Run()
		{
			ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.BIG_WHITE.Name + @"\");
			foreach (HotelName hotelName in HOTEL_NAMES)
			{
				HotelAvailability hotelAvailability = await BigWhite.GetAvailabilityForHotel(hotelName, START_DATE, END_DATE);
				if (AGGREGATE_ROOM_TYPES)
				{
					Dictionary<string, RoomAvailability> aggregatedAvailabilities = BigWhite.GetAggregatedAvailabilitiesForRoomType(hotelAvailability.RoomAvailabilities, START_DATE, END_DATE);
					hotelAvailability.RoomAvailabilities = aggregatedAvailabilities;
				}
				Console.WriteLine("About to enter excel writer");
				excelWriter.WriteHotelAvailability(hotelAvailability);
			}
		}
	}
}
