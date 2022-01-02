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

        public float ConvertToGpValue() => (float)Value / 100f;

        public Copper(string advancedDiceFormat)
        {
            Value = DiceRoller.RollForSum(advancedDiceFormat);
        }
    }
}
