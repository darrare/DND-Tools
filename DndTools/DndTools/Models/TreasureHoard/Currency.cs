using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.DiceRoller;
using Newtonsoft.Json;
using DndTools.Models.TreasureHoard.Helpers;
using DndTools.Models.TreasureHoard.JsonClasses;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace DndTools.Models.TreasureHoard
{
    public class Currency
    {
        public List<ICurrency> Pieces { get; set; } = new List<ICurrency>();


        /// <summary>
        /// Gets the total value in gold.
        /// </summary>
        public float ValueInGold
        {
            get
            {
                return Pieces.SumCurrencyToGoldValue();
            }
        }

        public Currency(int challengeRating, string calculateRollingGuideJson)
        {
            if (challengeRating < 0)
                throw new Exception($"Challenge rating of {challengeRating} out of range.");

            CurrencyRollingGuide currencyRollingGuide = 
                JsonConvert.DeserializeObject<CurrencyRollingGuide>(calculateRollingGuideJson);

            int diceRoll = DiceRoller.DiceRoller.Roll("1d100").Value;

            ChallengeLevel challengeLevel = 
                currencyRollingGuide.ChallengeLevel.First(t => challengeRating >= t.MinValue && challengeRating <= t.MaxValue);

            DiceRoll diceRollResult =
                challengeLevel.DiceRoll.First(t => diceRoll >= t.MinValue && diceRoll <= t.MaxValue);

            foreach (RolledCurrency item in diceRollResult.RolledCurrencies)
            {
                Pieces.Add(item.GenerateCurrency());
            }
        }
    }
}
