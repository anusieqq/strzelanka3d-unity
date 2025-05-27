using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    private PlayerInputActions inputActions;
    private Vector2 movementInput;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey("inputBindings"))
        {
            string rebinds = PlayerPrefs.GetString("inputBindings");
            inputActions.asset.LoadBindingOverridesFromJson(rebinds);
        }

    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.MovingLeft.started += OnMovingLeft;
        inputActions.Player.MovingLeft.canceled += OnMovingLeft;
        inputActions.Player.MovingRight.started += OnMovingRight;
        inputActions.Player.MovingRight.canceled += OnMovingRight;
        inputActions.Player.MovingUp.started += OnMovingUp;
        inputActions.Player.MovingUp.canceled += OnMovingUp;
        inputActions.Player.MovingDown.started += OnMovingDown;
        inputActions.Player.MovingDown.canceled += OnMovingDown;
    }

    void OnDisable()
    {
        inputActions.Player.MovingLeft.started -= OnMovingLeft;
        inputActions.Player.MovingLeft.canceled -= OnMovingLeft;
        inputActions.Player.MovingRight.started -= OnMovingRight;
        inputActions.Player.MovingRight.canceled -= OnMovingRight;
        inputActions.Player.MovingUp.started -= OnMovingUp;
        inputActions.Player.MovingUp.canceled -= OnMovingUp;
        inputActions.Player.MovingDown.started -= OnMovingDown;
        inputActions.Player.MovingDown.canceled -= OnMovingDown;
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.mass = 30f;

        originalHeight = transform.localScale.y;
        currentSpeed = moveSpeed;
        currentStamina = maxStamina;

        if (thirdPersonCamera == null)
            thirdPersonCamera = Camera.main;

        cameraOffset = new Vector3(0, 2f, -cameraDistance);
        Cursor.lockState = CursorLockMode.Locked;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
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

    private void OnMovingLeft(InputAction.CallbackContext context)
    {
        movementInput.x = context.ReadValue<float>() > 0.5f ? -1f : 0f;
        if (movementInput.x == 0 && inputActions.Player.MovingRight.ReadValue<float>() > 0.5f)
            movementInput.x = 1f;
    }

    private void OnMovingRight(InputAction.CallbackContext context)
    {
        movementInput.x = context.ReadValue<float>() > 0.5f ? 1f : 0f;
        if (movementInput.x == 0 && inputActions.Player.MovingLeft.ReadValue<float>() > 0.5f)
            movementInput.x = -1f;
    }

    private void OnMovingUp(InputAction.CallbackContext context)
    {
        movementInput.y = context.ReadValue<float>() > 0.5f ? 1f : 0f;
        if (movementInput.y == 0 && inputActions.Player.MovingDown.ReadValue<float>() > 0.5f)
            movementInput.y = -1f;
    }

    private void OnMovingDown(InputAction.CallbackContext context)
    {
        movementInput.y = context.ReadValue<float>() > 0.5f ? -1f : 0f;
        if (movementInput.y == 0 && inputActions.Player.MovingUp.ReadValue<float>() > 0.5f)
            movementInput.y = 1f;
    }

    void HandleMouseLook()
    {
        Vector2 look = inputActions.Player.Look.ReadValue<Vector2>();
        rotationX -= look.y * mouseSensitivity;
        rotationY += look.x * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -45f, 45f);

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        if (headTransform != null)
            headTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    void HandleMovement()
    {
        Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
        moveDirection.Normalize();

        bool sprintHeld = inputActions.Player.Sprint.ReadValue<float>() > 0.5f;
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
        if (inputActions.Player.Jump.triggered && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleCrouch()
    {
        if (inputActions.Player.Crouch.ReadValue<float>() > 0.5f)
            transform.localScale = crouchScale;
        else
            transform.localScale = new Vector3(1, originalHeight, 1);
    }

    void HandleShooting()
    {
        if (inputActions.Player.Shoot.triggered)
            Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootingPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
                bulletRb.velocity = shootingPoint.forward * bulletSpeed;
        }
    }

    void HandleStamina()
    {
        bool sprintHeld = inputActions.Player.Sprint.ReadValue<float>() > 0.5f;
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
}