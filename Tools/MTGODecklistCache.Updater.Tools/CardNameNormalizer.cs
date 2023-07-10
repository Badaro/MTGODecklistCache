using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace MTGODecklistCache.Updater.Tools
{
    public static class CardNameNormalizer
    {
        static readonly string _apiEndpoint = "https://api.scryfall.com/cards/search?order=cmc&q={query}";
        static Dictionary<string, string> _normalization = new Dictionary<string, string>(StringComparer.InvariantCulture);

        static CardNameNormalizer()
        {
            AddTextReplacement("Aether -is:dfc", "Aether", "Æther");
            AddTextReplacement("Aether -is:dfc", "Aether", "Ã\u0086ther");
            AddMultinameCards("is:dfc -is:extra Aether", (f, b) => $"{f}", t => $"{t.Replace("Aether", "Æther")}", true);
            AddMultinameCards("is:dfc -is:extra Aether", (f, b) => $"{f}", t => $"{t.Replace("Aether", "Ã\u0086ther")}", true);

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
            _normalization.Add("LÃ³rien Revealed", "Lórien Revealed");
            _normalization.Add("Troll of Khazad-dÃ»m", "Troll of Khazad-dûm");
            _normalization.Add("PalantÃ­r of Orthanc", "Palantír of Orthanc");
            _normalization.Add("SmÃ©agol, Helpful Guide", "Sméagol, Helpful Guide");

            // Starcitygames normalization errors
            _normalization.Add("Hall of the Storm Giants", "Hall of Storm Giants");
        }

        public static string Normalize(string card)
        {
            card = card.Trim();
            if (_normalization.ContainsKey(card)) return _normalization[card];
            else return card;
        }

        private static void AddTextReplacement(string query, string validString, string invalidString)
        {
            string api = _apiEndpoint.Replace("{query}", WebUtility.UrlEncode(query));
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


        private static void AddMultinameCards(string criteria, Func<string, string, string> createTargetName, Func<string, string> textReplacement = null, bool onlyCombinedNames = false)
        {
            string api = _apiEndpoint.Replace("{query}", WebUtility.UrlEncode(criteria));
            bool hasMore;
            do
            {
                string json = new WebClient().DownloadString(api);
                dynamic data = JsonConvert.DeserializeObject(json);

                foreach (var card in data.data)
                {
                    // FDB: Sometimes happens during spoiler season there's a DFC card "partially" 
                    //      added with only one face known, those need to be skipped
                    if (!(card as JObject).ContainsKey("card_faces")) continue;

                    string front = card.card_faces[0].name;
                    string back = card.card_faces[1].name;

                    if (textReplacement != null) front = textReplacement(front);
                    if (textReplacement != null) back = textReplacement(back);

                    string target = createTargetName(front, back);

                    if(!onlyCombinedNames) _normalization.Add(front, target);
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
