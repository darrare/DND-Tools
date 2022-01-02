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
    /// 
    /// Functions based off of a "WorkingString" variable that holds each character in a string
    /// until an action is performed, then it wipes the WorkingString so it is empty for the next actions.
    /// </summary>
    public class DeterministicFiniteAutomaton
    {
        private List<DfaNode> Nodes { get; set; } = new List<DfaNode>();
        private string WorkingString { get; set; } = "";

        public DeterministicFiniteAutomaton(int nodeCount, List<Tuple<int, int, string, Action<string>>> edgeConnections, List<Tuple<int, Action<string>>> acceptStates)
        {
            // Create nodes
            for (int i = 0; i < nodeCount; i++)
            {
                Nodes.Add(new DfaNode(this));
            }

            // Mark the accept states
            foreach (var acceptState in acceptStates)
            {
                Nodes[acceptState.Item1].IsAcceptState = true;
                Nodes[acceptState.Item1].SuccessAction = acceptState.Item2;
            }

            // Create edges
            foreach (var edgeConnection in edgeConnections)
            {
                Nodes[edgeConnection.Item1]
                    .AddExitTransition(Nodes[edgeConnection.Item2], edgeConnection.Item3, edgeConnection.Item4);
            }
        }

        public bool RunString(string input)
        {
            WorkingString = "";

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
            public DeterministicFiniteAutomaton Container { get; set; }
            public bool IsAcceptState { get; set; } = false;
            public List<DfaEdge> ExitTransitions { get; set; } = new List<DfaEdge>();
            public Action<string> SuccessAction { get; set; }

            public DfaNode(DeterministicFiniteAutomaton container)
            {
                Container = container;
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
                        // Do PreTransitionAction before going to the next node
                        DfaEdge transition = validTransitions.First();
                        if (transition.PreTransitionAction != null)
                        {
                            transition.PreTransitionAction(Container.WorkingString);
                            Container.WorkingString = "";
                        }

                        Container.WorkingString += inputChar;
                        return transition.TargetNode;
                    default:
                        throw new Exception($"Multiple valid transitions detected for '{inputChar}'... '{string.Join(',', validTransitions.Select(t => t.RegexTransitionString))}'");
                }
            }

            public void AddExitTransition(DfaNode target, string regexTransitionString, Action<string> preTransitionAction)
            {
                ExitTransitions.Add(new DfaEdge(this, target, regexTransitionString, preTransitionAction));
            }
        }

        internal class DfaEdge
        {
            public string RegexTransitionString { get; set; }
            public DfaNode FromNode { get; set; }
            public DfaNode TargetNode { get; set; }
            public Action<string> PreTransitionAction { get; set; }

            public DfaEdge(DfaNode fromNode, DfaNode targetNode, string regexTransitionString, Action<string> preTransitionAction)
            {
                FromNode = fromNode;
                TargetNode = targetNode;
                RegexTransitionString = regexTransitionString;
                PreTransitionAction = preTransitionAction;
            }
        }

        #endregion
    }
}
