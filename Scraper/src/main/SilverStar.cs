using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Scraper.Model;
using Scraper.Parser;

namespace Scraper
{
	class SilverStar
	{
		static readonly HttpClient httpClient = new HttpClient();

		public static async void Run()
		{
			DateTime startDate = new DateTime(2018, 1, 11);
			Dictionary<string, string> relevantHeaders = new Dictionary<string, string>();
			string initialPage = await GetInitialPageForHeadersAndAddCookie(relevantHeaders);
			string cookie = relevantHeaders["Cookie"];
			SilverStarParser.addRwidHeader(initialPage, relevantHeaders);
			string rwid = relevantHeaders["rwid"];
			string url = "https://www.reseze.net/servlet/WebresShowAvailable";
			string arrivalDateString = String.Format("&arrivalMonth={0}&arrivalDay={1}&arrivalYear={2}", startDate.Month, startDate.Day, startDate.Year);
			string postBody = "rwid=" + rwid + arrivalDateString + "&nightsStay=2&buildingCodeRoomType=any%7Cany&numberRooms=1&adults=1&children=0&age3=0&age4=0&rateCode=&groupId=&iataNumber=&ownerReservation=false&check=Check+Now";
			string html = await GetPage(url, postBody, cookie);
			Console.WriteLine(html);
		}

		public static async void AddAvailabilityForDate(ResortAvailability resortAvailability, DateTime startDate, Dictionary<string, string> relevantHeaders)
		{
			string cookie = relevantHeaders["Cookie"];
			string rwid = relevantHeaders["rwid"];
			string url = "https://www.reseze.net/servlet/WebresShowAvailable";
			string arrivalDateString = String.Format("&arrivalMonth=%d&arrivalDay=%d&arrivalYear=%d", startDate.Month, startDate.Day, startDate.Year);
			string postBody = "rwid=" + rwid + arrivalDateString + "&nightsStay=2&buildingCodeRoomType=any%7Cany&numberRooms=1&adults=1&children=0&age3=0&age4=0&rateCode=&groupId=&iataNumber=&ownerReservation=false&check=Check+Now";
			string html = await GetPage(url, postBody, cookie);

			//SilverStarParser.parseAvailabilityForDate(html, startDate, resortAvailability);
		}

		private static async Task<string> GetPage(string url, string postBody, string cookie)
		{
			HttpContent postContent = new StringContent(postBody);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = postContent;
			HttpResponseMessage response = await httpClient.PostAsync(url, postContent);
			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> GetInitialPageForHeadersAndAddCookie(Dictionary<string, string> relevantHeaders)
		{
			string url = "https://www.reseze.net/servlet/WebresResDesk?hotelid=1485&nightsStay=2";
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Add("Accept", "text/html");
			HttpResponseMessage response = await httpClient.SendAsync(request);
			// HttpResponseMessage response = await httpClient.GetAsync(url);
			IEnumerator<string> headerEnumerator = response.Headers.GetValues("Set-Cookie").GetEnumerator();
			headerEnumerator.MoveNext();
			relevantHeaders["Cookie"] = headerEnumerator.Current;
			
			return await response.Content.ReadAsStringAsync();
		}
	}
}
