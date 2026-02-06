using UnityEngine;

/// <summary>
/// Kompletný Player Movement s Animator Parameters a Stamina systémom
/// OPRAVENÁ VERZIA - Synchronizovaný Dash s animáciou + Stamina
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airMultiplier = 0.4f;


    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float gravity = -20f;
    private bool readyToJump = true;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AnimationCurve dashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer;
    private Vector3 dashDirection;

    [Header("Stamina Settings")]
    [SerializeField] private float sprintStaminaCost = 15f; // za sekundu
    [SerializeField] private float dashStaminaCost = 25f; // jednorazovo
    [SerializeField] private float jumpStaminaCost = 10f; // jednorazovo
    private bool isSprinting = false;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float playerRayCastOffset = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftControl;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    private Vector3 velocity;

    // References
    private CharacterController controller;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator not found! Please assign it in the inspector.");
            }
        }

        // Get PlayerStats if not assigned
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found! Please add PlayerStats to the player.");
            }
        }
    }

    private void Update()
    {
        
        // Ground check
        isGrounded = Physics.Raycast(transform.position + Vector3.up * playerRayCastOffset, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        if (isGrounded){
            if (velocity.y < 0 )
            {
                velocity.y = -2f; // Malý negativní velocity pro lepší přilnavost k zemi
            }
        }
        velocity.y += gravity * Time.deltaTime;

        GetInput();
        HandleSprinting();
        MovePlayer();
        UpdateAnimationParameters();

    
        // Handle dash timer
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashTimer = 0f;
            }
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump - kontrola staminy
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            if (playerStats.UseStamina(jumpStaminaCost))
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else
            {
                Debug.Log("Nedostatok staminy na skok!");
            }
        }

        // Dash - kontrola staminy
        if (Input.GetKeyDown(dashKey) && canDash && !isDashing)
        {
            if (playerStats.UseStamina(dashStaminaCost))
            {
                Dash();
            }
            else
            {
                Debug.Log("Nedostatok staminy na dash!");
            }
        }
    }

    private void HandleSprinting()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        bool wantsToSprint = Input.GetKey(sprintKey) && isMoving && isGrounded;

        // Ak chce sprintovať a má staminu
        if (wantsToSprint && playerStats.currentStamina > 0)
        {
            // Spotrebuj staminu
            if (playerStats.UseStamina(sprintStaminaCost * Time.deltaTime))
            {
                isSprinting = true;
                playerStats.StopStaminaRegen(); // Zastav regeneráciu počas sprintu
            }
            else
            {
                // Nedostatok staminy - prestať sprintovať
                isSprinting = false;
                playerStats.StartStaminaRegen();
            }
        }
        else
        {
            // Nie je držaný sprint key alebo sa nehýbe
            isSprinting = false;
            playerStats.StartStaminaRegen();
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        controller.Move((velocity) * Time.deltaTime);
        // Dash movement - synchronizovaný s animáciou
        if (isDashing)
        {
            // Použitie AnimationCurve pre plynulý dash
            float dashProgress = dashTimer / dashDuration;
            float curveValue = dashCurve.Evaluate(dashProgress);

            // Aplikuj dash silu podľa krivky
            Vector3 dashVelocity = dashDirection * dashSpeed * curveValue;
            controller.Move(dashVelocity * Time.deltaTime);
            return;
        }

        // Determine current speed (sprint alebo walk) - len ak má staminu
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // On ground
        if (isGrounded)
        {
            controller.Move((moveDirection) * Time.deltaTime * currentSpeed);
        }

        
        // // In air
        // else
        // {
        //     controller.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
        // }

        // // Apply custom gravity for better jump feel
        // if (!isGrounded)
        // {
        //     controller.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        // }
    }

    // private void SpeedControl()
    // {
    //     if (isDashing) return;

    //     Vector3 flatVel = new Vector3(controller.linearVelocity.x, 0f, controller.linearVelocity.z);

    //     // Limit velocity based on sprint or walk
    //     float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

    //     if (flatVel.magnitude > currentSpeed)
    //     {
    //         Vector3 limitedVel = flatVel.normalized * currentSpeed;
    //         controller.linearVelocity = new Vector3(limitedVel.x, controller.linearVelocity.y, limitedVel.z);
    //     }
    // }

    private void UpdateAnimationParameters()
    {
        if (animator == null) return;

        // Zistiť či sa hráč pohybuje
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        // Calculate speed for blend trees
        Vector3 flatVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float speed = flatVel.magnitude;

        // Nastaviť Bool parameters
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);

        // Nastaviť Float parameter pre speed
        animator.SetFloat("Speed", speed);
    }

    private void Jump()
    {
        // Reset y velocity
        velocity.y = 0f;

        //controller.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        velocity.y += jumpForce;

        // TRIGGER jump animáciu
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Dash()
    {
        if (moveDirection == Vector3.zero) return;

        // Ulož smer dashu na začiatku
        dashDirection = moveDirection.normalized;

        canDash = false;
        isDashing = true;
        dashTimer = 0f;

        // Reset y velocity for consistent dash
        velocity.y = 0f;

        // TRIGGER dash animáciu
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    // Optional: Visualize ground check in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * playerRayCastOffset, transform.position + Vector3.down * (playerHeight * 0.5f + 0.2f));
    }
}