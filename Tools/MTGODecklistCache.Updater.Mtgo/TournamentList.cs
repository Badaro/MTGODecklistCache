using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace MTGODecklistCache.Updater.Mtgo
{
    public static class TournamentList
    {
        static string _listUrl = "https://www.mtgo.com/en/mtgo/decklists/{year}/{month}";
        static string _rootUrl = "https://www.mtgo.com";

        public static Tournament[] GetTournaments(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null) endDate = DateTime.Now;

            List<Tournament> results = new List<Tournament>();
            for (var currentDate = startDate; (currentDate.Year < endDate.Value.Year) || (currentDate.Year == endDate.Value.Year && currentDate.Month <= endDate.Value.Month); currentDate = currentDate.AddMonths(1))
            {
                string tournamentListUrl = _listUrl.Replace("{month}", currentDate.Month.ToString("D2")).Replace("{year}", currentDate.Year.ToString());

                string htmlContent;
                using (WebClient client = new WebClient())
                {
                    htmlContent = client.DownloadString(tournamentListUrl);
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                var tournamentNodes = doc.DocumentNode.SelectNodes("//li[@class='decklists-item']");
                if (tournamentNodes == null) continue;

                foreach (var tournamentNode in tournamentNodes)
                {
                    var title = tournamentNode.SelectSingleNode("a/div/h3").InnerHtml;
                    var url = tournamentNode.SelectSingleNode("a").Attributes["href"].Value;
                    var dateString = tournamentNode.SelectSingleNode("a/time").Attributes["datetime"].Value;

                    DateTime parsedDate = DateTime.Parse(dateString).ToUniversalTime();
                    url = _rootUrl + url;

                    var uri = new Uri(url);

                    results.Add(new Tournament()
                    {
                        Name = title,
                        Date = parsedDate,
                        Uri = uri,
                        JsonFile = Path.ChangeExtension(Path.GetFileName(uri.PathAndQuery), ".json")
                    });
                }
            }

            return results.Where(t => t.Date >= startDate && t.Date <= endDate).OrderByDescending(t => t.Date).ToArray();
        }
    }

}
