using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class JammoPlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Vector3 moveValue;
    Vector2 moveValueInput;

    private int isWalkingHash;
    private int isRunningHash;

    private bool isPlayerGrounded;

    [Header("Player Attributes")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpHeight = 3f;
    public float rotationRate = 15.0f;
    public float playerGravity = -9.8f;

    CharacterController characterController;
    Animator animator;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        sprintAction = InputSystem.actions.FindAction("Player/Sprint");
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

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

            if (jumpAction.IsPressed())
            {
                moveValue.y = jumpHeight;
            }
        }
        else
        {
            moveValue.y += playerGravity * Time.deltaTime;
            if (moveValue.y < 0.01f && moveValue.y > -0.01f)
            {
                Debug.Log(moveValue.y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded && moveValue.y < 0)
        {
            moveValue.y = 0f;
        }

        moveValueInput = moveAction.ReadValue<Vector2>();
        
        moveValue.x = moveValueInput.x;
        moveValue.z = moveValueInput.y;

        HandleJumpAndGravity();
        HandleRotation();
        HandleAnimation();

        if (sprintAction.IsPressed())
        {
            characterController.Move(moveValue * runSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(moveValue * walkSpeed * Time.deltaTime);
        }
    }
}
