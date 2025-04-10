using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Camera thirdPersonCamera;

    [SerializeField] private GameObject bulletPrefab;  // Prefab pocisku
    [SerializeField] private Transform shootingPoint;  // Punkt, z kt�rego strzelamy (np. przed kamer�)
    [SerializeField] private float bulletSpeed = 20f;  // Pr�dko�� pocisku

    private Rigidbody rb;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isGrounded;
    private float originalHeight;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 cameraOffset;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.mass = 2f;
        originalHeight = transform.localScale.y;
        currentSpeed = moveSpeed;

        if (thirdPersonCamera == null)
        {
            thirdPersonCamera = Camera.main;
            if (thirdPersonCamera == null)
            {
                Debug.LogError("Nie znaleziono kamery!");
                return;
            }
        }

        cameraOffset = new Vector3(0, 2f, -cameraDistance);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleCameraPosition();
        HandleShooting();  // Dodajemy obs�ug� strzelania
    }

    void FixedUpdate()
    {
        CheckStairs();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationY += mouseX;
        rotationX = Mathf.Clamp(rotationX, -45f, 45f);

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    void HandleMovement()
    {
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection.Normalize();

        Vector3 velocity = rb.velocity;
        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
        rb.velocity = velocity;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = crouchScale;
        }
        else
        {
            transform.localScale = new Vector3(1, originalHeight, 1);
        }
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
        {
            rb.position += Vector3.up * 0.1f;
        }
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
        {
            thirdPersonCamera.transform.position = hit.point + hit.normal * 0.2f;
        }
        else
        {
            thirdPersonCamera.transform.position = desiredPosition;
        }

        thirdPersonCamera.transform.position = new Vector3(
            thirdPersonCamera.transform.position.x,
            Mathf.Max(thirdPersonCamera.transform.position.y, pivot.y),
            thirdPersonCamera.transform.position.z
        );

        thirdPersonCamera.transform.LookAt(pivot);
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // Lewy przycisk myszy
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootingPoint != null)
        {
            // Tworzymy pocisk
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.velocity = shootingPoint.forward * bulletSpeed;  // Pocisk leci w kierunku kamery
            }
        }
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
