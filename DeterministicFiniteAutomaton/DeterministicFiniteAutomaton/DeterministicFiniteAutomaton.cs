using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DeterministicFiniteAutomaton
{
    /// <summary>
    /// This Deterministic Finite Automaton is designed to handle string 
    /// input and handle it character by character.
    /// </summary>
    public class DeterministicFiniteAutomaton
    {
        private List<DfaNode> Nodes { get; set; } = new List<DfaNode>();

        public DeterministicFiniteAutomaton(int nodeCount, List<Tuple<int, int, string>> edgeConnections, List<int> acceptStates)
        {
            // Create nodes
            for (int i = 0; i < nodeCount; i++)
            {
                Nodes.Add(new DfaNode());
            }

            // Mark the accept states
            foreach (var acceptState in acceptStates)
            {
                Nodes[acceptState].IsAcceptState = true;
            }

            // Create edges
            foreach (var edgeConnection in edgeConnections)
            {
                Nodes[edgeConnection.Item1]
                    .AddExitTransition(Nodes[edgeConnection.Item2], edgeConnection.Item3);
            }
        }

        public bool TestStringForValidity(string input)
        {
            DfaNode curNode = Nodes[0];
            int stringIndex = 0;
            while (stringIndex < input.Length)
            {
                // No path to transition on, invalid string
                if (curNode == null)
                    return false;

                curNode = curNode.TransitionOnChar(input[stringIndex]);
                stringIndex++;
            }

            return curNode.IsAcceptState;
        }


        #region Private classes

        internal class DfaNode
        {
            public bool IsAcceptState { get; set; } = false;
            public List<DfaEdge> ExitTransitions { get; set; } = new List<DfaEdge>();

            public DfaNode()
            {

            }

            public DfaNode TransitionOnChar(char inputChar)
            {
                List<DfaEdge> validTransitions = 
                    ExitTransitions.Where(t => Regex.IsMatch(inputChar.ToString(), t.RegexTransitionString)).ToList();


                switch (validTransitions.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return validTransitions.First().TargetNode;
                    default:
                        throw new Exception($"Multiple valid transitions detected for '{inputChar}'... '{string.Join(',', validTransitions.Select(t => t.RegexTransitionString))}'");
                }
            }

            public void AddExitTransition(DfaNode target, string regexTransitionString)
            {
                ExitTransitions.Add(new DfaEdge(this, target, regexTransitionString));
            }
        }

        internal class DfaEdge
        {
            public string RegexTransitionString { get; set; }
            public DfaNode FromNode { get; set; }
            public DfaNode TargetNode { get; set; }
            public Action PostTransitionAction { get; set; }

            public DfaEdge(DfaNode fromNode, DfaNode targetNode, string regexTransitionString)
            {
                FromNode = fromNode;
                TargetNode = targetNode;
                RegexTransitionString = regexTransitionString;
            }
        }

        #endregion
    }
}
