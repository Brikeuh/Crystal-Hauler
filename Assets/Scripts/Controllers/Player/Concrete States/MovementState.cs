using UnityEngine;

public class MovementState : PlayerBaseState
{
    private float horizontalMovement;
    private float verticalMovement;
    public MovementState(Player player) : base(player) { }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered Movement State, setting IsWalking to true");
        player.SetAnimatorBool(Player.IsWalkingHash, true);
    }

    public override void UpdateState()
    {
        horizontalMovement = player.MoveValue.x;
        verticalMovement = player.MoveValue.y;

        player.ApplyGravity();

        if (player.SprintPressed)
        {
            player.SetAnimatorBool(Player.IsRunningHash, true);
        }
        else
        {
            player.SetAnimatorBool(Player.IsRunningHash, false);
        }

        HandleMovement();

        if (Mathf.Abs(horizontalMovement) < 0.1f && Mathf.Abs(verticalMovement) < 0.1f && player.IsGrounded)
        {
            player.StateMachine.SetState("Idle");
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }  

    private void HandleMovement()
    {
        // Handle Movement
        Vector3 move = new Vector3(horizontalMovement, 0f, verticalMovement);

        move = Quaternion.AngleAxis(player.CameraTransform.rotation.eulerAngles.y, Vector3.up) * move;

        // Handle Rotation
        Vector3 rotationPosition = new Vector3(move.x, 0f, move.z);

        Quaternion currentRotation = player.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(rotationPosition);
        player.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, player.RotationRate * Time.deltaTime);
    }
}
