using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

using Scraper.Parser;

namespace Scraper
{
    class SilverStarVanceCreek
    {
        private static readonly int URL_CODE_LENGTH = 24;

        private static readonly HttpClientHandler httpClientHandler = new HttpClientHandler();
        private static HttpClient httpClient;

        public static void Run()
        {
            SetUpHttpClient();
            string urlCode = GetUrlCodeFromInitialRequest();
            Console.WriteLine("urlCode = " + urlCode);
            string mainPage = GetEventValidationFromMainPageLoad(urlCode);
            Console.WriteLine("Contains __EVENTVALIDATION ? " + mainPage.Contains("__EVENTVALIDATION"));
            string eventValidationValue = SilverStarVanceCreekParser.GetEventValidationValue(mainPage);
            Console.WriteLine("Event validation = " + eventValidationValue);
            // string page = GetMainSearchResultsPage(urlCode, eventValidationValue);
            GetMainSearchResultsPage(urlCode, eventValidationValue);
            Thread.Sleep(4000);
            string page = GetCalendarPage(urlCode, eventValidationValue);
            Console.WriteLine("Page: " + page);
            Console.WriteLine("\n");
            Console.WriteLine("Contains Chilcoot? " + page.Contains("Chilcoot"));
            Console.WriteLine("Contains 234? " + page.Contains("234"));
            Console.WriteLine("Contains CalendarBackgroundOccupied ? " + page.Contains("CalendarBackgroundOccupied"));
        }

        private static void SetUpHttpClient()
        {
            httpClientHandler.AllowAutoRedirect = false;
            httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Host", "www.book.vancecreekhotel.com");
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.vancecreekhotel.com/");
            httpClient.DefaultRequestHeaders.Add("Upgrade-insecure-requests", "1");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
        }

        private static string GetUrlCodeFromInitialRequest()
        {
            HttpResponseMessage response = httpClient.GetAsync("https://www.book.vancecreekhotel.com/irmnet/(S(mmky5wnvtaughy0o1mwyi3b2))/Res/ResMain.aspx").Result;
            string location = response.Headers.GetValues("Location").ToList()[0];
            int startIndex = location.IndexOf('S') + 2;
            return location.Substring(startIndex, URL_CODE_LENGTH);
        }

