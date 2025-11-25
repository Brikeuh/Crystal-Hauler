using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class EnemyStateMachine
{
    private Dictionary<string, EnemyBaseState> states;
    private EnemyBaseState currentState;
    public EnemyBaseState CurrentState => currentState;
    public EnemyStateMachine()
    {
        states = new Dictionary<string, EnemyBaseState>();
    }

    public void AddState(string name, EnemyBaseState state)
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