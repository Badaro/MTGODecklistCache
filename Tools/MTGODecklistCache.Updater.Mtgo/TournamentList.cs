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

                foreach (var tournamentNode in doc.DocumentNode.SelectNodes("//li[@class='decklists-item']"))
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

        //private static Tournament[] ParseTournaments(string pageContent)
        //{
        //    List<Tournament> result = new List<Tournament>();

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(pageContent);

        //    foreach (var tournamentNode in doc.DocumentNode.SelectNodes("div/a"))
        //    {
        //        var tournamentUrl = tournamentNode.Attributes["href"].Value;
        //        var tournamentName = tournamentNode.SelectSingleNode("div/div[@class='title']").InnerText.Replace("\t", "").Replace("\n", "").Trim();

        //        string tournamentDate = String.Join(" ", tournamentName.Split(' ').TakeLast(3));
        //        tournamentName = tournamentName.Replace(tournamentDate, "").Trim();

        //        var tournamentUri = new Uri(_rootUrl + tournamentUrl);
        //        result.Add(new Tournament()
        //        {
        //            Name = tournamentName,
        //            Date = DateTime.Parse(tournamentDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime(),
        //            Uri = tournamentUri,
        //            JsonFile = $"{Path.GetFileName(tournamentUri.LocalPath)}.json"
        //        });
        //    }

        //    return result.ToArray();
        //}

        //private static string FormatDateForUrl(DateTime date)
        //{
        //    var day = date.Day.ToString("D2");
        //    var month = date.Month.ToString("D2");
        //    var year = date.Year.ToString("D4");
        //    return $"{month}%2F{day}%2F{year}";
        //}
    }

}
