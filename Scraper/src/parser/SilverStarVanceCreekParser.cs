using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Scraper.Parser
{
    class SilverStarVanceCreekParser
    {
        public static readonly string EVENT_VALIDATION = "__EVENTVALIDATION";
        public static string GetEventValidationValue(string page)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);

            HtmlNode inputTag = htmlDocument.GetElementbyId(EVENT_VALIDATION);
            return inputTag.GetAttributeValue("value", "NO VALUE FOUND");
        }
    }
}
