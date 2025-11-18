using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class PlayerStateMachine
{
    private Dictionary<string, PlayerBaseState> states;
    private PlayerBaseState currentState;
    public PlayerBaseState CurrentState => currentState;
    public PlayerStateMachine()
    {
        states = new Dictionary<string, PlayerBaseState>();
    }

    public void AddState(string name, PlayerBaseState state)
    {
        states[name] = state;
    }

    public void SetState(string name) // Switches to the specified state, called mainly by the states themselves
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        if (states.ContainsKey(name))
        {
            currentState = states[name];
            currentState.EnterState();
        }
    }

    public void Update() // Called in Player.Update(), functions as the state machine's Update loop
    {
        if (currentState != null) 
        {
            currentState.UpdateState();
        }
            
    }
}

