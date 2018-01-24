using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper.Model;

namespace Scraper
{
	namespace Model
	{
		class ResortAvailability
		{
			public ResortName Name { get; }
			public Dictionary<HotelName, HotelAvailability> HotelAvailabilities { get; }
		}
	}
}
