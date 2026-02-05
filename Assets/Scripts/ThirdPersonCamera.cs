using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerModel;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 300f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool lockVerticalAngle = false; // Zapni pre pevný uhol
    [SerializeField] private float fixedVerticalAngle = 35f; // Pevný uhol kamery (keď je lockVerticalAngle = true)

    [Header("Camera World Limits")]
    [SerializeField] private bool lockMaxXZ = false; // Zapni pre limit max X a max Z (world space)
    [SerializeField] private float maxX = 100f;
    [SerializeField] private float maxZ = 100f;

    [Header("Camera Position")]
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float shoulderOffset = 0.5f; // Offset doprava ako v PUBG

    [Header("Camera Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.1f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Collision")]
    [SerializeField] private float collisionOffset = 0.3f;
    [SerializeField] private LayerMask collisionLayers;

    private float rotationY = 0f;
    private float rotationX = 0f;

    private Vector3 currentVelocity;
    private Vector3 targetPosition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ak je pevný uhol zapnutý, nastav ho hneď na začiatku
        if (lockVerticalAngle)
        {
            rotationX = fixedVerticalAngle;
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdatePlayerRotation();
    }

    private void HandleMouseInput()
    {
        // Získaj mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Horizontálna rotácia (vždy funguje)
        rotationY += mouseX;

        if (lockVerticalAngle)
        {
            // PEVNÝ UHOL - ignoruj vertikálny mouse input
            rotationX = fixedVerticalAngle;
        }
        else
        {
            // NORMÁLNY REŽIM - vertikálny mouse input funguje
            rotationX -= mouseY;
            // Limit vertikálnej rotácie (nemôžeš sa pozrieť úplne hore/dolu)
            rotationX = Mathf.Clamp(rotationX, -35f, 70f);
        }

        // Aplikuj rotáciu na kameru a orientation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        orientation.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    private void UpdateCameraPosition()
    {
        // Vypočítaj základnú target pozíciu
        Vector3 targetDir = transform.rotation * new Vector3(shoulderOffset, 0f, -cameraDistance);
        Vector3 targetPos = player.position + Vector3.up * cameraHeight + targetDir;

        // Collision detection - raycast od hráča ku kamere
        RaycastHit hit;
        Vector3 directionToCamera = targetPos - (player.position + Vector3.up * cameraHeight);

        if (Physics.Raycast(player.position + Vector3.up * cameraHeight, directionToCamera.normalized, out hit, cameraDistance, collisionLayers))
        {
            // Ak niečo blokuje kameru, posuň ju bližšie
            targetPos = hit.point - directionToCamera.normalized * collisionOffset;
        }

        // WORLD LIMITS (max X a max Z)
        if (lockMaxXZ)
        {
            if (targetPos.x > maxX) targetPos.x = maxX;
            if (targetPos.z > maxZ) targetPos.z = maxZ;
        }

        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, positionSmoothTime);
    }

    private void UpdatePlayerRotation()
    {
        // Otáčaj player model smerom kde sa hýbe
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir.normalized);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Odomkni/zamkni kurzor s Escape
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