        private static string GetEventValidationFromMainPageLoad(string urlCode)
        {
            HttpResponseMessage response = httpClient.GetAsync("https://www.book.vancecreekhotel.com/irmnet/(S(" + urlCode + "))/Res/ResMain.aspx").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private static string GetMainSearchResultsPage(string urlCode, string eventValidationValue)
        {
            httpClient.DefaultRequestHeaders.Clear();

            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Cache-control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Host", "www.book.vancecreekhotel.com");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://www.book.vancecreekhotel.com");
            // httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(jrhztxc1nqvqcvlnfmorxg0u))/Res/ResMain.aspx");
            // httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(mmky5wnvtaughy0o1mwyi3b2))/Res/ResMain.aspx");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(" + urlCode + "))/Res/ResMain.aspx");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("X-MicrosoftAjax", "Delta=true");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            // string url = "https://www.book.vancecreekhotel.com/irmnet/(S(jrhztxc1nqvqcvlnfmorxg0u))/Res/ResMain.aspx";
            // string url = "https://www.book.vancecreekhotel.com/irmnet/(S(mmky5wnvtaughy0o1mwyi3b2))/Res/ResMain.aspx";
            string url = "https://www.book.vancecreekhotel.com/irmnet/(S(" + urlCode + "))/Res/ResMain.aspx";

            HttpContent postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ScriptManager1", "UpdatePanelSearchCriteria|btnCheckAvail"),
                // new KeyValuePair<string, string>("ScriptManager1", "UpdatePanelAvailability|dlRoomTypes$ctl01$lbtnViewCalendar"),
                // new KeyValuePair<string, string>("__EVENTTARGET", "dlRoomTypes$ctl01$lbtnViewCalendar"),
                new KeyValuePair<string, string>("__EVENTTARGET", ""),
                new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                new KeyValuePair<string, string>("__LASTFOCUS", ""),
                new KeyValuePair<string, string>("__VIEWSTATE", ""),
                new KeyValuePair<string, string>("__EVENTVALIDATION", eventValidationValue),
                new KeyValuePair<string, string>("JavaScriptEnabled", ""),
                new KeyValuePair<string, string>("hdnClickSearchProgrammatically", "btnCheckAvail"),
                new KeyValuePair<string, string>("hdnImageViewerHeight", "75px"),
                new KeyValuePair<string, string>("hdnSelectedRateInfo", ""),
                // TODO: Find out what exactly the other numbers in the WebMonthCalendar values are
                new KeyValuePair<string, string>("wdpArrival_clientState", ""),
                new KeyValuePair<string, string>("WebMonthCalendar1_clientState", ""),
                //new KeyValuePair<string, string>("wdpArrival_clientState", "|0|012018-6-28-0-0-0-0||[[[[]],[],[]],[{},[]],\"012018-6-28-0-0-0-0\"]"),
                // new KeyValuePair<string, string>("WebMonthCalendar1_clientState", "[[[[]],[],[]],[{},[]],\"01,2018,06,2018,6,28\"]"),
                new KeyValuePair<string, string>("wcNights", "1"),
                new KeyValuePair<string, string>("wdpDeparture_clientState", ""),
                new KeyValuePair<string, string>("WebMonthCalendar2_clientState", ""),
                // new KeyValuePair<string, string>("wdpDeparture_clientState", "|0|012018-6-28-0-0-0-0||[[[[]],[],[]],[{},[]],\"012018-6-28-0-0-0-0\"]"),
                // new KeyValuePair<string, string>("WebMonthCalendar2_clientState", "[[[[]],[],[]],[{},[]],\"01,2018,06,2018,6,28\"]"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl1Short", "1"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl2Short", "0"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl3Short", "0"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl4Short", "0"),
                new KeyValuePair<string, string>("wcPropertyCode", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcRDPReq2", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcRDPReq3", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcIRMReq2", "*"),
                new KeyValuePair<string, string>("txtPromoCode", ""),
                /*new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomType", "CREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomNum", "CC231"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnDescription", "Chilcoot Hotel Room"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomDetailURL", ""),*/
                new KeyValuePair<string, string>("wdwDialog_clientState", ""),
                // new KeyValuePair<string, string>("wdwDialog_clientState", "[[[[null,3,null,null,null,null,0,null,1,0,null,3]],[[[[[null,null,null]],[[[[[]],[],null],[null,null],[null]],[[[[]],[],null],[null,null],[null]],[[[[]],[],null],[null,null],[null]]],[]],[{},[]],null],[[[[null,null,null,null]],[],[]],[{},[]],null]],[]],[{},[]],\"3,3,,,,,0\"]"),
                new KeyValuePair<string, string>("hdnPropertyName", ""),
                new KeyValuePair<string, string>("_IG_CSS_LINKS_", "~/App_Themes/Default/1CommonStyles.css|~/App_Themes/Default/Calendar.css|~/App_Themes/Default/Grids.css|~/App_Themes/Default/haRoomDetails.css|~/App_Themes/Default/ig_styles.css|~/App_Themes/Default/MonitorStyles.css|~/App_Themes/Default/OwnerStyles.css|~/App_Themes/Default/ResMain.css|~/App_Themes/Default/ResStyles.css|~/App_Themes/Default/RoomsCal.css|~/App_Themes/Default/RoundedBoxes.css|~/App_Themes/Default/Survey.css|~/App_Themes/Default/v4Buttons.css|~/App_Themes/Default/v4ig_styles.css|~/App_Themes/Default/v4OwnerStyles.css|~/App_Themes/Default/v4ResStyles.css|../ig_res/Default/ig_dialogwindow.css|../ig_res/Default/ig_monthcalendar.css|../ig_res/Default/ig_texteditor.css|../ig_res/Default/ig_shared.css"),
                new KeyValuePair<string, string>("__ASYNCPOST", "true"),
                // This property affects the outcome
                new KeyValuePair<string, string>("btnCheckAvail", "Check Availability")
            });
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = postContent;

            HttpResponseMessage response = httpClient.SendAsync(request).Result;
            Console.WriteLine("\nResponse code = " + response.StatusCode);
            return response.Content.ReadAsStringAsync().Result;
        }

        private static string GetCalendarPage(string urlCode, string eventValidationValue)
        {
            httpClient.DefaultRequestHeaders.Clear();

            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Cache-control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Host", "www.book.vancecreekhotel.com");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://www.book.vancecreekhotel.com");
            // httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(jrhztxc1nqvqcvlnfmorxg0u))/Res/ResMain.aspx");
            // httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(mmky5wnvtaughy0o1mwyi3b2))/Res/ResMain.aspx");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://www.book.vancecreekhotel.com/irmnet/(S(" + urlCode + "))/Res/ResMain.aspx");
            httpClient.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("X-MicrosoftAjax", "Delta=true");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            // string url = "https://www.book.vancecreekhotel.com/irmnet/(S(jrhztxc1nqvqcvlnfmorxg0u))/Res/ResMain.aspx";
            // string url = "https://www.book.vancecreekhotel.com/irmnet/(S(mmky5wnvtaughy0o1mwyi3b2))/Res/ResMain.aspx";
            string url = "https://www.book.vancecreekhotel.com/irmnet/(S(" + urlCode + "))/Res/ResMain.aspx";

