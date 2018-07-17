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
			public static readonly HotelName BIG_WHITE_CHATEAU_RIDGE = new HotelName(ResortName.BIG_WHITE, "Chateau on the Ridge");
			public static readonly HotelName BIG_WHITE_COPPER_KETTLE = new HotelName(ResortName.BIG_WHITE, "Copper Kettle Lodge");
			public static readonly HotelName BIG_WHITE_EAGLES = new HotelName(ResortName.BIG_WHITE, "Eagles Resort");
			public static readonly HotelName BIG_WHITE_GRIZZLY = new HotelName(ResortName.BIG_WHITE, "Grizzly Lodge");
			public static readonly HotelName BIG_WHITE_PLAZA_RIDGE = new HotelName(ResortName.BIG_WHITE, "Plaza on the Ridge");
			public static readonly HotelName BIG_WHITE_PTARMIGAN = new HotelName(ResortName.BIG_WHITE, "Ptarmigan Inn");
			public static readonly HotelName BIG_WHITE_SNOWY_CREEK = new HotelName(ResortName.BIG_WHITE, "Snowy Creek");
			public static readonly HotelName BIG_WHITE_STONEBRIDGE = new HotelName(ResortName.BIG_WHITE, "Stonebridge Lodge");
			public static readonly HotelName BIG_WHITE_STONEGATE = new HotelName(ResortName.BIG_WHITE, "Stonegate Resort");
			public static readonly HotelName BIG_WHITE_SUNDANCE = new HotelName(ResortName.BIG_WHITE, "Sundance Resort");
			public static readonly HotelName BIG_WHITE_TOWERING_PINES = new HotelName(ResortName.BIG_WHITE, "Towering Pines");
			public static readonly HotelName BIG_WHITE_TRAPPERS_CROSSING = new HotelName(ResortName.BIG_WHITE, "Trappers Crossing");
			public static readonly HotelName BIG_WHITE_WHITEFOOT = new HotelName(ResortName.BIG_WHITE, "Whitefoot Lodge");

            public static readonly HotelName SILVER_CREEK = new HotelName(ResortName.SILVER_CREEK, "Silver Creek");

            public static readonly HotelName BANFF_BOUNDARY = new HotelName(ResortName.BANFF_BOUNDARY, "Banff Boundary");

            public static readonly HotelName FIRE_MOUNTAIN = new HotelName(ResortName.FIRE_MOUNTAIN, "Fire Mountain");

            public static readonly HotelName MYSTIC_SPRINGS = new HotelName(ResortName.MYSTIC_SPRINGS, "Mystic Springs");

            public static readonly HotelName SUMMIT_PENTHOUSES = new HotelName(ResortName.SUMMIT_PENTHOUSES, "Summit Penthouses");

            public ResortName ResortName { get; }
			public string Name { get; }

			public HotelName(ResortName resortName, string name)
			{
				ResortName = resortName;
				Name = name;
			}

			public String GetDisplayName()
			{
                if (ResortName.Name == Name)
                {
                    return ResortName.Name;  //prevents redundant duplicate resort/hotel names for single-hotel resorts
                }
                else
                {
                    return ResortName.Name + "-" + Name;
                }
				
			}
		}
	}
}
