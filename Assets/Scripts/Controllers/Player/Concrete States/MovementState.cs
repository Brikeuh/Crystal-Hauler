using UnityEngine;

public class MovementState : PlayerBaseState
{
    private float horizontalMovement;
    private float verticalMovement;
    private float speed;
    private Vector3 move;

    public MovementState(Player player) : base(player) { }

    public override void EnterState()
    {
        base.EnterState();
        move = new Vector3();
    }

    public override void UpdateState()
    {
        horizontalMovement = player.MoveValue.x;
        verticalMovement = player.MoveValue.y;

        player.ApplyGravity();
        HandleInput();
        HandleMovement(speed);
        ChangeState();
    }

    public override void ChangeState()
    {
        if (Mathf.Abs(horizontalMovement) < 0.1f && Mathf.Abs(verticalMovement) < 0.1f && !player.JumpPressed && player.IsGrounded)
        {
            player.StateMachine.SetState("Idle");
            return;
        }
        else if (player.Velocity.y < -0.1f && !player.IsGrounded)
        {
            player.StateMachine.SetState("Falling");
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }  

    private void HandleInput()
    {
        if (Mathf.Abs(horizontalMovement) > 0.1f || Mathf.Abs(verticalMovement) > 0.1f)
        {
            player.SetAnimatorBool(Player.IsWalkingHash, true);
        }
        else if (Mathf.Abs(horizontalMovement) < 0.1f && Mathf.Abs(verticalMovement) < 0.1f)
        {
            player.SetAnimatorBool(Player.IsWalkingHash, false);
        }
        
        if (player.RunPressed)
        {
            player.SetAnimatorBool(Player.IsRunningHash, true);
            speed = player.MoveSpeed * player.RunModifier;
        }
        else if (!player.RunPressed)
        {
            player.SetAnimatorBool(Player.IsRunningHash, false);
            speed = player.MoveSpeed;
        }

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

    private void HandleMovement(float moveSpeed)
    {
        // Handle Movement
        move.x = horizontalMovement;
        move.y = Mathf.Max(-0.01f, move.y + player.Gravity * Time.deltaTime);
        move.z = verticalMovement;

        move = Quaternion.AngleAxis(player.CameraTransform.rotation.eulerAngles.y, Vector3.up) * move;
        player.Move(move, moveSpeed);

        // Handle Rotation
        Vector3 rotationPosition = new Vector3(move.x, 0f, move.z);

        if (rotationPosition != Vector3.zero)
        {
            Quaternion currentRotation = player.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(rotationPosition);
            player.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, player.RotationRate * Time.deltaTime);
        } 
        
    }
}
