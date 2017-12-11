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

			public string Name { get; }

			public ResortName(string name)
			{
				Name = name;
			}
		}
	}
}
