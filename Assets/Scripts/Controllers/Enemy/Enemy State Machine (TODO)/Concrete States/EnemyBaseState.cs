using UnityEngine;

public abstract class EnemyBaseState
{
    protected Enemy enemy;

    public EnemyBaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void EnterState() { }

    public abstract void UpdateState();

    public virtual void ChangeState() { }

    public virtual void ExitState() { }
}
