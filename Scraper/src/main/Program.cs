using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper.Model;
using Scraper.Util;

namespace Scraper
{
	class Program
	{
		static readonly DateTime START_DATE = new DateTime(2017, 12, 8);
		static readonly DateTime END_DATE = new DateTime(2018, 4, 7);

		static void Main(string[] args)
		{
			// BigWhite.Run(START_DATE, END_DATE);
			// ExcelWriter.WriteHotelAvailability(new HotelAvailability(HotelName.BIG_WHITE_BULLET_CREEK));
			DateTime date = new DateTime(2017, 12, 10);
			DateTime date2 = new DateTime(2018, 4, 7);
			
			Console.WriteLine(date.ToShortDateString());
			Console.ReadKey();
		}

		private async static void Loop()
		{
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine("In loop: " + i);
				// DoThing().Wait();
				string x = await DoThing();
				Console.WriteLine(x);
			}
		}

		private async static Task<string> DoThing()
		{
			Task delay = Task.Delay(1000);
			Console.WriteLine("Doing thing");
			await delay;
			return "hi";
		}

	}
}
