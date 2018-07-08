using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
	namespace Model
	{
		class ResortName
		{
			public static readonly ResortName BIG_WHITE = new ResortName("Big White");
            public static readonly ResortName SILVER_CREEK = new ResortName("Silver Creek");
            public static readonly ResortName BANFF_BOUNDARY = new ResortName("Banff Boundary");
            public static readonly ResortName FIRE_MOUNTAIN = new ResortName("Fire Mountain");

			public string Name { get; }

			public ResortName(string name)
			{
				Name = name;
			}
		}
	}
}
