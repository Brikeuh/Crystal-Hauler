using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class JammoPlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction interactAction;

    Vector3 moveValue;
    Vector2 moveValueInput;

    private int isWalkingHash;
    private int isRunningHash;
    private int isFallingHash;
    private int isGroundedHash;
    private int isJumpingHash;

    private float speed;
    private float speedModifier;
    private float fallAfterJumpThreshold; // Determines when the player begins to fall after jumping.

    [Header("Player Attributes")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpHeight = 12f;
    public float rotationRate = 15.0f;
    public float playerGravity = -9.8f;
    public float crystalCount = 0f;

    [Header("External Object References")]
    public Transform cameraTransform;

    CharacterController characterController;
    Animator animator;
    AnimatorStateInfo animatorStateInfo;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isFallingHash = Animator.StringToHash("isFalling");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isJumpingHash = Animator.StringToHash("isJumping");

        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        sprintAction = InputSystem.actions.FindAction("Player/Sprint");

        fallAfterJumpThreshold = jumpHeight/2f; // Increasing the denominator will make the player fall sooner.

        Vector3 initialMove = new Vector3(0f, playerGravity, 0f);
        characterController.Move(initialMove * Time.deltaTime);
    }

    void HandleMovement()
    {
        moveValueInput = moveAction.ReadValue<Vector2>();

        moveValue.x = moveValueInput.x;
        moveValue.z = moveValueInput.y;

        if (sprintAction.IsPressed())
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        moveValue = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveValue;
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isFalling = animator.GetBool(isFallingHash);
        bool isGrounded = animator.GetBool(isGroundedHash);
        bool isJumping = animator.GetBool(isJumpingHash);

        if (moveAction.IsPressed() && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (!moveAction.IsPressed() && isWalking)
        {
            animator.SetBool("isWalking", false);
        }

        if ((moveAction.IsPressed() && sprintAction.IsPressed()) && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if ((!moveAction.IsPressed() || !sprintAction.IsPressed()) && isRunning)
        {
            animator.SetBool("isRunning", false);
        }

        if (moveValue.y > fallAfterJumpThreshold) // Jumping on the way up
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isGrounded", false);
        }
        else if (moveValue.y < fallAfterJumpThreshold && !characterController.isGrounded) // Falling after jump on the way down
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isGrounded", false);
        }
        else if (characterController.isGrounded) // After jump, hitting the ground
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isGrounded", true);
        }

        if (jumpAction.IsPressed() && characterController.isGrounded && CheckStateForJump())
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    void HandleRotation()
    {
        Vector3 rotationPosition;

        rotationPosition.x = moveValue.x;
        rotationPosition.y = 0.0f;
        rotationPosition.z = moveValue.z;

        Quaternion currentRotation = transform.rotation;

        if (moveAction.IsPressed())
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotationPosition);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationRate * Time.deltaTime);
        }
    }

    void HandleJumpAndGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -0.05f;
            moveValue.y = groundedGravity;

            if (jumpAction.IsPressed() && CheckStateForJump())
            {
                moveValue.y = jumpHeight - (jumpHeight * speedModifier);
            }
        }
        else
        {
            moveValue.y += playerGravity * Time.deltaTime;
        }
    }

    // Helper function that checks if the animator is in the Run, Walk, or Idle states before allowing the player to jump.
    bool CheckStateForJump()
    {
        return animatorStateInfo.IsName("Base Layer.Run") || animatorStateInfo.IsName("Base Layer.Walk") || animatorStateInfo.IsName("Base Layer.Idle");
    }

    // Update is called once per frame
    void Update()
    {
        speedModifier = crystalCount / 10;
        animator.SetFloat("moveSpeedModifier", 1-speedModifier);
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Handle player input and other calculations
        HandleMovement();
        HandleRotation();
        HandleJumpAndGravity();
        HandleAnimation();

        float finalSpeed = speed - (speed * speedModifier);

        // Apply movement
        Vector3 finalMove = new Vector3(moveValue.x * finalSpeed, moveValue.y, moveValue.z * finalSpeed);
        characterController.Move(finalMove * Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState= CursorLockMode.None;
        }
    }
}
