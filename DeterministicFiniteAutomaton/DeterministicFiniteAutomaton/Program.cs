using System;
using System.Collections.Generic;
using System.Linq;

namespace DeterministicFiniteAutomaton
{
    class Program
    {
        static void Main(string[] args)
        {
        

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
