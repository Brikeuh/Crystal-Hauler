using UnityEngine;

public abstract class PlayerBaseState
{
    protected Player player;

    public PlayerBaseState(Player player)
    {
        this.player = player;
    }

    public virtual void EnterState() { }

    public abstract void UpdateState();

    public virtual void ChangeState() { }

    public virtual void ExitState() { }
}
