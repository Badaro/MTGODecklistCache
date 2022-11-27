using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MtgMelee
{
    internal class MtgMeleeDeckInfo
    {
        public Deck Deck { get; set; }
        public Standing Standing { get; set; }
        public RoundV2[] Rounds { get; set; }
    }
}
