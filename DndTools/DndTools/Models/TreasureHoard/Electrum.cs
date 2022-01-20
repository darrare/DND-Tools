using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.DiceRoller;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard
{
    public class Electrum : ICurrency
    {
        public int Value { get; set; }

        public CurrencyTypeEnum Type { get => CurrencyTypeEnum.Electrum; }
        public float ConvertToGpValue() => (float)Value / 2f;

        public Electrum(string advancedDiceFormat)
        {
            Value = DiceRoller.DiceRoller.Roll(advancedDiceFormat).Value;
        }
    }
}
