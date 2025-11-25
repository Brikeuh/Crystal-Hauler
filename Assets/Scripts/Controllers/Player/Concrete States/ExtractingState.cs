using UnityEngine;

public class ExtractingState : PlayerBaseState
{
    public ExtractingState(Player player) : base(player){ }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        player.SetAnimatorBool(Player.IsExtractingHash, true);

        ChangeState();
    }

    public override void ChangeState()
    {
        base.ChangeState();

        if (!player.CanExtract)
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


