using UnityEngine;

public class IdleState :  PlayerBaseState
{
    private float horizontalMovement;
    private float verticalMovement;
    public IdleState(Player player) : base(player) { }

    public override void EnterState() 
    {
        base.EnterState();
        player.ResetAnimationBools(); // Reset all animator bools, make sure the animator is in the idle state.
    }

    public override void UpdateState()
    {
        horizontalMovement = player.MoveValue.x;
        verticalMovement = player.MoveValue.y;

        player.ApplyGravity();

        if (Mathf.Abs(horizontalMovement) > 0.1f || Mathf.Abs(verticalMovement) > 0.1f)
        {
            player.StateMachine.SetState("Movement");
            return;
        }
    }
    
    public override void ExitState() 
    {
        base.ExitState();
    }
}
