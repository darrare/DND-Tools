using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard
{
    public class Electrum : ICurrency
    {
        public int Value { get; set; }

        public float ConvertToGpValue() => (float)Value / 2f;

        public Electrum(string advancedDiceFormat)
        {
            Value = DiceRoller.RollForSum(advancedDiceFormat);
        }
    }
}
