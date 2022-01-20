using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard.Helpers
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Converts the list to it's value in Gold Pieces.
        /// </summary>
        /// <param name="currencies">Collection of currencies</param>
        /// <returns>Value in Gold Pieces</returns>
        public static float SumCurrencyToGoldValue(this List<ICurrency> currencies)
            => currencies.Sum(t => t.ConvertToGpValue());
    }
}
