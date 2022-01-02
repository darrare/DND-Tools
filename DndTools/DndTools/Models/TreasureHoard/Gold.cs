using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard.Helpers;

namespace DndTools.Models.TreasureHoard
{
    public class Gold : ICurrency
    {
        public int Value { get; set; }

        public float ConvertToGpValue() => (float)Value;

        public Gold(string advancedDiceFormat)
        {
            Value = DiceRoller.RollForSum(advancedDiceFormat);
        }
    }
}
