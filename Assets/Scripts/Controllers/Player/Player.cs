using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float rotationRate = 15.0f;
    [SerializeField] private float runModifier = 1.5f;
    [SerializeField] private Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject crystalCount;

    private CharacterController controller;
    private Animator animator;
    private PlayerStateMachine stateMachine;
    private Vector3 velocity;
    private bool isGrounded;

    private InputAction moveInput;
    private InputAction jumpInput;
    private InputAction sprintInput;
    private Vector2 moveValue;
    private bool jumpPressed;
    private bool sprintPressed;

    private static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int moveSpeedModifierHash = Animator.StringToHash("moveSpeedModifier");

    // Public getters to access private fields
    public bool IsGrounded => isGrounded;
    public bool JumpPressed => jumpPressed;
    public bool SprintPressed => sprintPressed;
    public float MoveSpeed => moveSpeed;
    public float RunModifier => runModifier;
    public float JumpForce => jumpForce;
    public float RotationRate => rotationRate;
    public Vector2 MoveValue => moveValue;
    public Vector3 Velocity => velocity;
    public PlayerStateMachine StateMachine => stateMachine;
    public Transform CameraTransform => cameraTransform;
    public static int IsWalkingHash => isWalkingHash;
    public static int IsRunningHash => isRunningHash;
    public static int MoveSpeedModifierHash => moveSpeedModifierHash;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stateMachine = new PlayerStateMachine();
        animator = GetComponent<Animator>();

        // Initialize states
        var idleState = new IdleState(this);
        var movementState = new MovementState(this);
        //var fallingState = new FallingState(this);

        // Add states to state machine
        stateMachine.AddState("Idle", idleState);
        stateMachine.AddState("Movement", movementState);
        //stateMachine.AddState("Falling", fallingState);

        // Set initial state
        stateMachine.SetState("Idle");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveInput = InputSystem.actions.FindAction("Player/Move");
        jumpInput = InputSystem.actions.FindAction("Player/Jump");
        sprintInput = InputSystem.actions.FindAction("Player/Sprint");
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovementInput();
        CheckGroundStatus();
        stateMachine.Update();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
    }
    private void CheckMovementInput()
    {
        moveValue = moveInput.ReadValue<Vector2>();
        jumpPressed = jumpInput.IsPressed();
        sprintPressed = sprintInput.IsPressed();
    }

    public void OnAnimatorMove()
    {
        SetAnimatorFloat(moveSpeedModifierHash, 1f - (crystalCount.Value * 0.1f)); // Each crystal reduces speed by 10%
        Vector3 velocity = animator.deltaPosition;
        controller.Move(velocity);
    }
    public void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ResetAnimationBools()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }

    public void SetAnimatorBool(int parameterHash, bool value)
    {
        animator.SetBool(parameterHash, value);
    }

    public void SetAnimatorFloat(int parameterHash, float value)
    {
        animator.SetFloat(parameterHash, value);
    }
}
