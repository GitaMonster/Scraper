using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
	namespace Model
	{
		class HotelName
		{
			public static readonly HotelName BIG_WHITE_BEARS_PAW = new HotelName(ResortName.BIG_WHITE, "Bears Paw");
			public static readonly HotelName BIG_WHITE_BLACK_BEAR = new HotelName(ResortName.BIG_WHITE, "Black Bear Lodge");
			public static readonly HotelName BIG_WHITE_BULLET_CREEK = new HotelName(ResortName.BIG_WHITE, "Bullet Creek Cabins");

			public ResortName ResortName { get; }
			public string Name { get; }

			public HotelName(ResortName resortName, string name)
			{
				ResortName = resortName;
				Name = name;
			}

			public String GetDisplayName()
			{
				return ResortName.Name + " - " + Name;
			}
		}
	}
}
