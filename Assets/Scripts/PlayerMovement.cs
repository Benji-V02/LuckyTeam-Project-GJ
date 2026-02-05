using UnityEngine;

/// <summary>
/// Kompletn˝ Player Movement s Animator Parameters
/// OPRAVEN¡ VERZIA - Synchronizovan˝ Dash s anim·ciou
/// BEZ isSprinting parametra - len isMoving
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

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftControl;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    // References
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                UnityEngine.Debug.LogWarning("Animator not found! Please assign it in the inspector.");
            }
        }
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        GetInput();
        SpeedControl();
        UpdateAnimationParameters();

        // Handle drag
        if (isGrounded && !isDashing)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

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

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Dash
        if (Input.GetKeyDown(dashKey) && canDash && !isDashing)
        {
            Dash();
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // Dash movement - synchronizovan˝ s anim·ciou
        if (isDashing)
        {
            // Pouûitie AnimationCurve pre plynul˝ dash
            float dashProgress = dashTimer / dashDuration;
            float curveValue = dashCurve.Evaluate(dashProgress);

            // Aplikuj dash silu podæa krivky
            Vector3 dashVelocity = dashDirection * dashSpeed * curveValue;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);
            return;
        }

        // Determine current speed (sprint alebo walk)
        float currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        // On ground
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        }
        // In air
        else
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Apply custom gravity for better jump feel
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        if (isDashing) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limit velocity based on sprint or walk
        float currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null) return;

        // Zistiù Ëi sa hr·Ë pohybuje
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        // Calculate speed for blend trees (ak pouûÌvaö)
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = flatVel.magnitude;

        // Nastaviù Bool parameters (BEZ isSprinting)
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);

        // Nastaviù Float parameter pre speed
        // Animator Controller pouûije t˙to hodnotu na rozlÌöenie walk vs sprint
        animator.SetFloat("Speed", speed);
    }

    private void Jump()
    {
        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        // TRIGGER jump anim·ciu
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

        // Uloû smer dashu na zaËiatku
        dashDirection = moveDirection.normalized;

        canDash = false;
        isDashing = true;
        dashTimer = 0f;

        // Reset y velocity for consistent dash
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // TRIGGER dash anim·ciu
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
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (playerHeight * 0.5f + 0.2f));
    }
}