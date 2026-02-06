using UnityEngine;

/// <summary>
/// Jednoduchý Player Movement - len WSAD pohyb
/// </summary>
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float stepOffset = 0.5f; // Výška schodu ktorú dokáže prejsť
    [SerializeField] private float slopeLimit = 45f; // Maximálny uhol svahu

    [Header("Stamina Settings")]
    [SerializeField] private float sprintStaminaCost = 15f; // za sekundu
    private bool isSprinting = false;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    private CharacterController controller;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float gravity = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Nastav CharacterController pre schody a svahy
        if (controller != null)
        {
            controller.stepOffset = stepOffset;
            controller.slopeLimit = slopeLimit;
            controller.minMoveDistance = 0f; // Dôležité pre plynulý pohyb
        }

        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator not found! Please assign it in the inspector.");
            }
        }

        // Get camera if not assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("Main Camera not found! Please assign camera in the inspector.");
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
        // Jednoduchá ground check - použitie built-in CharacterController
        isGrounded = controller.isGrounded;

        // Aplikuj gravitáciu
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Malá hodnota pre lepšiu priľnavosť k zemi
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        GetInput();
        HandleSprinting();
        MovePlayer();
        UpdateAnimationParameters();
    }

    private void GetInput()
    {
        // Získaj WSAD input
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D
        verticalInput = Input.GetAxisRaw("Vertical");     // W/S
    }

    private void HandleSprinting()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        bool wantsToSprint = Input.GetKey(sprintKey) && isMoving;

        // Ak chce sprintovať a má staminu
        if (wantsToSprint && playerStats != null && playerStats.currentStamina > 0)
        {
            // Spotrebuj staminu
            if (playerStats.UseStamina(sprintStaminaCost * Time.deltaTime))
            {
                isSprinting = true;
                playerStats.StopStaminaRegen();
            }
            else
            {
                isSprinting = false;
                playerStats.StartStaminaRegen();
            }
        }
        else
        {
            isSprinting = false;
            if (playerStats != null)
            {
                playerStats.StartStaminaRegen();
            }
        }
    }

    private void MovePlayer()
    {
        // Vypočítaj smer pohybu RELATÍVNE KU KAMERE
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignoruj Y os (aby pohyb nebol ovplyvnený vertikálnym uhlom kamery)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Vypočítaj smer pohybu relatívne ku kamere
        moveDirection = forward * verticalInput + right * horizontalInput;

        // Normalizuj ak sa pohybuješ diagonálne (aby nebola väčšia rýchlosť)
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Určí aktuálnu rýchlosť (sprint alebo walk)
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Silnejšia downward sila pre lepšie lezenie po schodoch
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f; // Silnejšia sila pre lepšie držanie na zemi
        }

        // Kombinovaný pohyb - horizontálny + vertikálny (gravitácia)
        Vector3 move = moveDirection * currentSpeed + velocity;
        controller.Move(move * Time.deltaTime);
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null) return;

        // Zistiť či sa hráč pohybuje
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        // Vypočítaj rýchlosť pre animácie
        Vector3 flatVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float speed = flatVel.magnitude;

        // Nastaviť Bool parameters
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", isGrounded);

        // Nastaviť Float parameter pre speed
        animator.SetFloat("Speed", speed);
    }

    // Vizualizácia ground check v editore
    private void OnDrawGizmos()
    {
        if (controller == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}