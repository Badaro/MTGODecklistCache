using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Updater
{
    // Qty,Card,Sideboard,Companion,Player_Name,Player_Id,Tournament_Name,Tournament_Number,Player_Username
    public class ManaTradersCsvRecord
    {
        [Index(0)]
        public int Count { get; set; }
        [Index(1)]
        public string Card { get; set; }
        [Index(2)]
        public bool Sideboard { get; set; }
        [Index(8)]
        public string Player { get; set; }
    }
}
