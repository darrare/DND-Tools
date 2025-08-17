using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeterministicFiniteAutomaton
{
    public class RollResults
    {
        /// <summary>
        /// This is manipulated explicitly in the automata to get accurate results when
        /// calling multiple keep functions in the same command notation. Do not use this
        /// value anywhere but within DiceRoller class
        /// </summary>
        public List<int> PlaceholderRollBucket { get; set; } = new List<int>();
        public List<int> Rolls { get; set; } = new List<int>();
        public float Multiplier { get; set; } = 1;
        public int Addition { get; set; } = 0;
        public int Value { get { return (int)((float)Rolls.Sum() * Multiplier + Addition); } }
    }

    public static class DiceRoller
    {
        private static RollResults ResultsPointer { get; set; }

        private static DeterministicFiniteAutomaton Automaton { get; set; }

        static DiceRoller()
        {
            InitializeAutomiton();
        }

        public static RollResults Roll(string advancedDiceNotation)
        {
            Random.SetSeed(0);
            RollResults results = new RollResults();
            ResultsPointer = results;

            if (Automaton.RunString(advancedDiceNotation))
                return results;
            else
                throw new Exception($"Failure on rolling dice with notation {advancedDiceNotation}");
        }

        private static void InitializeAutomiton()
        {
            int nodeCount = 18;

            Action<string> rollDice = (workingString) =>
            {
                int numDice = workingString.StripLeadingInteger();
                int diceSize = workingString.StripTrailingInteger();
                for (int i = 0; i < numDice; i++)
                {
                    ResultsPointer.PlaceholderRollBucket.Add(Random.Int(1, diceSize + 1));
                }
            };
            Action<string> keepHigh = (workingString) =>
            {
                int numToKeep = workingString.StripTrailingInteger();
                IEnumerable<int> consideredRolls = ResultsPointer.PlaceholderRollBucket.OrderByDescending(t => t).Take(numToKeep);
                ResultsPointer.Rolls.AddRange(consideredRolls);
                ResultsPointer.PlaceholderRollBucket.RemoveRange(consideredRolls);
            };
            Action<string> keepLow = (workingString) =>
            {
                int numToKeep = workingString.StripTrailingInteger();
                IEnumerable<int> consideredRolls = ResultsPointer.PlaceholderRollBucket.OrderByDescending(t => t).TakeLast(numToKeep);
                ResultsPointer.Rolls.AddRange(consideredRolls);
                ResultsPointer.PlaceholderRollBucket.RemoveRange(consideredRolls);
            };
            Action<string> dropHigh = (workingString) =>
            {
                int numToDrop = workingString.StripTrailingInteger();
               IEnumerable<int> consideredRolls = ResultsPointer.PlaceholderRollBucket.OrderByDescending(t => t).TakeLast(ResultsPointer.PlaceholderRollBucket.Count - numToDrop);
                ResultsPointer.Rolls.AddRange(consideredRolls);
                ResultsPointer.PlaceholderRollBucket.RemoveRange(consideredRolls);
            };
            Action<string> dropLow = (workingString) =>
            {
                int numToDrop = workingString.StripTrailingInteger();
                IEnumerable<int> consideredRolls = ResultsPointer.PlaceholderRollBucket.OrderByDescending(t => t).Take(ResultsPointer.PlaceholderRollBucket.Count - numToDrop);
                ResultsPointer.Rolls.AddRange(consideredRolls);
                ResultsPointer.PlaceholderRollBucket.RemoveRange(consideredRolls);
            };
            Action<string> multiplier = (workingString) => 
            { 
                if (Regex.IsMatch(workingString, "[×x*]"))
                {
                    ResultsPointer.Multiplier = workingString.StripTrailingInteger();
                }
                else
                {
                    ResultsPointer.Multiplier = 1f / workingString.StripTrailingInteger();
                }
            };
            Action<string> addition = (workingString) => 
            {
                if (Regex.IsMatch(workingString, "[+]"))
                {
                    ResultsPointer.Addition = workingString.StripTrailingInteger();
                }
                else
                {
                    ResultsPointer.Addition = -workingString.StripTrailingInteger();
                }
            };

            List<Tuple<int, int, string, Action<string>>> edgeConnections = new List<Tuple<int, int, string, Action<string>>>()
            {
                // xdy
                new Tuple<int, int, string, Action<string>>(0, 1, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(1, 1, "[0-9]", null),
                new Tuple<int, int, string, Action<string>>(1, 2, "[d]", null),
                new Tuple<int, int, string, Action<string>>(2, 3, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(3, 3, "[0-9]", null),

                // Keeps
                new Tuple<int, int, string, Action<string>>(3, 4, "[k]", rollDice),
                new Tuple<int, int, string, Action<string>>(4, 5, "[h]", null),
                new Tuple<int, int, string, Action<string>>(4, 7, "[l]", null),
                new Tuple<int, int, string, Action<string>>(5, 6, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(6, 6, "[0-9]", null),
                new Tuple<int, int, string, Action<string>>(6, 4, "[k]", keepHigh),
                new Tuple<int, int, string, Action<string>>(7, 8, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(8, 8, "[0-9]", null),
                new Tuple<int, int, string, Action<string>>(8, 4, "[k]", keepLow),

                // Drops
                new Tuple<int, int, string, Action<string>>(3, 9, "[d]", rollDice),
                new Tuple<int, int, string, Action<string>>(9, 10, "[h]", null),
                new Tuple<int, int, string, Action<string>>(9, 12, "[l]", null),
                new Tuple<int, int, string, Action<string>>(10, 11, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(11, 11, "[0-9]", null),
                new Tuple<int, int, string, Action<string>>(11, 9, "[d]", dropHigh),
                new Tuple<int, int, string, Action<string>>(12, 13, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(13, 13, "[0-9]", null),
                new Tuple<int, int, string, Action<string>>(13, 9, "[d]", dropLow),

                // Multiplier
                new Tuple<int, int, string, Action<string>>(3, 14, @"[×x*\/÷]", rollDice),
                new Tuple<int, int, string, Action<string>>(6, 14, @"[×x*\/÷]", null),
                new Tuple<int, int, string, Action<string>>(8, 14, @"[×x*\/÷]", null),
                new Tuple<int, int, string, Action<string>>(11, 14, @"[×x*\/÷]", null),
                new Tuple<int, int, string, Action<string>>(13, 14, @"[×x*\/÷]", null),
                new Tuple<int, int, string, Action<string>>(14, 15, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(15, 15, "[0-9]", null),

                // Additions
                new Tuple<int, int, string, Action<string>>(3, 16, "[+-]", rollDice),
                new Tuple<int, int, string, Action<string>>(6, 16, "[+-]", keepHigh),
                new Tuple<int, int, string, Action<string>>(8, 16, "[+-]", keepLow),
                new Tuple<int, int, string, Action<string>>(11, 16, "[+-]", dropHigh),
                new Tuple<int, int, string, Action<string>>(13, 16, "[+-]", dropLow),
                new Tuple<int, int, string, Action<string>>(15, 16, "[+-]", multiplier),
                new Tuple<int, int, string, Action<string>>(16, 17, "[1-9]", null),
                new Tuple<int, int, string, Action<string>>(17, 17, "[0-9]", null),
            };

            List<Tuple<int, Action<string>>> acceptStates = new List<Tuple<int, Action<string>>>()
            {
                new Tuple<int, Action<string>>(3, rollDice),
                new Tuple<int, Action<string>>(6, keepHigh),
                new Tuple<int, Action<string>>(8, keepLow),
                new Tuple<int, Action<string>>(11, dropHigh),
                new Tuple<int, Action<string>>(13, dropLow),
                new Tuple<int, Action<string>>(15, multiplier),
                new Tuple<int, Action<string>>(17, addition),
            };

            Automaton = new DeterministicFiniteAutomaton(nodeCount, edgeConnections, acceptStates);
        }
    }
}
