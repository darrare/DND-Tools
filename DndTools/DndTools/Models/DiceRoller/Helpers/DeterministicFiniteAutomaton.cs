using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DndTools.Models.DiceRoller.Helpers
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
        /// <summary>
        /// Collection of nodes that represent the DFA states.
        /// </summary>
        private List<DfaNode> Nodes { get; set; } = new List<DfaNode>();

        /// <summary>
        /// Used by the actions performed during transitions to make impactful 
        /// logical operations that makes this DFA class useful.
        /// </summary>
        private string WorkingString { get; set; } = "";

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="nodeCount">Number of nodes</param>
        /// <param name="edgeConnections">Detailed information about the edges of the DFA</param>
        /// <param name="acceptStates">Accepting states with accepting actions</param>
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

        /// <summary>
        /// Run a string through the DFA
        /// </summary>
        /// <param name="input">The string to run</param>
        /// <returns>True if success, False if failure</returns>
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

            if (curNode.IsAcceptState)
            {
                if (curNode.SuccessAction != null)
                {
                    curNode.SuccessAction(WorkingString);
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Private classes

        /// <summary>
        /// Internal DFA node class for the DFA's use.
        /// </summary>
        internal class DfaNode
        {
            /// <summary>
            /// The parent DFA holding this node (used for referencing the DFA's WorkingString variable).
            /// </summary>
            public DeterministicFiniteAutomaton Container { get; set; }

            /// <summary>
            /// Is this node an accepting state.
            /// </summary>
            public bool IsAcceptState { get; set; } = false;

            /// <summary>
            /// Transitions away from this node.
            /// </summary>
            public List<DfaEdge> ExitTransitions { get; set; } = new List<DfaEdge>();

            /// <summary>
            /// Action to perform upon the string ending on this node.
            /// </summary>
            public Action<string> SuccessAction { get; set; }

            /// <summary>
            /// Public constructor
            /// </summary>
            /// <param name="container">The container holding this node</param>
            public DfaNode(DeterministicFiniteAutomaton container)
            {
                Container = container;
            }

            /// <summary>
            /// Transition to another node based on the inputChar.
            /// </summary>
            /// <param name="inputChar">Determines which transition to take (or if to fail)</param>
            /// <returns>The new node we transitioned to</returns>
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

            /// <summary>
            /// Adds a transition option to this node.
            /// </summary>
            /// <param name="target">Node to transition to</param>
            /// <param name="regexTransitionString">string that must be met in order to transition</param>
            /// <param name="preTransitionAction">Action to occur just before the transition happens</param>
            public void AddExitTransition(DfaNode target, string regexTransitionString, Action<string> preTransitionAction)
            {
                ExitTransitions.Add(new DfaEdge(this, target, regexTransitionString, preTransitionAction));
            }
        }

        /// <summary>
        /// Internal DFA edge class for the DFA's use.
        /// </summary>
        internal class DfaEdge
        {
            /// <summary>
            /// String that must be met in order to transition.
            /// </summary>
            public string RegexTransitionString { get; set; }

            /// <summary>
            /// Starting node.
            /// </summary>
            public DfaNode FromNode { get; set; }

            /// <summary>
            /// Node that this edge takes us to.
            /// </summary>
            public DfaNode TargetNode { get; set; }

            /// <summary>
            /// Action to occur just before we transition to TargetNode.
            /// </summary>
            public Action<string> PreTransitionAction { get; set; }

            /// <summary>
            /// Public constructor.
            /// </summary>
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
