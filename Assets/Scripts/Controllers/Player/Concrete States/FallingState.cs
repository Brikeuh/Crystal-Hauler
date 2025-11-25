using UnityEngine;

public class FallingState : PlayerBaseState
{
    public FallingState(Player player) : base(player) { }

    public override void EnterState()
    {
        player.SetAnimatorBool(Player.IsFallingHash, true);
    }

    public override void UpdateState()
    {
        player.ApplyGravity();

        ChangeState();
    }

    public override void ChangeState()
    {
        if (player.IsGrounded)
        {
            player.SetAnimatorBool(Player.IsFallingHash, false);
            player.StateMachine.SetState("Idle");
            return;
        }
    }

    public override void ExitState() { }
}
