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

        public static TreasureHoardModel Create(int challengeLevel, string currencyRollingGuideJson)
        {
            TreasureHoardModel model = new TreasureHoardModel();
            model.GenerateHoard(challengeLevel, currencyRollingGuideJson);
            return model;
        }

        public void GenerateHoard(int challengeLevel, string currencyRollingGuideJson)
        {
            Currency = new Currency(challengeLevel, currencyRollingGuideJson);
        }
    }
}
