using System;
using System.Collections.Generic;
using System.Linq;

namespace DeterministicFiniteAutomaton
{
    class Program
    {
        static void Main(string[] args)
        {
            // Valid
            RollResults r1 = DiceRoller.Roll("2d6");
            RollResults r2 = DiceRoller.Roll("15d20");
            RollResults r3 = DiceRoller.Roll("15d20kh5");
            RollResults r4 = DiceRoller.Roll("15d20kl5");
            RollResults r5 = DiceRoller.Roll("15d20dh5");
            RollResults r6 = DiceRoller.Roll("15d20dl5");
            RollResults r7 = DiceRoller.Roll("15d20kh50");
            RollResults r8 = DiceRoller.Roll("15d20kl50");
            RollResults r9 = DiceRoller.Roll("15d20dh50");
            RollResults r10 = DiceRoller.Roll("15d20dl50");
            RollResults r11 = DiceRoller.Roll("15d20dl5dh5");
            RollResults r12 = DiceRoller.Roll("15d20dh5dl5");
            RollResults r13 = DiceRoller.Roll("15d20kl5kh5");
            RollResults r14 = DiceRoller.Roll("15d20kh5kl5");
            RollResults r15 = DiceRoller.Roll("15d20dl1dh20");
            RollResults r16 = DiceRoller.Roll("15d20dh1dl20");
            RollResults r17 = DiceRoller.Roll("15d20dl20dh1");
            RollResults r18 = DiceRoller.Roll("15d20dh20dl1");
            RollResults r19 = DiceRoller.Roll("15d20kl1kh20");
            RollResults r20 = DiceRoller.Roll("15d20kh1kl20");
            RollResults r21 = DiceRoller.Roll("15d20kl20kh1");
            RollResults r22 = DiceRoller.Roll("15d20kh20kl1");
            RollResults r23 = DiceRoller.Roll("15d20x2");
            RollResults r24 = DiceRoller.Roll("15d20*2");
            RollResults r25 = DiceRoller.Roll("15d20/2");
            RollResults r26 = DiceRoller.Roll("15d20+50");
            RollResults r27 = DiceRoller.Roll("15d20-50");
            RollResults r28 = DiceRoller.Roll("15d20/2+2");
            RollResults r29 = DiceRoller.Roll("15d20/2-2");
            RollResults r30 = DiceRoller.Roll("15d20x2+2");
            RollResults r31 = DiceRoller.Roll("15d20x2-2");

            // Invalid
            RollResults r32 = DiceRoller.Roll("15d20dh5kl5x");
            RollResults r33 = DiceRoller.Roll("x15d20kh5dl5");
            RollResults r34 = DiceRoller.Roll("1x5d20dh5kl5");
            RollResults r35 = DiceRoller.Roll("15xd20kh5dl5");
            RollResults r36 = DiceRoller.Roll("15dx20dh5kl5");
            RollResults r37 = DiceRoller.Roll("15d2x0kh5dl5");
            RollResults r38 = DiceRoller.Roll("15d20xdh5kl5");
            RollResults r39 = DiceRoller.Roll("15d20kxh5dl5");
            RollResults r40 = DiceRoller.Roll("15d20dhx5kl5");
            RollResults r41 = DiceRoller.Roll("15d20kh5xdl5");
            RollResults r42 = DiceRoller.Roll("15d20dh5kxl5");
            RollResults r43 = DiceRoller.Roll("15d20kh5dlx5");
            RollResults r44 = DiceRoller.Roll("15d20dh5kl5");
            RollResults r45 = DiceRoller.Roll("15d20kh5dl5");
            RollResults r46 = DiceRoller.Roll("15d20+50+50");
            RollResults r47 = DiceRoller.Roll("15d20+50-50");
            RollResults r48 = DiceRoller.Roll("15d20-50-50");
        }
    }
}
