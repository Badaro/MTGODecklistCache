using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards
{
    internal class WizardsAjaxResult
    {
        public int status { get; set; }
        public string[] data { get; set; }
        public int offset { get; set; }
        public int displaySeeMore { get; set; }
    }
}
