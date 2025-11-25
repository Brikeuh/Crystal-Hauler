using UnityEngine;

public class AttackState : PlayerBaseState
{
    public AttackState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.ResetAnimationBools();
        player.AttackFinished = false;
    }

    public override void UpdateState()
    {
        player.SetAnimatorBool(Player.IsAttackingHash, true);

        ChangeState();
    }

    public override void ChangeState()
    {
        if (player.AttackFinished)
        {
            player.StateMachine.SetState("Idle");
            return;
        }
    }
}
