using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DndTools.Models.TreasureHoard.Helpers
{
    /// <summary>
    /// Static dice rolling class to allow advanced dice notation into logical dice rolls.
    /// </summary>
    public static class DiceRoller
    {
        /// <summary>
        /// Pointer to the results object so that the automata actions
        /// have some data to use when making decisions.
        /// </summary>
        private static RollResults ResultsPointer { get; set; }

        /// <summary>
        /// Automaton used for determining and performing logical operations
        /// on the advanced dice notation string.
        /// </summary>
        private static DeterministicFiniteAutomaton Automaton { get; set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static DiceRoller()
        {
            InitializeAutomaton();
        }

        /// <summary>
        /// Generates roll results using the advanced dice notation string
        /// </summary>
        /// <param name="advancedDiceNotation">Determines how to roll</param>
        /// <returns>Results of the roll</returns>
        public static RollResults Roll(string advancedDiceNotation)
        {
            RollResults results = new RollResults();
            ResultsPointer = results;

            if (Automaton.RunString(advancedDiceNotation))
                return results;
            else
                throw new Exception($"Failure on rolling dice with notation {advancedDiceNotation}");
        }

        /// <summary>
        /// Initialize the automaton to know how to handle advanced dice notation
        /// </summary>
        private static void InitializeAutomaton()
        {
            int nodeCount = 18;

            // Create the actions used while running the automaton to record the string information.
            Action<string> rollDice = (workingString) =>
            {
                int numDice = workingString.StripLeadingInteger();
                int diceSize = workingString.StripTrailingInteger();
                for (int i = 0; i < numDice; i++)
                {
                    ResultsPointer.Results.Add(new RollResult(Random.Int(1, diceSize + 1)));
                }
            };
            Action<string> keepHigh = (workingString) =>
            {
                int numToKeep = workingString.StripTrailingInteger();
                if (ResultsPointer.Results.Any(t => t.HasBeenRemoved.HasValue))
                {
                    ResultsPointer.Results.OrderByDescending(t => t)
                       .Take(numToKeep).ToList()
                       .ForEach(t => t.HasBeenRemoved = false);
                }
                else
                {
                    ResultsPointer.Results.OrderByDescending(t => t)
                        .TakeLast(ResultsPointer.Results.Count - numToKeep).ToList()
                        .ForEach(t => t.HasBeenRemoved = true);
                    ResultsPointer.Results.Where(t => !t.HasBeenRemoved.HasValue).ToList().ForEach(t => t.HasBeenRemoved = false);
                }
            };
            Action<string> keepLow = (workingString) =>
            {
                int numToKeep = workingString.StripTrailingInteger();
                if (ResultsPointer.Results.Any(t => t.HasBeenRemoved.HasValue))
                {
                    ResultsPointer.Results.OrderByDescending(t => t)
                       .TakeLast(numToKeep).ToList()
                       .ForEach(t => t.HasBeenRemoved = false);
                }
                else
                {
                    ResultsPointer.Results.OrderByDescending(t => t)
                        .Take(ResultsPointer.Results.Count - numToKeep).ToList()
                        .ForEach(t => t.HasBeenRemoved = true);
                    ResultsPointer.Results.Where(t => !t.HasBeenRemoved.HasValue).ToList().ForEach(t => t.HasBeenRemoved = false);
                }
            };
            Action<string> dropHigh = (workingString) =>
            {
                int numToDrop = workingString.StripTrailingInteger();
                ResultsPointer.Results.OrderByDescending(t => t).Take(numToDrop).ToList().ForEach(t => t.HasBeenRemoved = true);
            };
            Action<string> dropLow = (workingString) =>
            {
                int numToDrop = workingString.StripTrailingInteger();
                ResultsPointer.Results.OrderByDescending(t => t).TakeLast(numToDrop).ToList().ForEach(t => t.HasBeenRemoved = true);
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

            // All edge connections with instructions on what to do pre-transition.
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

            // Accepting states with actions to perform on accept
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

            // Create the automaton
            Automaton = new DeterministicFiniteAutomaton(nodeCount, edgeConnections, acceptStates);
        }
    }
}
