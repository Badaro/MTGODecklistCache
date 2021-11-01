using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace MTGODecklistCache.Updater.Tools
{
    public static class CardNameNormalizer
    {
        static readonly string _apiEndpoint = "https://api.scryfall.com/cards/search?order=cmc&q={query}";
        static Dictionary<string, string> _normalization = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static CardNameNormalizer()
        {
            AddCardsByCriteria("is:split", (f, b) => $"{f} // {b}");
            AddCardsByCriteria("is:dfc -is:extra", (f, b) => $"{f}");
            AddCardsByCriteria("is:adventure", (f, b) => $"{f}");
            AddCardsByCriteria("is:flip", (f, b) => $"{f}");

            // ManaTraders normalization errors
            _normalization.Add("Full Art Plains", "Plains");
            _normalization.Add("Full Art Island", "Island");
            _normalization.Add("Full Art Swamp", "Swamp");
            _normalization.Add("Full Art Mountain", "Mountain");
            _normalization.Add("Full Art Forest", "Forest");
        }

        public static string Normalize(string card)
        {
            if (_normalization.ContainsKey(card)) return _normalization[card];
            else return card;
        }

        private static void AddCardsByCriteria(string criteria, Func<string, string, string> createTargetName)
        {
            string api = _apiEndpoint.Replace("{query}", WebUtility.UrlEncode(criteria));
            bool hasMore;
            do
            {
                string json = new WebClient().DownloadString(api);
                dynamic data = JsonConvert.DeserializeObject(json);

                foreach (var card in data.data)
                {
                    string front = card.card_faces[0].name;
                    string back = card.card_faces[1].name;
                    string target = createTargetName(front, back);

                    _normalization.Add(front, target);
                    _normalization.Add($"{front}/{back}", target);
                    _normalization.Add($"{front} / {back}", target);
                    _normalization.Add($"{front}//{back}", target);
                    _normalization.Add($"{front} // {back}", target);
                    _normalization.Add($"{front}///{back}", target);
                    _normalization.Add($"{front} /// {back}", target);
                }

                hasMore = data.has_more;
                api = data.next_page;
            }
            while (hasMore);
        }
    }
}
