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
            // RunBigWhite();
            // RunSilverCreek();
            // RunBanffBoundary();
            SilverStarVanceCreek.Run();
            Console.ReadKey();
		}

        private static void RunBanffBoundary()
        {
            ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.BANFF_BOUNDARY.Name + @"\");
            ResortAvailability resortAvailability = BanffBoundary.GetResortAvailability(BanffBoundary.START_DATE, BanffBoundary.END_DATE);
            excelWriter.WriteHotelAvailability(resortAvailability.HotelAvailabilities[HotelName.BANFF_BOUNDARY]);
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, HotelName.BANFF_BOUNDARY);
        }

        private static void RunSilverCreek()
        {
            ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.SILVER_CREEK.Name + @"\");
            ResortAvailability resortAvailability = SilverCreek.GetResortAvailability(SilverCreek.START_DATE, SilverCreek.END_DATE);
            excelWriter.WriteHotelAvailability(resortAvailability.HotelAvailabilities[HotelName.SILVER_CREEK]);
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, HotelName.SILVER_CREEK);
        }

		private static async void RunBigWhite()
		{
			ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.BIG_WHITE.Name + @"\");
			foreach (HotelName hotelName in HOTEL_NAMES)
			{
				HotelAvailability hotelAvailability = await BigWhite.GetAvailabilityForHotel(hotelName, BigWhite.START_DATE, BigWhite.END_DATE);
				if (AGGREGATE_ROOM_TYPES)
				{
					Dictionary<string, RoomAvailability> aggregatedAvailabilities = BigWhite.GetAggregatedAvailabilitiesForRoomType(hotelAvailability.RoomAvailabilities, BigWhite.START_DATE, BigWhite.END_DATE);
					hotelAvailability.RoomAvailabilities = aggregatedAvailabilities;
				}
				Console.WriteLine("About to enter excel writer");
				excelWriter.WriteHotelAvailability(hotelAvailability);
			}
		}
	}
}