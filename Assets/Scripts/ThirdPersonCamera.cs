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

    [Header("Camera Position")]
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float shoulderOffset = 0.5f; // Offset doprava ako v PUBG

    [Header("Camera Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.1f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Collision")]
    [SerializeField] private float collisionOffset = 0.3f;
    [SerializeField] private LayerMask collisionLayers;

    private float rotationY = 0f;
    private float rotationX = 0f;

    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    private float currentDistance;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDistance = cameraDistance;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleZoom();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdatePlayerRotation();
    }

    private void HandleMouseInput()
    {
        // ZÌskaj mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rot·cia kamery
        rotationY += mouseX;
        rotationX -= mouseY;

        // Limit vertik·lnej rot·cie (nemÙûeö sa pozrieù ˙plne hore/dolu)
        rotationX = Mathf.Clamp(rotationX, -35f, 70f);

        // Aplikuj rot·ciu na kameru a orientation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        orientation.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    private void HandleZoom()
    {
        // Scroll wheel zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scrollInput * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);
    }

    private void UpdateCameraPosition()
    {
        // VypoËÌtaj z·kladn˙ target pozÌciu
        Vector3 targetDir = transform.rotation * new Vector3(shoulderOffset, 0f, -currentDistance);
        Vector3 targetPos = player.position + Vector3.up * cameraHeight + targetDir;

        // Collision detection - raycast od hr·Ëa ku kamere
        RaycastHit hit;
        Vector3 directionToCamera = targetPos - (player.position + Vector3.up * cameraHeight);

        if (Physics.Raycast(player.position + Vector3.up * cameraHeight, directionToCamera.normalized,
            out hit, currentDistance, collisionLayers))
        {
            // Ak nieËo blokuje kameru, posuÚ ju bliûöie
            targetPos = hit.point - directionToCamera.normalized * collisionOffset;
        }

        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, positionSmoothTime);
    }

    private void UpdatePlayerRotation()
    {
        // Ot·Ëaj player model smerom kde sa h˝be
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