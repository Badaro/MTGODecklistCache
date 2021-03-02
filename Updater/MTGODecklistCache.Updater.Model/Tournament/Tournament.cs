using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Model
{
    public class Tournament
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public Uri Uri { get; set; }

        public override string ToString()
        {
            return this.Name + "|" + this.Date.ToString("yyyy-MM-dd");
        }
    }
}
