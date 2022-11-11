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
            AddTextReplacement("Aether", "Æther");
            AddTextReplacement("Aether", "Ã\u0086ther");

            AddMultinameCards("is:split", (f, b) => $"{f} // {b}");
            AddMultinameCards("is:dfc -is:extra", (f, b) => $"{f}");
            AddMultinameCards("is:adventure", (f, b) => $"{f}");
            AddMultinameCards("is:flip", (f, b) => $"{f}");

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
            _normalization.Add("JuzA?m Djinn", "Juzám Djinn");
            _normalization.Add("Sol'kanar the Tainted", "Sol'Kanar the Tainted");
            _normalization.Add("Minsc ?amp? Boo, Timeless Heroes", "Minsc & Boo, Timeless Heroes");

            // MTGO normalization errors
            _normalization.Add("Jotun Grunt", "Jötun Grunt");
            _normalization.Add("Lim-DÃ»l's Vault", "Lim-Dûl's Vault");
            _normalization.Add("Rain Of Tears", "Rain of Tears");
            _normalization.Add("Altar Of Dementia", "Altar of Dementia");
            _normalization.Add("JuzÃ¡m Djinn", "Juzám Djinn");
            _normalization.Add("SÃ©ance", "Séance");
            _normalization.Add("Tura KennerÃ¼d, Skyknight", "Tura Kennerüd, Skyknight");

            // Starcitygames normalization errors
            _normalization.Add("Hall of the Storm Giants", "Hall of Storm Giants");

        }

        public static string Normalize(string card)
        {
            card = card.Trim();
            if (_normalization.ContainsKey(card)) return _normalization[card];
            else return card;
        }

        private static void AddTextReplacement(string validString, string invalidString)
        {
            string api = _apiEndpoint.Replace("{query}", WebUtility.UrlEncode(validString));
            bool hasMore;
            do
            {
                string json = new WebClient().DownloadString(api);
                dynamic data = JsonConvert.DeserializeObject(json);

                foreach (var card in data.data)
                {
                    string cardName = card.name;
                    string invalidCardName = cardName.Replace(validString, invalidString);

                    if (!cardName.Contains(validString)) continue; // For some odd reason scryfall returns "Breya, Etherium Shaper" when searching for "Aether"
                    _normalization.Add(invalidCardName, cardName);
                }

                hasMore = data.has_more;
                api = data.next_page;
            }
            while (hasMore);
        }


        private static void AddMultinameCards(string criteria, Func<string, string, string> createTargetName)
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
