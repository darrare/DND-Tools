using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard.Helpers
{
    public class RollResults
    {
        public List<RollResult> Results { get; set; } = new List<RollResult>();

        public List<int> ValidRolls 
        { 
            get 
            { 
                return Results
                    .Where(t => !t.HasBeenRemoved.HasValue || !t.HasBeenRemoved.Value)
                    .Select(t => t.Value).ToList(); 
            } 
        }

        public float Multiplier { get; set; } = 1;
        public int Addition { get; set; } = 0;
        public int Value { get { return (int)((float)ValidRolls.Sum() * Multiplier + Addition); } }
    }

    public class RollResult : IComparable<RollResult>
    {
        public int Value { get; set; }
        public bool? HasBeenRemoved { get; set; }

        public RollResult(int value, bool? hasBeenRemoved = null)
        {
            Value = value;
            HasBeenRemoved = hasBeenRemoved;
        }

        public int CompareTo(RollResult other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
