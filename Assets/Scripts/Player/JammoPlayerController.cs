using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class JammoPlayerController : MonoBehaviour
{
    private float Health = 100f;
    private float MaxHealth = 100f;
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
    public float speedModifier;
    private float fallAfterJumpThreshold; // Determines when the player begins to fall after jumping.

    [Header("Player Attributes")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpHeight = 12f;
    public float rotationRate = 15.0f;
    public float playerGravity = -9.8f;
    public int crystalCount = 0;
    private float groundedBufferTime = 0.2f;
    private float groundedTimer = 0f;
    CharacterController characterController;
    public Animator animator;
    AnimatorStateInfo animatorStateInfo;
    public ParticleSystem CrystalPickupEffect;
    public ParticleSystem CrystalPlacedEffect;
    public AudioSource FootStepSound;


    void UpdateGroundedStatus()
    {
        if (characterController.isGrounded)
            groundedTimer = groundedBufferTime;
        else
            groundedTimer -= Time.deltaTime;

        bool isActuallyGrounded = groundedTimer > 0f;
        animator.SetBool("isGrounded", isActuallyGrounded);
    }
    void Start()
    {

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        UpdateGroundedStatus();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isFallingHash = Animator.StringToHash("isFalling");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isJumpingHash = Animator.StringToHash("isJumping");

        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        sprintAction = InputSystem.actions.FindAction("Player/Sprint");

        fallAfterJumpThreshold = jumpHeight / 2f; // Increasing the denominator will make the player fall sooner.

        Vector3 initialMove = new Vector3(0f, playerGravity, 0f);
        characterController.Move(initialMove * Time.deltaTime);
    }

    void HandleMovement()
    {
        moveValueInput = moveAction.ReadValue<Vector2>();

        moveValue.x = moveValueInput.x;
        moveValue.z = moveValueInput.y;

        if (characterController.isGrounded && moveValueInput != Vector2.zero)
        {
            if (!FootStepSound.isPlaying)
                FootStepSound.Play();
        }
        else
        {
            if (FootStepSound.isPlaying)
                FootStepSound.Stop();
        }
        if (sprintAction.IsPressed())
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    void HandleAnimation()
    {
        // ---- READ CURRENT MOVEMENT INPUT ----
        bool moving = moveAction.IsPressed();
        bool sprinting = sprintAction.IsPressed();

        animator.SetBool("isWalking", moving && !sprinting && characterController.isGrounded);
        animator.SetBool("isRunning", moving && sprinting && characterController.isGrounded);

        // ---- GROUND CHECK BUFFER ----
        if (characterController.isGrounded)
            groundedTimer = groundedBufferTime;
        else
            groundedTimer -= Time.deltaTime;

        bool isActuallyGrounded = groundedTimer > 0f;

        // ---- JUMP ----
        if (jumpAction.WasPressedThisFrame() && isActuallyGrounded && CheckStateForJump())
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
            animator.SetBool("isGrounded", false);
            return;
        }

        // ---- FALL ----
        if (!isActuallyGrounded && moveValue.y < -0.1f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            animator.SetBool("isGrounded", false);
        }

        // ---- LAND ----
        if (isActuallyGrounded && !wasGroundedLastFrame)
        {
            // Landed this frame
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isGrounded", true);
            // animator.SetTrigger("Land"); // optional
        }

        wasGroundedLastFrame = isActuallyGrounded;
    }


    private bool wasGroundedLastFrame = true;
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
        speedModifier = 1-((float)crystalCount / 10);
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
  UpdateAnimatorSpeed();
        // Handle player input and other calculations
        HandleMovement();
        HandleRotation();
        HandleJumpAndGravity();
        HandleAnimation();

        float finalSpeed =speed * speedModifier;

        // Apply movement
        Vector3 finalMove = new Vector3(moveValue.x * finalSpeed, moveValue.y, moveValue.z * finalSpeed);
        characterController.Move(finalMove * Time.deltaTime);


    }
    public void AddCrystals(int amount)
    {
        ChangeCrystals(amount);
        
        if (CrystalPickupEffect != null)
        {
            CrystalPickupEffect.Play();
        }
        SoundManager.Instance.PlaySound(SoundNames.CrystalPicked, SoundType.Effect);
        UIManager.Instance.ShowToast("Crystal Picked Up. Now Deliver Them to Extraction Point");

    }

    internal void DepositCrystals()
    {
        UIManager.Instance.ShowToast(crystalCount + " Crystals Deposited. Score Gained");
        EventManager.OnScoresChanged?.Invoke((int)crystalCount);
        ChangeCrystals(-crystalCount);
       
        CrystalPlacedEffect.Play();
        SoundManager.Instance.PlaySound(SoundNames.CrystalPlaced, SoundType.Effect);

    }
    public void ChangeCrystals(int amount)
    {
        crystalCount += amount;
    EventManager.OnCrystalChanged?.Invoke(amount);
    }
    public void OnPlayerHit(float HitAmount)
    {
        if (crystalCount > 0)
        {
            ChangeCrystals(-1);
            UIManager.Instance.ShowToast("Player Hit, Crystal Removed", 1f);
            UIManager.Instance.HealthImage.fillAmount = Health / MaxHealth;
        }

    }
    void UpdateAnimatorSpeed()
{
    // Ensures the animator slows down or speeds up based on speedModifier
    // Example: if speedModifier = 0.5, animations play at half speed
    if (animator != null)
    {
        animator.speed = Mathf.Clamp(speedModifier, 0.1f, 1f);
    }
}
}
