using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;
using Newtonsoft.Json;
using DndTools.Models.TreasureHoard.JsonClasses;

namespace DndTools.Models.TreasureHoard
{
    public class Currency
    {
        List<ICurrency> Pieces { get; set; } = new List<ICurrency>();

        public Currency(int challengeRating, string calculateRollingGuideJson)
        {
            if (challengeRating < 0)
                throw new Exception($"Challenge rating of {challengeRating} out of range.");

            int diceRoll = DiceRoller.RollForSum("1d100");

            CurrencyRollingGuide jsonData = 
                JsonConvert.DeserializeObject<CurrencyRollingGuide>(calculateRollingGuideJson);
        }
    }
}
