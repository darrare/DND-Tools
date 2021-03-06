using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DndTools.Models.DiceRoller.Helpers
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Takes any digits from the front of a string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>An integer at the front of the string</returns>
        public static int StripLeadingInteger(this string str)
        {
            Match leadingInteger = Regex.Match(str, @"^\d+");
            
            if (!leadingInteger.Success)
                throw new Exception($"String {str} doesn't lead with an integer");

            try
            {
                return int.Parse(leadingInteger.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing {leadingInteger.Value} to an integer.", ex);
            }
        }

        /// <summary>
        /// Takes any digits from the end of a string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>An integer at the end of the string</returns>
        public static int StripTrailingInteger(this string str)
        {
            Match trailingInteger = Regex.Match(str, @"\d+$");

            if (!trailingInteger.Success)
                throw new Exception($"String {str} doesn't end with an integer");

            try
            {
                return int.Parse(trailingInteger.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing {trailingInteger.Value} to an integer.", ex);
            }
        }

        /// <summary>
        /// Removes all the selected characters to remove from the string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="toRemove">The characters to remove</param>
        /// <returns>The string with the characters removed</returns>
        public static string RemoveAllOfTypeChars(this string str, params char[] toRemove)
        {
            string copy = (string)str.Clone();
            foreach (string c in toRemove.Select(t => t.ToString()))
            {
                copy = copy.Replace(c, "");
            }
            return copy;
        }

        /// <summary>
        /// Removes elements with the equivalent value(s) from the collection.
        /// </summary>
        /// <param name="collection">The collection to modify</param>
        /// <param name="range">The elements to remove</param>
        public static void RemoveRange(this List<int> collection, IEnumerable<int> range)
        {
            foreach (var i in range)
            {
                collection.Remove(i);
            }
        }
    }
}
