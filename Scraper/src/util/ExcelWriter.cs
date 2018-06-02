using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using Scraper.Model;

namespace Scraper
{
	namespace Util
	{
		class ExcelWriter
		{
			private const int DATE_STARTING_COLUMN = 4;
			private string fileOutputPath;

			public ExcelWriter(string fileOutputPath)
			{
				this.fileOutputPath = fileOutputPath;
			}

			public void WriteHotelAvailability(HotelAvailability hotelAvailability)
			{
				Application application = new Application();
				if (application == null)
				{
					throw new Exception("Excel is not properly installed");
				}
				application.DisplayAlerts = false;
				DateTime startDate = hotelAvailability.GetEarliestKnownDate();
				DateTime endDate = hotelAvailability.GetLatestKnownDate();

				Workbook workbook = application.Workbooks.Add();
				Worksheet worksheet = (Worksheet)workbook.Worksheets.get_Item(1);
				worksheet.Name = hotelAvailability.Name.GetDisplayName();

				worksheet.Cells[1, 1] = hotelAvailability.Name.GetDisplayName();

				Console.WriteLine("About to add date labels");
				AddAllDateLabels(worksheet, startDate, endDate);

				Console.WriteLine("About to add availabilities");
				AddAllAvailabilities(worksheet, hotelAvailability, startDate, endDate);

				Directory.CreateDirectory(fileOutputPath);
				workbook.SaveAs(fileOutputPath + hotelAvailability.Name.GetDisplayName() + ".xls", XlFileFormat.xlWorkbookNormal, null, null, null, null, XlSaveAsAccessMode.xlShared);
				workbook.Close(true);
				application.Quit();

				Console.WriteLine("Excel file created");
			}

			private void AddAllAvailabilities(Worksheet worksheet, HotelAvailability hotelAvailability, DateTime startDate, DateTime endDate)
			{
				int currentRow = 3;
				foreach (KeyValuePair<string, RoomAvailability> entry in hotelAvailability.RoomAvailabilities)
				{
					worksheet.Cells[currentRow, 2] = entry.Key;
					int currentColumn = DATE_STARTING_COLUMN;
					Console.WriteLine("Writing excel availability for room " + entry.Key);
					foreach (DateTime date in DateUtils.GetOrderedDateRange(startDate, endDate))
					{
						string symbol = Symbols.GetSymbolForAvailability(entry.Value.TotalAvailability[date]);
						// Console.WriteLine("About to write cell for date " + DateUtils.GetReadableDateFormat(date));
						worksheet.Cells[currentRow, currentColumn] = symbol;
						//Console.WriteLine("About to format cell");
						//worksheet.Cells[currentRow, currentColumn].HorizontalAlignment = XlHAlign.xlHAlignCenter;
						//Console.WriteLine("Done room");
						currentColumn++;
					}
					currentRow++;
				}
			}

			private void AddAllDateLabels(Worksheet worksheet, DateTime startDate, DateTime endDate)
			{
				// First month
				int daysInStartMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
				int firstMonthCellWidth = daysInStartMonth - startDate.Day + 1;

				int firstMonthEndColumn = firstMonthCellWidth + 3;
				string firstMonthEndCell = ColumnIntToName(firstMonthEndColumn) + "1";

				Range firstMonthRange = worksheet.Range["D1", firstMonthEndCell];
				firstMonthRange.Merge();
				string firstMonthName = startDate.ToString("Y");
				firstMonthRange.Value = firstMonthName;

				AddDateLabelsInRange(worksheet, DATE_STARTING_COLUMN, startDate.Day, firstMonthCellWidth);

				DateTime firstMonth = new DateTime(startDate.Year, startDate.Month, 1);
				DateTime finalMonth = new DateTime(endDate.Year, endDate.Month, 1);
				DateTime newMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);

				// Middle months
				int startColumn = firstMonthEndColumn + 1;
				while (newMonth < finalMonth)
				{
					int monthCellWidth = DateTime.DaysInMonth(newMonth.Year, newMonth.Month);
					int endColumn = startColumn + monthCellWidth - 1;
					string startCell = ColumnIntToName(startColumn) + "1";
					string endCell = ColumnIntToName(endColumn) + "1";

					Range monthRange = worksheet.Range[startCell, endCell];
					monthRange.Merge();
					string monthName = newMonth.ToString("Y");
					monthRange.Value = monthName;

					AddDateLabelsInRange(worksheet, startColumn, 1, monthCellWidth);

					startColumn = endColumn + 1;
					newMonth = new DateTime(newMonth.Year, newMonth.Month, 1).AddMonths(1);
				}

				// Final month
                if (firstMonth.AddMonths(1) <= finalMonth)
				{
					int finalMonthCellWidth = endDate.Day;
					int finalMonthEndColumn = startColumn + finalMonthCellWidth - 1;
					string finalMonthStartCell = ColumnIntToName(startColumn) + "1";
					string finalMonthEndCell = ColumnIntToName(finalMonthEndColumn) + "1";

					Range finalMonthRange = worksheet.Range[finalMonthStartCell, finalMonthEndCell];
					finalMonthRange.Merge();
					string finalMonthName = newMonth.ToString("Y");
					finalMonthRange.Value = finalMonthName;

					AddDateLabelsInRange(worksheet, startColumn, 1, finalMonthCellWidth);
				}
			}

			private void AddDateLabelsInRange(Worksheet worksheet, int startColumn, int startValue, int range)
			{
				int currentColumn = startColumn;
				for (int i = startValue; i < (startValue + range); i++)
				{
					worksheet.Cells[2, currentColumn] = i.ToString();
					worksheet.Cells[2, currentColumn].HorizontalAlignment = XlHAlign.xlHAlignCenter;
					currentColumn++;
				}
			}

			private static string ColumnIntToName(int column)
			{
				string columnName = "";
				if (column > 26)
				{
					int prefixLetter = column / 26 + 64;
					if ((column % 26) == 0)
					{
						prefixLetter--;
					}
					columnName += (char)prefixLetter;
				}
				int suffixLetterValue = column % 26;
				if (suffixLetterValue == 0)
				{
					columnName += 'Z';
				}
				else
				{
					char suffixLetter = (char)((column % 26) + 64);
					columnName += suffixLetter;
				}
				return columnName;
			}
		}
	}
}
