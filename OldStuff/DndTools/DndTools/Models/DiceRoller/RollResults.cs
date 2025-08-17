using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndTools.Models.DiceRoller
{
    /// <summary>
    /// Holder for the results of a roll.
    /// In the future, if we want some form of a display it is useful
    /// to know what dice are left, and which are removed. So that's why
    /// we keep the discarded dice recorded.
    /// </summary>
    public class RollResults
    {
        /// <summary>
        /// Results of the xdy roll.
        /// </summary>
        public List<RollResult> Results { get; set; } = new List<RollResult>();

        /// <summary>
        /// Custom query to pull only the dice that aren't
        /// excluded due to any selective rules.
        /// </summary>
        public List<int> KeptRolls 
        { 
            get 
            { 
                return Results
                    .Where(t => !t.HasBeenRemoved.HasValue || !t.HasBeenRemoved.Value)
                    .Select(t => t.Value).ToList(); 
            } 
        }

        /// <summary>
        /// Custom query to pull only the dice that are
        /// excluded due to any selective rules.
        /// </summary>
        public List<int> DiscardedRolls
        {
            get
            {
                return Results
                    .Where(t => t.HasBeenRemoved.HasValue && t.HasBeenRemoved.Value)
                    .Select(t => t.Value).ToList();
            }
        }

        /// <summary>
        /// Any multiplier to the total value of the dice.
        /// </summary>
        public float Multiplier { get; set; } = 1;

        /// <summary>
        /// Any addition (you can add negatively) to the total value of the dice.
        /// </summary>
        public int Addition { get; set; } = 0;

        /// <summary>
        /// The total value of the dice, with modifiers included.
        /// </summary>
        public int Value { get { return (int)((float)KeptRolls.Sum() * Multiplier + Addition); } }
    }

    /// <summary>
    /// Helper object to hold the result of a roll, and track whether or not it has been removed.
    /// nullable bool used to track multiple keeps so we can perform the right action.
    /// </summary>
    public class RollResult : IComparable<RollResult>
    {
        /// <summary>
        /// Value of the dice
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// null = Hasn't been touched by any keep commands
        /// false = Valid dice
        /// true = invalid dice
        /// </summary>
        public bool? HasBeenRemoved { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value of the dice</param>
        /// <param name="hasBeenRemoved">If it has been removed</param>
        public RollResult(int value, bool? hasBeenRemoved = null)
        {
            Value = value;
            HasBeenRemoved = hasBeenRemoved;
        }

        /// <summary>
        /// Compares two RollResult objects.
        /// We only care about comparing the Value property.
        /// </summary>
        /// <param name="other">The other object to compare</param>
        /// <returns>-1 this is less than, 0 equal to, 1 this is greater than</returns>
        public int CompareTo(RollResult other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
