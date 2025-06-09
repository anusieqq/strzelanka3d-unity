using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Moving : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Camera thirdPersonCamera;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float bulletSpeed = 20f;

    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 0.5f;
    [SerializeField] private Slider staminaSlider;

    [Header("Input System")]
    public InputActionAsset inputActions; // Referencja do InputActionAsset
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private InputAction shootAction;

    private Rigidbody rb;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isGrounded;
    private float originalHeight;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 cameraOffset;
    private float currentSpeed;
    private float currentStamina;
    private bool isSprinting;
    private Vector2 movementInput;

    private void Awake()
    {
        // SprawdŸ, czy InputActionAsset jest przypisany
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w Moving! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // ZnajdŸ akcje
        upAction = inputActions.FindAction("Player/Moving Up");
        downAction = inputActions.FindAction("Player/Moving Down");
        leftAction = inputActions.FindAction("Player/Moving Left");
        rightAction = inputActions.FindAction("Player/Moving Right");
        lookAction = inputActions.FindAction("Player/Look");
        jumpAction = inputActions.FindAction("Player/Jump");
        crouchAction = inputActions.FindAction("Player/Crouch");
        sprintAction = inputActions.FindAction("Player/Sprint");
        shootAction = inputActions.FindAction("Player/Shoot");

        // SprawdŸ, czy akcje zosta³y znalezione
        if (upAction == null || downAction == null || leftAction == null || rightAction == null ||
            lookAction == null || jumpAction == null || crouchAction == null || sprintAction == null || shootAction == null)
        {
            Debug.LogError("Nie znaleziono jednej lub wiêcej akcji w InputActionAsset! SprawdŸ nazwy akcji (Moving Up, Moving Down, Moving Left, Moving Right, Look, Jump, Crouch, Sprint, Shoot).");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Resetuj kamerê
        if (thirdPersonCamera == null)
        {
            thirdPersonCamera = Camera.main;
            if (thirdPersonCamera == null)
            {
                Debug.LogWarning("Nie znaleziono kamery w scenie: " + scene.name);
            }
            else
            {
                Debug.Log("Moving: Camera updated to " + thirdPersonCamera.name);
            }
        }

        // W³¹cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w³¹czone po za³adowaniu sceny: " + scene.name);
        }

        // SprawdŸ headTransform
        if (headTransform == null)
        {
            Debug.LogWarning("headTransform nie jest przypisany w Moving! Upewnij siê, ¿e jest ustawiony w Inspectorze.");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.mass = 30f;

        originalHeight = transform.localScale.y;
        currentSpeed = moveSpeed;
        currentStamina = maxStamina;

        if (thirdPersonCamera == null)
        {
            thirdPersonCamera = Camera.main;
            if (thirdPersonCamera == null)
            {
                Debug.LogWarning("Brak kamery przypisanej w thirdPersonCamera i Camera.main nie znaleziono.");
            }
        }

        if (headTransform == null)
        {
            Debug.LogWarning("headTransform nie jest przypisany w Moving! Upewnij siê, ¿e jest ustawiony w Inspectorze.");
        }

        cameraOffset = new Vector3(0, 2f, -cameraDistance);
        Cursor.lockState = CursorLockMode.Locked;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        // W³¹cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w³¹czone w Moving.");
        }
    }

    private void OnDestroy()
    {
        // Wy³¹cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("InputActions wy³¹czone w Moving.");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleCameraPosition();
        HandleShooting();
        HandleStamina();
    }

    void FixedUpdate()
    {
        CheckStairs();
    }

    void HandleMouseLook()
    {
        if (lookAction == null)
        {
            Debug.LogError("lookAction jest null w HandleMouseLook!");
            return;
        }

        if (!lookAction.enabled)
        {
            Debug.LogWarning("lookAction nie jest w³¹czone! W³¹czanie...");
            lookAction.Enable();
        }

        Vector2 look = lookAction.ReadValue<Vector2>();
        if (look != Vector2.zero)
        {
            Debug.Log("Look Input: " + look); // Log dla debugowania
        }

        rotationX -= look.y * mouseSensitivity;
        rotationY += look.x * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -45f, 45f);

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        if (headTransform != null)
            headTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        else
            Debug.LogWarning("headTransform is null in HandleMouseLook!");
    }

    void HandleMovement()
    {
        // Inicjalizuj wektor ruchu
        float horizontal = 0f;
        float vertical = 0f;

        // Odczytaj wartoœci z akcji
        if (upAction != null && upAction.IsPressed()) vertical += 1f;
        if (downAction != null && downAction.IsPressed()) vertical -= 1f;
        if (leftAction != null && leftAction.IsPressed()) horizontal -= 1f;
        if (rightAction != null && rightAction.IsPressed()) horizontal += 1f;

        movementInput = new Vector2(horizontal, vertical).normalized;

        Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
        moveDirection.Normalize();

        bool sprintHeld = sprintAction != null && sprintAction.ReadValue<float>() > 0f;
        bool isMoving = movementInput.magnitude > 0.1f;

        isSprinting = sprintHeld && isMoving && currentStamina > 0;
        currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 velocity = rb.velocity;
        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;

        if (isGrounded)
            velocity.y = -5f;

        rb.velocity = velocity;
    }

    void HandleJump()
    {
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleCrouch()
    {
        if (crouchAction != null && crouchAction.ReadValue<float>() > 0f)
            transform.localScale = crouchScale;
        else
            transform.localScale = new Vector3(1, originalHeight, 1);
    }

    void HandleShooting()
    {
        if (shootAction != null && shootAction.WasPressedThisFrame())
            Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootingPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootingPoint.forward * bulletSpeed;
            }
        }
    }

    void HandleStamina()
    {
        bool sprintHeld = sprintAction != null && sprintAction.ReadValue<float>() > 0f;
        bool isMoving = movementInput.magnitude > 0.1f;

        if (sprintHeld && isMoving && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina);
        }

        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    void HandleCameraPosition()
    {
        if (thirdPersonCamera == null)
        {
            Debug.LogWarning("Brak kamery w thirdPersonCamera, pomijam HandleCameraPosition.");
            return;
        }

        Vector3 pivot = transform.position + Vector3.up * 1.5f;
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 desiredOffset = rotation * cameraOffset;
        Vector3 desiredPosition = pivot + desiredOffset;

        Vector3 direction = (desiredPosition - pivot).normalized;
        float distance = cameraOffset.magnitude;

        if (Physics.Raycast(pivot, direction, out RaycastHit hit, distance))
            thirdPersonCamera.transform.position = hit.point + hit.normal * 0.2f;
        else
            thirdPersonCamera.transform.position = desiredPosition;

        thirdPersonCamera.transform.position = new Vector3(
            thirdPersonCamera.transform.position.x,
            Mathf.Max(thirdPersonCamera.transform.position.y, pivot.y),
            thirdPersonCamera.transform.position.z
        );

        thirdPersonCamera.transform.LookAt(pivot);
    }

    void CheckStairs()
    {
        if (!isGrounded || rb.velocity.magnitude < 0.1f) return;

        float detectionDistance = 0.5f;
        float stepHeight = 0.5f;
        Vector3 moveDirection = transform.forward;

        Vector3 rayStartLow = transform.position + Vector3.up * 0.1f;
        Vector3 rayStartHigh = transform.position + Vector3.up * stepHeight;

        bool lowHit = Physics.Raycast(rayStartLow, moveDirection, detectionDistance, LayerMask.GetMask("Default"));
        bool highHit = Physics.Raycast(rayStartHigh, moveDirection, detectionDistance, LayerMask.GetMask("Default"));

        if (lowHit && !highHit)
            rb.position += Vector3.up * 0.1f;
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public void SetCamera(Camera camera)
    {
        if (camera != null)
        {
            thirdPersonCamera = camera;
            Debug.Log("Kamera przypisana do Moving.cs: " + camera.name);
        }
        else
        {
            Debug.LogError("Próba przypisania null jako kamery w SetCamera!");
        }
    }
}