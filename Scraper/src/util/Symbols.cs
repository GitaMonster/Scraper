using System;
using Scraper.Model;

namespace Scraper
{
	namespace Util
	{
		class Symbols
		{
			public static string GetSymbolForAvailability(AvailabilityType availabilityType)
			{
				switch (availabilityType)
				{
					case AvailabilityType.AVAILABLE:
						return "Y";
					case AvailabilityType.UNAVAILABLE:
						return "X";
					case AvailabilityType.BLOCKED:
						return " ";
					default:
						throw new Exception("Error: Cannot set a symbol for an unset availability");
				}
			}
		}
	}
}