            HttpContent postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ScriptManager1", "UpdatePanelAvailability|dlRoomTypes$ctl01$lbtnViewCalendar"),
                new KeyValuePair<string, string>("__EVENTTARGET", "dlRoomTypes$ctl01$lbtnViewCalendar"),
                new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                new KeyValuePair<string, string>("__LASTFOCUS", ""),
                new KeyValuePair<string, string>("__VIEWSTATE", ""),
                new KeyValuePair<string, string>("__EVENTVALIDATION", eventValidationValue),
                new KeyValuePair<string, string>("JavaScriptEnabled", ""),
                new KeyValuePair<string, string>("hdnClickSearchProgrammatically", "false"),
                new KeyValuePair<string, string>("hdnImageViewerHeight", "75px"),
                new KeyValuePair<string, string>("hdnSelectedRateInfo", ""),
                // TODO: Find out what exactly the other numbers in the WebMonthCalendar values are
                new KeyValuePair<string, string>("wdpArrival_clientState", "|0|012018-7-9-0-0-0-0||[[[[]],[],[]],[{},[]],\"012018-7-9-0-0-0-0\"]"),
                new KeyValuePair<string, string>("WebMonthCalendar1_clientState", "[[[[]],[],[]],[{},[]],\"01,2018,07,2018,7,9\"]"),
                new KeyValuePair<string, string>("wcNights", "1"),
                new KeyValuePair<string, string>("wdpDeparture_clientState", "|0|012018-7-10-0-0-0-0||[[[[]],[],[]],[{},[]],\"012018-7-10-0-0-0-0\"]"),
                new KeyValuePair<string, string>("WebMonthCalendar2_clientState", "[[[[]],[],[]],[{},[]],\"01,2018,07,2018,7,10\"]"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl1Short", "1"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl2Short", "0"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl3Short", "0"),
                new KeyValuePair<string, string>("ResPeople1$wcPpl4Short", "0"),
                new KeyValuePair<string, string>("wcPropertyCode", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcRDPReq2", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcRDPReq3", "*"),
                new KeyValuePair<string, string>("ucGuestRequests$wcIRMReq2", "*"),
                new KeyValuePair<string, string>("txtPromoCode", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomType", "CREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomNum", "CC234"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnDescription", "Chilcoot Hotel Room"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl01$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl02$hdnRoomType", "CREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl02$hdnRoomNum", "CC242"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl02$hdnDescription", "Chilcoot Hotel Room"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl02$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl03$hdnRoomType", "CREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl03$hdnRoomNum", "CC442"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl03$hdnDescription", "Chilcoot Hotel Room"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl03$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl04$hdnRoomType", "CREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl04$hdnRoomNum", "CCREG"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl04$hdnDescription", "Chilcoot Hotel Room"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl04$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl05$hdnRoomType", "V1BR"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl05$hdnRoomNum", "VC207"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl05$hdnDescription", "Vance Creek 1B Suite"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl05$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl06$hdnRoomType", "V1BR"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl06$hdnRoomNum", "VC302"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl06$hdnDescription", "Vance Creek 1B Suite"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl06$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("dlRoomTypes$ctl07$hdnRoomType", "V1BR"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl07$hdnRoomNum", "VC303"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl07$hdnDescription", "Vance Creek 1B Suite"),
                new KeyValuePair<string, string>("dlRoomTypes$ctl07$hdnRoomDetailURL", ""),
                new KeyValuePair<string, string>("wdwDialog_clientState", "[[[[null,3,null,null,null,null,0,null,1,0,null,0]],[[[[[null,null,null]],[[[[[]],[],null],[null,null],[null]],[[[[]],[],null],[null,null],[null]],[[[[]],[],null],[null,null],[null]]],[]],[{},[]],null],[[[[null,null,null,null]],[],[]],[{},[]],null]],[]],[{},[]],\"3,0,,,,,0\"]"),
                new KeyValuePair<string, string>("hdnPropertyName", ""),
                new KeyValuePair<string, string>("_IG_CSS_LINKS_", "~/App_Themes/Default/1CommonStyles.css|~/App_Themes/Default/Calendar.css|~/App_Themes/Default/Grids.css|~/App_Themes/Default/haRoomDetails.css|~/App_Themes/Default/ig_styles.css|~/App_Themes/Default/MonitorStyles.css|~/App_Themes/Default/OwnerStyles.css|~/App_Themes/Default/ResMain.css|~/App_Themes/Default/ResStyles.css|~/App_Themes/Default/RoomsCal.css|~/App_Themes/Default/RoundedBoxes.css|~/App_Themes/Default/Survey.css|~/App_Themes/Default/v4Buttons.css|~/App_Themes/Default/v4ig_styles.css|~/App_Themes/Default/v4OwnerStyles.css|~/App_Themes/Default/v4ResStyles.css|../ig_res/Default/ig_dialogwindow.css|../ig_res/Default/ig_monthcalendar.css|../ig_res/Default/ig_texteditor.css|../ig_res/Default/ig_shared.css"),
                new KeyValuePair<string, string>("__ASYNCPOST", "true"),
                // This property affects the outcome
                // new KeyValuePair<string, string>("btnCheckAvail", "Check Availability")
            });
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = postContent;

            HttpResponseMessage response = httpClient.SendAsync(request).Result;
            Console.WriteLine("\nResponse code = " + response.StatusCode);
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
