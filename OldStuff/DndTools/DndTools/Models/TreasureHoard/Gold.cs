using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;
using DndTools.Models.DiceRoller;

namespace DndTools.Models.TreasureHoard
{
    public class Gold : ICurrency
    {
        public int Value { get; set; }

        public CurrencyTypeEnum Type { get => CurrencyTypeEnum.Gold; }

        public float ConvertToGpValue() => (float)Value;

        public Gold(string advancedDiceFormat)
        {
            Value = DiceRoller.DiceRoller.Roll(advancedDiceFormat).Value;
        }
    }
}
