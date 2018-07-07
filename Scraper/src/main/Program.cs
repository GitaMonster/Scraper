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

		public static void Main(string[] args)
		{
            RunBanffBoundary();
            RunSilverCreek();
            RunBigWhite();
            // SilverStarVanceCreek.Run();
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
			foreach (HotelName hotelName in BigWhite.HOTEL_NAMES)
			{
				HotelAvailability hotelAvailability = await BigWhite.GetAvailabilityForHotel(hotelName, BigWhite.START_DATE, BigWhite.END_DATE);
				if (AGGREGATE_ROOM_TYPES)
				{
					Dictionary<string, RoomAvailability> aggregatedAvailabilities = BigWhite.GetAggregatedAvailabilitiesForRoomType(hotelAvailability.RoomAvailabilities, BigWhite.START_DATE, BigWhite.END_DATE);
					hotelAvailability.RoomAvailabilities = aggregatedAvailabilities;
				}
				excelWriter.WriteHotelAvailability(hotelAvailability);
			}
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, BigWhite.HOTEL_NAMES);
		}
	}
}