using StateMachines;
using System;
using System.Collections.Generic;

namespace Reversi
{
    /// <summary>
    /// Creates a basic state machine for the reversi game.
    /// </summary>
    public class ReversiStateMachine : StateMachine
    {
        /// <summary>
        /// Constructor for the reversi state machine.  Includes the entries for the state machine transitions for this game declared as
        /// a list of items of the form:  
        /// list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Inactive, Command.Begin), ProcessState.Active));
        /// </summary>
        public ReversiStateMachine() : base()
        {
            List<StateMachineTransitionElement> list = new List<StateMachineTransitionElement>();

            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Inactive, Command.Begin), ProcessState.Active));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.STATE_INACTIVE, Command.Exit), ProcessState.Terminated));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.STATE_INACTIVE, Command.Begin), ProcessState.Active));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Inactive, Command.Exit), ProcessState.Terminated));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Inactive, Command.Begin), ProcessState.Active));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Active, Command.End), ProcessState.Inactive));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Active, Command.Pause), ProcessState.Paused));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Paused, Command.End), ProcessState.Inactive));
            list.Add(new StateMachineTransitionElement(new StateTransition(ProcessState.Paused, Command.Resume), ProcessState.Active));

            CreateTransitionDictionary(list);
        }
        public static void TestStateMachine()
        {
            Console.WriteLine("Current State = " + CurrentState);
            Console.WriteLine("Command.Begin: Current State = " + MoveNextState(Command.Begin));
            Console.WriteLine("Command.Pause: Current State = " + MoveNextState(Command.Pause));
            Console.WriteLine("Command.End: Current State = " + MoveNextState(Command.End));
            Console.WriteLine("Command.Exit: Current State = " + MoveNextState(Command.Exit));
            Console.ReadLine();
        }
    }
}
