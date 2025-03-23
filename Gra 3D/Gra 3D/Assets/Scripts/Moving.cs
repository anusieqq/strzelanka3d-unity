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

    private Rigidbody rb;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isGrounded;
    private float originalHeight;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Camera playerCamera;
    private Vector3 cameraOffset;
    private float currentSpeed; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = true;
        originalHeight = transform.localScale.y;
        currentSpeed = moveSpeed;
        
        // Pobieranie kamery
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Nie znaleziono kamery! Upewnij się, że jest dzieckiem gracza.");
            return;
        }

        // Ustawienie kamery za postacią
        cameraOffset = new Vector3(0, 2f, -cameraDistance);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (playerCamera == null) return; 

        // Obsługa rotacji myszką
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

        // Ustawienie pozycji i rotacji kamery
        Vector3 desiredPosition = transform.position + Quaternion.Euler(rotationX, rotationY, 0) * cameraOffset;
        playerCamera.transform.position = desiredPosition;
        playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);

       
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

      
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 movement = (right * horizontalInput + forward * verticalInput).normalized * currentSpeed;

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; 
        }

        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = crouchScale;
        }
        else
        {
            transform.localScale = new Vector3(1, originalHeight, 1);
        }

        
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    void OnCollisionEnter(Collision collision)
    {
       
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) 
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
