using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace MTGODecklistCache.Updater.ManaTraders
{
    public static class TournamentList
    {
        static string _tournamentListUrl = "https://www.manatraders.com/tournaments/2";
        static string _tournamentRootUrl = "https://www.manatraders.com";

        public static Tournament[] GetTournaments()
        {
            List<Tournament> tournaments = new List<Tournament>();

            string htmlContent;
            using (WebClient client = new WebClient())
            {
                htmlContent = client.DownloadString(_tournamentListUrl);
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var tournamentListNode = doc.DocumentNode.SelectSingleNode("//select");

            foreach(var tournamentNode in tournamentListNode.SelectNodes("option"))
            {
                string dateAndFormat = tournamentNode.InnerText;
                string url = tournamentNode.Attributes["value"].Value;

                string[] dateAndFormatSegments = dateAndFormat.Split("|").Select(s => s.Trim()).ToArray();
                string monthAndYear = dateAndFormatSegments[0];
                string format = dateAndFormatSegments[1];

                DateTime tournamentDate = DateTime.Parse($"01 {monthAndYear}", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                tournamentDate = tournamentDate.AddMonths(1).AddDays(-1); // We're using the last day of the month as date
                if (tournamentDate > DateTime.UtcNow) continue;

                // Skips invitationals for now since they are broken
                if (tournamentDate.Month == 12) continue;

                format = format[0].ToString().ToUpper() + format.Substring(1); // Standardize with first letter in uppercase

                tournaments.Add(new Tournament()
                {
                    Name = $"ManaTraders Series {format} {monthAndYear}",
                    Date = tournamentDate,
                    Uri = new Uri($"{_tournamentRootUrl}{url}/"),
                    JsonFile = $"manatraders-series-{format.ToLower()}-{monthAndYear.ToLower().Replace(" ","-")}-{tournamentDate.ToString("yyyy-MM-dd")}.json"
                });
            }

            return tournaments.ToArray();
        }
    }

}
