using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard
{
    public class TreasureHoardModel
    {
        public Currency Currency { get; set; }

        public TreasureHoardModel()
        {

        }

        public void GenerateHoard(int challengeLevel, string currencyRollingGuideJson)
        {
            Currency = new Currency(challengeLevel, currencyRollingGuideJson);
        }
    }
}
