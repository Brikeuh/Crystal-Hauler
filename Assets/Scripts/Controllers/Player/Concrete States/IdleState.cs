using UnityEngine;

public class IdleState :  PlayerBaseState
{
    private float horizontalMovement;
    private float verticalMovement;
    public IdleState(Player player) : base(player) { }

    public override void EnterState() 
    {
        base.EnterState();
        Debug.Log("Entered Idle State");
        player.ResetAnimationBools(); // Reset all animator bools, make sure the animator is in the idle state.
    }

    public override void UpdateState()
    {
        horizontalMovement = player.MoveValue.x;
        verticalMovement = player.MoveValue.y;

        player.ApplyGravity();

        ChangeState();

        if (player.JumpPressed && player.IsGrounded && Time.time >= player.NextJumpTime)
        {
            player.SetAnimatorBool(Player.IsJumpingHash, true);
            player.Jump();

            player.NextJumpTime = Time.time + player.JumpCooldown;
        }
        else
        {
            player.SetAnimatorBool(Player.IsJumpingHash, false);
        }
    }
    
    public override void ChangeState()
    {
        if (Mathf.Abs(horizontalMovement) > 0.1f || Mathf.Abs(verticalMovement) > 0.1f)
        {
            player.StateMachine.SetState("Movement");
            return;
        }
        else if (player.Velocity.y < -0.1f && !player.IsGrounded)
        {
            player.StateMachine.SetState("Falling");
            return;
        }
        else if(player.CanPickup)
        {
            player.StateMachine.SetState("Pickup");
            return;
        }
        else if (player.CanExtract)
        {
            player.StateMachine.SetState("Extract");
            return;
        }

    }

    public override void ExitState() 
    {
        base.ExitState();
    }
}
