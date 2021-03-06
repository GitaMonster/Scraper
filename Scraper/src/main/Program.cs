﻿using System;
using System.IO;
using System.Collections.Generic;
using Scraper.Model;
using Scraper.Util;

namespace Scraper
{
    // TODO: Is it necessary to override hashCode and equals for C# models?
	class Program
	{
		static readonly string EXCEL_OUTPUT_PATH = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\resources\ExcelOutput\";
		static readonly bool AGGREGATE_ROOM_TYPES = false;

		public static void Main(string[] args)
		{
            //RunBanffBoundary();
            //RunFireMountain();
            //RunMysticSprings();
            //MysticSprings.Run();  //for testing only
            //RunSilverCreek();    //website is down
            RunSummitPenthouses();

            // RunBigWhite();
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

        private static void RunFireMountain()
        {
            ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.FIRE_MOUNTAIN.Name + @"\");
            ResortAvailability resortAvailability = FireMountain.GetResortAvailability(FireMountain.START_DATE, FireMountain.END_DATE);   //;
            excelWriter.WriteHotelAvailability(resortAvailability.HotelAvailabilities[HotelName.FIRE_MOUNTAIN]);
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, HotelName.FIRE_MOUNTAIN);
        }

        private static void RunMysticSprings()
        {
            ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.MYSTIC_SPRINGS.Name + @"\");
            ResortAvailability resortAvailability = MysticSprings.GetResortAvailability(MysticSprings.START_DATE, MysticSprings.END_DATE);  
            excelWriter.WriteHotelAvailability(resortAvailability.HotelAvailabilities[HotelName.MYSTIC_SPRINGS]);
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, HotelName.MYSTIC_SPRINGS);
        }

        private static void RunSummitPenthouses()
        {
            ExcelWriter excelWriter = new ExcelWriter(EXCEL_OUTPUT_PATH + ResortName.SUMMIT_PENTHOUSES.Name + @"\");
            ResortAvailability resortAvailability = SummitPenthouses.GetResortAvailability(SummitPenthouses.START_DATE, SummitPenthouses.END_DATE); 
            excelWriter.WriteHotelAvailability(resortAvailability.HotelAvailabilities[HotelName.SUMMIT_PENTHOUSES]);
            EmailSender.SendEmail(EXCEL_OUTPUT_PATH, HotelName.SUMMIT_PENTHOUSES);
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