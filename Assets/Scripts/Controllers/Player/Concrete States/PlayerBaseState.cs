using UnityEngine;

public abstract class PlayerBaseState
{
    protected Player player;

    public PlayerBaseState(Player player)
    {
        this.player = player;
    }

    public virtual void EnterState() 
    { 
        Debug.Log($"Entered {this.GetType().Name}");
    }

    public abstract void UpdateState();

    public virtual void ExitState() 
    {
        Debug.Log($"Exited {this.GetType().Name}");
    }
}
