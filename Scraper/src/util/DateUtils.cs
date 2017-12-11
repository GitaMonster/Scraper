using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
	namespace Util
	{
		class DateUtils
		{
			public static string GetMonthDayShortYearFormat(DateTime date)
			{
				int day = date.Day;
				int month = date.Month;
				int year = date.Year;
				string dayString = (day < 10) ? ("0" + day) : day.ToString();
				string monthString = (month < 10) ? ("0" + month) : month.ToString();
				string yearString = year.ToString().Substring(2, 2);
				return String.Format("{0}/{1}/{2}", monthString, dayString, yearString);
			}
		}
	}
}
