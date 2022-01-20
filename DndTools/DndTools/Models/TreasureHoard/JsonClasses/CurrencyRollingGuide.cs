using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard.JsonClasses
{
    public class CurrencyRollingGuide
    {
        [JsonPropertyName("challengeLevel")]
        public List<ChallengeLevel> ChallengeLevel { get; set; }
    }

    public class ChallengeLevel
    {
        [JsonPropertyName("levelRange")]
        public string LevelRange { get; set; }

        public int MinValue { get { return LevelRange.StripLeadingInteger(); } }
        public int MaxValue { get { return LevelRange.StripTrailingInteger(); } }

        [JsonPropertyName("diceRoll")]
        public List<DiceRoll> DiceRoll { get; set; }
    }

    public class DiceRoll
    {
        [JsonPropertyName("rollRange")]
        public string RollRange { get; set; }

        public int MinValue { get { return RollRange.StripLeadingInteger(); } }
        public int MaxValue { get { return RollRange.StripTrailingInteger(); } }

        [JsonPropertyName("rolledCurrencies")]
        public List<RolledCurrency> RolledCurrencies { get; set; }
    }

    public class RolledCurrency
    {
        private static Dictionary<string, CurrencyTypeEnum> JsonInputToValidCurrencyType = new Dictionary<string, CurrencyTypeEnum>()
        {
            { "cp", CurrencyTypeEnum.Copper },
            { "copper", CurrencyTypeEnum.Copper },
            { "sp", CurrencyTypeEnum.Silver },
            { "silver", CurrencyTypeEnum.Silver },
            { "ep", CurrencyTypeEnum.Electrum },
            { "electrum", CurrencyTypeEnum.Electrum },
            { "gp", CurrencyTypeEnum.Gold },
            { "gold", CurrencyTypeEnum.Gold },
            { "pp", CurrencyTypeEnum.Platinum },
            { "platinum", CurrencyTypeEnum.Platinum },
        };

        [JsonPropertyName("currencyType")]
        public string CurrencyType { get; set; }

        public CurrencyTypeEnum CurrencyTypeEnum 
        { 
            get 
            {
                if (!JsonInputToValidCurrencyType.ContainsKey(CurrencyType))
                    throw new Exception($"\"{CurrencyType}\" not valid to convert to CurrencyTypeEnum.");

                return JsonInputToValidCurrencyType[CurrencyType]; 
            } 
        }

        [JsonPropertyName("rollFormat")]
        public string RollFormat { get; set; }

        /// <summary>
        /// Generates the currency represented by this object.
        /// </summary>
        /// <returns>The currency represented by this object</returns>
        public ICurrency GenerateCurrency()
        {
            switch (CurrencyTypeEnum)
            {
                case CurrencyTypeEnum.Copper:
                    return new Copper(RollFormat);
                case CurrencyTypeEnum.Silver:
                    return new Silver(RollFormat);
                case CurrencyTypeEnum.Electrum:
                    return new Electrum(RollFormat);
                case CurrencyTypeEnum.Gold:
                    return new Gold(RollFormat);
                case CurrencyTypeEnum.Platinum:
                    return new Platinum(RollFormat);
                default:
                    throw new Exception($"Error generating currency of type {CurrencyType} and roll format {RollFormat}");
            }
        }
    }
}
