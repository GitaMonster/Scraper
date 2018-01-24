using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

// Add the following using directive, and add a reference for System.Net.Http. 
using System.Net.Http;

namespace Scraper
{
	class SuttonPlace
	{
		public static async void Run()
		{
			await CreateMultipleTasksAsync();
			string result = "\n\nFinished";
			Console.WriteLine(result);
		}

		private static async Task CreateMultipleTasksAsync()
		{
			// Declare an HttpClient object, and increase the buffer size. The 
			// default buffer size is 65,536.
			HttpClient client =
				new HttpClient() { MaxResponseContentBufferSize = 1000000 };

			// Create and start the tasks. As each task finishes, DisplayResults  
			// displays its length.
			Task<int> download1 = ProcessURLAsync("http://msdn.microsoft.com", client);
			Task<int> download2 = ProcessURLAsync("http://msdn.microsoft.com/en-us/library/hh156528(VS.110).aspx", client);
			Task<int> download3 = ProcessURLAsync("http://msdn.microsoft.com/en-us/library/67w7t67f.aspx", client);
			Task<int> download4 = ProcessURLAsync("http://msdn.microsoft.com", client);
			Task<int> download5 = ProcessURLAsync("http://msdn.microsoft.com/en-us/library/hh156528(VS.110).aspx", client);
			Task<int> download6 = ProcessURLAsync("http://msdn.microsoft.com/en-us/library/67w7t67f.aspx", client);

			// Await each task.
			int length1 = await download1;
			int length2 = await download2;
			int length3 = await download3;
			int length4 = await download4;
			int length5 = await download5;
			int length6 = await download6;

			int total = length1 + length2 + length3 + length4 + length5 + length6;

			// Display the total count for the downloaded websites.
			string result = string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
			Console.WriteLine(result);
		}

		private static async Task<int> ProcessURLAsync(string url, HttpClient client)
		{
			var byteArray = await client.GetByteArrayAsync(url);
			DisplayResults(url, byteArray);
			return byteArray.Length;
		}

		private static void DisplayResults(string url, byte[] content)
		{ 
			var bytes = content.Length;
			var displayURL = url.Replace("http://", "");
			string result = string.Format("\n{0,-58} {1,8}", displayURL, bytes);
			Console.WriteLine(result);
		}
	}
}