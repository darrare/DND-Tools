using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard
{
    public class Copper : ICurrency
    {
        public int Value { get; set; }

        public CurrencyTypeEnum Type { get => CurrencyTypeEnum.Copper; }

        public float ConvertToGpValue() => (float)Value / 100f;

        public Copper(string advancedDiceFormat)
        {
            Value = DiceRoller.DiceRoller.Roll(advancedDiceFormat).Value;
        }
    }
}
