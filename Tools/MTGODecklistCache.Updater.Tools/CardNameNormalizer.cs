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

            // Wizards normalization errors
            _normalization.Add("Lurrus of the Dream Den", "Lurrus of the Dream-Den");
            _normalization.Add("Kongming, ??quot?Sleeping Dragon??quot?", "Kongming, \"Sleeping Dragon\"");
            _normalization.Add("GhazbA?n Ogre", "Ghazbán Ogre");
            _normalization.Add("Lim-DA?l's Vault", "Lim-Dûl's Vault");
            _normalization.Add("Lim-DAul's Vault", "Lim-Dûl's Vault");
            _normalization.Add("SAcance", "Séance");
            _normalization.Add("Æther Vial", "Aether Vial");
            _normalization.Add("Ghirapur Æther Grid", "Ghirapur Aether Grid");
            _normalization.Add("Unravel the Æther", "Unravel the Aether");
            _normalization.Add("JuzA?m Djinn", "Juzám Djinn");
            _normalization.Add("Sol'kanar the Tainted", "Sol'Kanar the Tainted");
            _normalization.Add("Minsc ?amp? Boo, Timeless Heroes", "Minsc & Boo, Timeless Heroes");

            // Starcitygames normalization errors
            _normalization.Add("Hall of the Storm Giants", "Hall of Storm Giants");

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
