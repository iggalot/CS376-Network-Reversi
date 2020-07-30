using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StateMachines
{
    /// <summary>
    /// Enum for the different available states in this machine.
    /// </summary>
    public enum ProcessState
    {
        STATE_INACTIVE,
        STATE_CONNECTING,
        STATE_ACKNOWLEDGE,
        STATE_STARTGAME,
        STATE_PLAYER1_TURN,
        STATE_PLAYER2_TURN,
        STATE_LISTENING,
        STATE_SENDDATA,
        STATE_WAITINGDATA_PLAYER1,
        STATE_WAITINGDATA_PLAYER2,
        STATE_GAMEISOVER,
        STATE_CLOSECONNECTION,
        Inactive,
        Active,
        Paused,
        Terminated

    }

    /// <summary>
    /// Commands that are defined to move from state to the next state in the state machine
    /// </summary>
    public enum Command
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }

    /// <summary>
    /// A class defining the state machine TransitionDictionary
    /// See <see cref="StateTransition"/> and <see cref="ProcessState"/> for details of available items for this machine
    /// </summary>
    public class StateMachineTransitionElement
    {
        // The current transition (state, command)
        public StateTransition Transition { get; set; }
        
        // The destination state
        public ProcessState State { get; set; }
        public StateMachineTransitionElement(StateTransition t, ProcessState p)
        {
            Transition = t;
            State = p; 
        }
    }

    /// <summary>
    /// Class that defines a basic state transition pair
    /// </summary>
    public class StateTransition
    {
        public readonly ProcessState CurrentState;
        public readonly Command Command;

        public StateTransition(ProcessState currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    /// <summary>
    /// A class that defines the state machine behavior, including creation of the dictionary for
    /// referencing the different states and the defined moves between them.
    /// </summary>
    public class StateMachine
    {
        // Dictionary containing our state transitions
        public static Dictionary<StateTransition, ProcessState> TransitionDictionary;

        // The current state machine
        public static ProcessState CurrentState;

        #region Constructor

        static StateMachine()
        {
            TransitionDictionary = new Dictionary<StateTransition, ProcessState>();
            CurrentState = ProcessState.STATE_INACTIVE;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="list">A list of defined <see cref="StateMachineTransitionElement"/> needed to create the transition dictionary</param>
        public void CreateTransitionDictionary(List<StateMachineTransitionElement> list)
        {
            TransitionDictionary = new Dictionary<StateTransition, ProcessState>();

            // Build our dictionary of transition states.
            //  {new StateTransition(ProcessState.STATE_INACTIVE, Command.Exit), ProcessState.Terminated }
            foreach (StateMachineTransitionElement item in list)
            {
                ProcessState item_next_state = item.State;
                StateTransition item_trans = item.Transition;

                ProcessState item_trans_state = item_trans.CurrentState;
                Command item_trans_command = item_trans.Command;

                // Search for duplicate entries in the dictionary.
                bool itemFound = false;
                foreach(KeyValuePair<StateTransition,ProcessState> entry in TransitionDictionary)
                {
                    // If a duplicate is found stop searching since we don't need to add a new entry if its already in there
                    if((entry.Key.CurrentState == item_trans_state) && (entry.Key.Command == item_trans_command) && (entry.Value == item_next_state))
                    {
                        Console.WriteLine("Duplicate found in: " + "< " + item_trans_state + ", " + item_trans_command + " > , " + item_next_state);
                        itemFound = true;
                        break;
                    } 
                }

                // if a match wasn't found, add a new entry to the transition dictionary.
                if(!itemFound)
                {
                    Console.WriteLine("Adding state machine entry: < " + item_trans_state + ", " + item_trans_command + " > , " + item_next_state);
                    TransitionDictionary.Add(item.Transition, item.State);
                }
            }
        }


        #endregion

        /// <summary>
        /// Retrieves the next state from our state machine
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>

        public static ProcessState GetNext(Command command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            ProcessState nextState;
            if (!TransitionDictionary.TryGetValue(transition, out nextState))
                throw new System.Exception("Invalid transition: " + CurrentState + " -> " + command);
            return nextState;
        }

        /// <summary>
        /// Changes the current state to the next appropriate state
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static ProcessState MoveNextState(Command command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }
    }

}
