using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runModifier = 1.5f;
    [SerializeField] private float rotationRate = 15.0f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;

    private CharacterController controller;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    private InputAction moveInput;
    private InputAction jumpInput;
    private InputAction runInput;
    private InputAction interactInput;

    private Vector3 velocity;
    private Vector2 moveValue;

    private bool jumpPressed;
    private bool runPressed;
    private bool isGrounded;
    private bool canPickup;
    private bool canExtract;
    private bool interactPressed;

    private float nextJumpTime = 0f;
    private float health;

    private static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int isFallingHash = Animator.StringToHash("isFalling");
    private static readonly int isPickingUpHash = Animator.StringToHash("isPickingUp");
    private static readonly int isExtractingHash = Animator.StringToHash("isExtracting");
    private static readonly int isGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int moveSpeedModifierHash = Animator.StringToHash("moveSpeedModifier");

    #region Public getters to access private fields
    public bool IsGrounded => isGrounded;
    public bool JumpPressed => jumpPressed;
    public bool RunPressed => runPressed;
    public bool InteractPressed => interactPressed;
    public bool CanPickup { get => canPickup; set => canPickup = value; }
    public bool CanExtract { get => canExtract; set => canExtract = value; }
    public float CrystalCount { get => crystalCountSO.Value; set => crystalCountSO.Value = value; }
    public float FillCircleAmount => fillCircleAmountSO.Value;
    public float MoveSpeed => moveSpeed;
    public float RunModifier => runModifier;
    public float JumpForce => jumpForce;
    public float JumpCooldown => jumpCooldown;
    public float NextJumpTime { get => nextJumpTime; set => nextJumpTime = value; }
    public float Gravity => gravity;
    public float RotationRate => rotationRate;
    public Vector2 MoveValue => moveValue;
    public Vector3 Velocity => velocity;
    public PlayerStateMachine StateMachine => stateMachine;
    public Transform CameraTransform => cameraTransform;
    public static int IsWalkingHash => isWalkingHash;
    public static int IsRunningHash => isRunningHash;
    public static int MoveSpeedModifierHash => moveSpeedModifierHash;
    public static int IsJumpingHash => isJumpingHash;
    public static int IsFallingHash => isFallingHash;
    public static int IsPickingUpHash => isPickingUpHash;
    public static int IsExtractingHash => isExtractingHash;
    #endregion
    private void Awake()
    {
        if (fillCircleAmountSO == null || crystalCountSO == null || playerHealthSO == null)
        {
            Debug.LogError("One or more ScriptableObjects are not assigned in the inspector.");
            return;
        }
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stateMachine = new PlayerStateMachine();
        animator = GetComponent<Animator>();

        // Initialize states
        var idleState = new IdleState(this);
        var movementState = new MovementState(this);
        var fallingState = new FallingState(this);
        var pickupState = new PickupState(this);
        var extractingState = new ExtractingState(this);

        // Add states to state machine
        stateMachine.AddState("Idle", idleState);
        stateMachine.AddState("Movement", movementState);
        stateMachine.AddState("Falling", fallingState);
        stateMachine.AddState("Pickup", pickupState);
        stateMachine.AddState("Extract", extractingState);

        // Set initial state
        stateMachine.SetState("Idle");

        moveInput = InputSystem.actions.FindAction("Player/Move");
        jumpInput = InputSystem.actions.FindAction("Player/Jump");
        runInput = InputSystem.actions.FindAction("Player/Sprint");
        interactInput = InputSystem.actions.FindAction("Player/Interact");

        playerHealthSO.Value = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMovementInput();
        CheckGroundStatus();
        stateMachine.Update();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        animator.SetBool(isGroundedHash, isGrounded);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
    }
    private void UpdateMovementInput()
    {
        moveValue = moveInput.ReadValue<Vector2>();
        jumpPressed = jumpInput.triggered;
        runPressed = runInput.IsPressed();
        interactPressed = interactInput.IsPressed();
    }

    #region Character Movement Helper Methods
    public void Move(Vector3 velocity, float speed)
    {
        velocity.x = velocity.x * speed;
        velocity.z = velocity.z * speed;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * 2f * -gravity);
    }

    public void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Animation Helper Methods
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
    #endregion

    public void TakeDamage(float damageAmount)
    {
        playerHealthSO.Value -= damageAmount;
        if (playerHealthSO.Value <= 0)
        {
            Die();
        }
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current health: {playerHealthSO.Value}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Implement death logic (e.g., disable GameObject, play death animation)
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        // Chase distance (d1) - Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
