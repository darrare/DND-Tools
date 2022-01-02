using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard.Helpers
{
    public static class DiceRoller
    {
        /// <summary>
        /// Rolls a dice using the advanced dice format.
        /// NOTE: This format is limited to using only xdy and selective results notation.
        /// Any multipliers or additions should call the RollForSum function instead.
        /// Examples: 2d6kh1, 20d10kh2kl2, 15d20dl2dh2
        /// Bad examples: kh1 (doesn't include xdy), 10d6kh1dl1 (includes both keep and drop)
        /// </summary>
        /// <param name="advancedDiceFormat">Format string</param>
        /// <returns>List of dice</returns>
        public static List<int> Roll(string advancedDiceFormat)
        {
            // Ensure the proper notation for this function
            // See examples in summary comment of this method
            if (!Regex.IsMatch(advancedDiceFormat, @"\d+d\d+(k[hl](\d|(\d)))*")
                && !Regex.IsMatch(advancedDiceFormat, @"\d+d\d+(d[hl](\d|(\d)))*"))
            {
                throw new Exception($"{advancedDiceFormat} is invalid.");
            }

            List<int> rolledValues = new List<int>();
            bool[] charTracker = new bool[advancedDiceFormat.Length];

            // Must have at the base level, xdy notation where x and y are integers
            Match xdy;
            if ((xdy = Regex.Match(advancedDiceFormat, @"\d+d\d+")).Captures.Count != 1)
            {
                throw new Exception($"{advancedDiceFormat} is invalid. Failed when looking for xdy. Only one instance of xdy is allowed.");
            }

            // Mark the bits as "used"
            markBitsAsUsed(charTracker, xdy.Index, xdy.Value.Length);

            // Pull the integers from the string result
            int x = xdy.Value.StripLeadingInteger();
            int y = xdy.Value.StripTrailingInteger();

            // Roll the dice and add them to the collection
            for (int i = 0; i < x; i++)
            {
                rolledValues.Add(Random.Int(1, y));
            }

            // ---------- Check for selective results (k[hl]y or k[hl](y) and d[hl]y or d[hl](y))------------
            Match keeps;
            Match drops;
            if ((keeps = Regex.Match(advancedDiceFormat, @"k[hl](\d+|\(\d+\))")).Success
                ^ (drops = Regex.Match(advancedDiceFormat, @"d[hl](\d+|\(\d+\))")).Success)
            {
                if (keeps.Success)
                {
                    Match current = keeps;
                    List<int> keepingRolls = new List<int>();
                    List<int> tempRolledValues = rolledValues.ToList(); // Copies
                    do
                    {
                        markBitsAsUsed(charTracker, current.Index, current.Value.Length);

                        string result = current.Value.RemoveAllOfTypeChars('(', ')');
                        char highLow = result[1];
                        int count = result.StripTrailingInteger();

                        if (highLow != 'h' && highLow != 'l')
                        {
                            throw new Exception($"Error trying to parse index 1 as h or l from {result}. Received {highLow}.");
                        }

                        // Add the keeps to the keeping rolls list
                        List<int> toKeep = removeExcludedRolls(tempRolledValues, highLow == 'h', tempRolledValues.Count - count);
                        keepingRolls.AddRange(toKeep);

                        // Remove the values already kept so they can't be kept more than once
                        toKeep.ForEach(t => tempRolledValues.Remove(t));
                    } while ((current = current.NextMatch()).Value != "");

                    // Assign the kept rolls to the rolled values collection
                    rolledValues = keepingRolls;
                }
                else if (drops.Success)
                {
                    Match current = drops;
                    do
                    {
                        markBitsAsUsed(charTracker, current.Index, current.Value.Length);

                        string result = current.Value.RemoveAllOfTypeChars('(', ')');
                        char highLow = result[1];
                        int count = result.StripTrailingInteger();

                        if (highLow != 'h' && highLow != 'l')
                        {
                            throw new Exception($"Error trying to parse index 1 as h or l from {result}. Received {highLow}.");
                        }

                        // Remove the drops
                        rolledValues = removeExcludedRolls(rolledValues, highLow == 'l', count);
                    } while ((current = current.NextMatch()).Value != "");
                }
            }
            else if (keeps.Success && drops.Success)
            {
                throw new Exception($"Both keeps (k) and drops(d) cannot be used in the same command: {advancedDiceFormat}");
            }

            // Multipliers checked in sum method

            // Additions checked in sum method

            // -------------- Verifiy that the appropriate bits have been handled ---------------
            foreach (bool bit in charTracker)
            {
                if (!bit)
                {
                    throw new Exception($"Not all characters in {advancedDiceFormat} were used. " +
                        $"Any multipliers or additions applied to the string must call the RollForSum method instead.");
                }
            }

            // Randomly sort at the end so the results feel interesting. I don't know if this will ever be necessary, but you never know.
            return rolledValues.OrderBy(t => Random.Float()).ToList();
        }

        private static List<int> removeExcludedRolls(List<int> rolledValues, bool removeFromFront, int count)
        {
            List<int> sortedRolls = rolledValues.OrderBy(t => t).ToList();

            if (!removeFromFront)
            {
                sortedRolls.Reverse();
            }

            for (int i = 0; i < count; i++)
            {
                if (sortedRolls.Count > 0)
                    sortedRolls.RemoveAt(0);
            }

            return sortedRolls;
        }

        private static void markBitsAsUsed(bool[] charTracker, int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                if (charTracker[i] == true)
                {
                    throw new Exception($"Checking already handled string index {i}.");
                }

                charTracker[i] = true;
            }
        }

        /// <summary>
        /// Rolls based on the advancedDiceFormat and returns the sum value.
        /// </summary>
        /// <param name="advancedDiceFormat">Format</param>
        /// <param name="ifDivisionRoundUp">Division truncates. 5/2 = 2 for example. This will round up instead of round down.</param>
        /// <returns>Integer of the dice value</returns>
        public static int RollForSum(string advancedDiceFormat, bool ifDivisionRoundUp = false)
        {
            bool[] charTracker = new bool[advancedDiceFormat.Length];

            // Parse out the bit that we want to use for the roll function
            Match rollAndSelective;
            if ((rollAndSelective = Regex.Match(advancedDiceFormat, @"\d+d\d+(k[hl](\d+|\(\d+\)))*")).Captures.Count == 0)
            {
                if ((rollAndSelective = Regex.Match(advancedDiceFormat, @"\d+d\d+(d[hl](\d+|\(\d+\)))*")).Captures.Count == 0)
                {
                    throw new Exception($"{advancedDiceFormat} is invalid.");
                }
            }

            if (rollAndSelective.Captures.Count != 1)
            {
                throw new Exception($"{advancedDiceFormat} is invalid.");
            }

            markBitsAsUsed(charTracker, 0, rollAndSelective.Length);

            // Get the total value using just the roll and selective parts of the string
            float totalValue = Roll(rollAndSelective.Value).Sum();

            // -------------- Check for multipliers (× or x or * and / or ÷)------------------
            Match multipliers;
            if ((multipliers = Regex.Match(advancedDiceFormat, @"[×x*\/÷]\d+")).Captures.Count > 0)
            {
                Match current = multipliers;
                do
                {
                    markBitsAsUsed(charTracker, current.Index, current.Value.Length);

                    char modifier = current.Value[0];
                    float multiplier = current.Value.StripTrailingInteger();

                    if (modifier == '×' || modifier == 'x' || modifier == '*')
                    {
                        totalValue *= multiplier;
                    }
                    else if (modifier == '/' || modifier == '÷')
                    {
                        totalValue /= multiplier;
                    }
                    else
                    {
                        throw new Exception($"Pulled invalid modifier from {current.Value}. Format string: {advancedDiceFormat}");
                    }
                } while ((current = current.NextMatch()).Value != "");
            }

            // Apply any +x or -x at the end of the advancedDiceFormat string
            Match addition;
            if ((addition = Regex.Match(advancedDiceFormat, @"[+-]\d+")).Captures.Count > 1)
            {
                throw new Exception($"String format: {advancedDiceFormat} has more than one addition or subtraction modifier which is invalid.");
            }
            else if (addition.Captures.Count == 1)
            {
                markBitsAsUsed(charTracker, addition.Index, addition.Value.Length);
                if (addition.Value[0] == '+')
                {
                    totalValue += addition.Value.StripTrailingInteger();
                }
                else if (addition.Value[0] == '-')
                {
                    totalValue -= addition.Value.StripTrailingInteger();
                }
            }

            // -------------- Verifiy that the appropriate bits have been handled ---------------
            foreach (bool bit in charTracker)
            {
                if (!bit)
                {
                    throw new Exception($"Not all characters in {advancedDiceFormat} were used. " +
                        $"Any multipliers or additions applied to the string must call the RollForSum method instead.");
                }
            }

            return ifDivisionRoundUp ? (int)totalValue + 1 : (int)totalValue;
        }
    }
}
