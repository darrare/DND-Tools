using System;
using System.Collections.Generic;

namespace DeterministicFiniteAutomaton
{
    class Program
    {
        static void Main(string[] args)
        {
            int nodeCount = 18;

            List<Tuple<int, int, string>> edgeConnections = new List<Tuple<int, int, string>>()
            {
                // xdy
                new Tuple<int, int, string>(0, 1, "[1-9]"),
                new Tuple<int, int, string>(1, 1, "[0-9]"),
                new Tuple<int, int, string>(1, 2, "[d]"),
                new Tuple<int, int, string>(2, 3, "[1-9]"),
                new Tuple<int, int, string>(3, 3, "[0-9]"),

                // Keeps
                new Tuple<int, int, string>(3, 4, "[k]"),
                new Tuple<int, int, string>(4, 5, "[h]"),
                new Tuple<int, int, string>(4, 7, "[l]"),
                new Tuple<int, int, string>(5, 6, "[1-9]"),
                new Tuple<int, int, string>(6, 6, "[0-9]"),
                new Tuple<int, int, string>(6, 4, "[k]"),
                new Tuple<int, int, string>(7, 8, "[1-9]"),
                new Tuple<int, int, string>(8, 8, "[0-9]"),
                new Tuple<int, int, string>(8, 4, "[k]"),

                // Drops
                new Tuple<int, int, string>(3, 9, "[d]"),
                new Tuple<int, int, string>(9, 10, "[h]"),
                new Tuple<int, int, string>(9, 12, "[l]"),
                new Tuple<int, int, string>(10, 11, "[1-9]"),
                new Tuple<int, int, string>(11, 11, "[0-9]"),
                new Tuple<int, int, string>(11, 9, "[d]"),
                new Tuple<int, int, string>(12, 13, "[1-9]"),
                new Tuple<int, int, string>(13, 13, "[0-9]"),
                new Tuple<int, int, string>(13, 9, "[d]"),

                // Multiplier
                new Tuple<int, int, string>(3, 14, @"[×x*\/÷]"),
                new Tuple<int, int, string>(6, 14, @"[×x*\/÷]"),
                new Tuple<int, int, string>(8, 14, @"[×x*\/÷]"),
                new Tuple<int, int, string>(11, 14, @"[×x*\/÷]"),
                new Tuple<int, int, string>(13, 14, @"[×x*\/÷]"),
                new Tuple<int, int, string>(14, 15, "[1-9]"),
                new Tuple<int, int, string>(15, 15, "[0-9]"),

                // Additions
                new Tuple<int, int, string>(3, 16, "[+-]"),
                new Tuple<int, int, string>(6, 16, "[+-]"),
                new Tuple<int, int, string>(7, 16, "[+-]"),
                new Tuple<int, int, string>(11, 16, "[+-]"),
                new Tuple<int, int, string>(13, 16, "[+-]"),
                new Tuple<int, int, string>(15, 16, "[+-]"),
                new Tuple<int, int, string>(16, 17, "[1-9]"),
                new Tuple<int, int, string>(17, 17, "[0-9]"),
            };

            List<int> acceptStates = new List<int>() { 3, 6, 8, 11, 13, 15, 17 };

            DeterministicFiniteAutomaton myAuto = new DeterministicFiniteAutomaton(nodeCount, edgeConnections, acceptStates);

            bool result;


            // All valid
            result = myAuto.TestStringForValidity("2d6");
            result = myAuto.TestStringForValidity("15d20kh5");
            result = myAuto.TestStringForValidity("15d20kh5");
            result = myAuto.TestStringForValidity("15d20kl5");
            result = myAuto.TestStringForValidity("15d20dh5");
            result = myAuto.TestStringForValidity("15d20dl5");
            result = myAuto.TestStringForValidity("15d20kh50");
            result = myAuto.TestStringForValidity("15d20kl50");
            result = myAuto.TestStringForValidity("15d20dh50");
            result = myAuto.TestStringForValidity("15d20dl50");
            result = myAuto.TestStringForValidity("15d20dl5dh5");
            result = myAuto.TestStringForValidity("15d20dh5dl5");
            result = myAuto.TestStringForValidity("15d20kl5kh5");
            result = myAuto.TestStringForValidity("15d20kh5kl5");
            result = myAuto.TestStringForValidity("15d20dl1dh20");
            result = myAuto.TestStringForValidity("15d20dh1dl20");
            result = myAuto.TestStringForValidity("15d20dl20dh1");
            result = myAuto.TestStringForValidity("15d20dh20dl1");
            result = myAuto.TestStringForValidity("15d20kl1kh20");
            result = myAuto.TestStringForValidity("15d20kh1kl20");
            result = myAuto.TestStringForValidity("15d20kl20kh1");
            result = myAuto.TestStringForValidity("15d20kh20kl1");
            result = myAuto.TestStringForValidity("15d20x2");
            result = myAuto.TestStringForValidity("15d20*2");
            result = myAuto.TestStringForValidity("15d20/2");
            result = myAuto.TestStringForValidity("15d20+50");
            result = myAuto.TestStringForValidity("15d20-50");
            result = myAuto.TestStringForValidity("15d20/2+2");
            result = myAuto.TestStringForValidity("15d20/2-2");
            result = myAuto.TestStringForValidity("15d20x2+2");
            result = myAuto.TestStringForValidity("15d20x2-2");

            // Some invalid
            result = myAuto.TestStringForValidity("15d20dh5kl5x");
            result = myAuto.TestStringForValidity("x15d20kh5dl5");
            result = myAuto.TestStringForValidity("1x5d20dh5kl5");
            result = myAuto.TestStringForValidity("15xd20kh5dl5");
            result = myAuto.TestStringForValidity("15dx20dh5kl5");
            result = myAuto.TestStringForValidity("15d2x0kh5dl5");
            result = myAuto.TestStringForValidity("15d20xdh5kl5");
            result = myAuto.TestStringForValidity("15d20kxh5dl5");
            result = myAuto.TestStringForValidity("15d20dhx5kl5");
            result = myAuto.TestStringForValidity("15d20kh5xdl5");
            result = myAuto.TestStringForValidity("15d20dh5kxl5");
            result = myAuto.TestStringForValidity("15d20kh5dlx5");
            result = myAuto.TestStringForValidity("15d20dh5kl5");
            result = myAuto.TestStringForValidity("15d20kh5dl5");
            result = myAuto.TestStringForValidity("15d20+50+50");
            result = myAuto.TestStringForValidity("15d20+50-50");
            result = myAuto.TestStringForValidity("15d20-50-50");
        }
    }
}
