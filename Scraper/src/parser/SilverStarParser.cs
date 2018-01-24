using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Scraper
{
	namespace Parser
	{
		class SilverStarParser
		{
			public static void addRwidHeader(string pageText, Dictionary<string, string> relevantHeaders)
			{
				// Document doc = Jsoup.parse(pageText);
				HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.LoadHtml(pageText);
				IEnumerable<HtmlNode> els = doc.DocumentNode.Descendants("input");
				// List<Element> inputElements = els.stream().filter(el->el.attr("name").equals("rwid")).collect(Collectors.toList());

				List<HtmlNode> inputElements = new List<HtmlNode>();
				foreach (HtmlNode el in els)
				{
					if (el.GetAttributeValue("name", "").Equals("rwid"))
					{
						inputElements.Add(el);
					}
				}
				if (inputElements.Count > 1)
				{
					throw new Exception("Error parsing out the rwid; there are multiple valid input elements");
				}
				string rwid = inputElements[0].GetAttributeValue("value", "");
				relevantHeaders["rwid"] = rwid;
			}
		}
	}
}
