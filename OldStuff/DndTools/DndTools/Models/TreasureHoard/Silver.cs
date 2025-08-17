using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard
{
    public class Silver : ICurrency
    {
        public int Value { get; set; }

        public CurrencyTypeEnum Type { get => CurrencyTypeEnum.Silver; }

        public float ConvertToGpValue() => (float)Value / 10f;

        public Silver(string advancedDiceFormat)
        {
            Value = DiceRoller.DiceRoller.Roll(advancedDiceFormat).Value;
        }
    }
}
