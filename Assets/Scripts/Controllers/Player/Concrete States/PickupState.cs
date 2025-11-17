using UnityEngine;

public class PickupState : PlayerBaseState
{
    public PickupState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        player.SetAnimatorBool(Player.IsPickingUpHash, true);

        ChangeState();
    }

    public override void ChangeState()
    {
        base.ChangeState();

        if(!player.CanPickup)
        {
            player.StateMachine.SetState("Idle");
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    
}
