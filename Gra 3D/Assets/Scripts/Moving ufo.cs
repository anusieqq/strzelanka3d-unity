using UnityEngine;

public class UFOMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float detectionRange = 5f;
    public float wanderChangeInterval = 3f;
    public int damage = 10;
    public float ignorePlayerTime = 2f;
    public float raycastDistance = 2f;

    public float minY;
    public float maxY;

    public AudioClip ufoAttackSound;
    private AudioSource audioSource;

    private Transform playerTransform;
    private Vector3 moveDirection;
    private bool isChasing = false;
    private float nextDirectionChangeTime = 0f;
    private float ignorePlayerUntil = 0f;

    private Rigidbody rb;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        moveDirection = GetRandomDirection();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // ⬇️ Fizyczny ruch
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.drag = 4f; // tłumienie ślizgania

        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.5f;
        }

        // Warstwa - minY / maxY
        string layerName = LayerMask.LayerToName(gameObject.layer);

        if (layerName == "Parter")
        {
            minY = 2.0f;
            maxY = 6.0f;
        }
        else if (layerName == "FirstFloor")
        {
            minY = 8.0f;
            maxY = 13.0f;
        }
        else
        {
            Debug.LogWarning($"UFO ({gameObject.name}) ma nieznaną warstwę ({layerName}), używam domyślnych limitów.");
            minY = 0.5f;
            maxY = 5.0f;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            CheckPlayerDistance();
        }

        AvoidWalls();
        MoveAndRotate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Interaction playerInteraction = other.GetComponent<Interaction>();
            if (playerInteraction != null)
            {
                playerInteraction.TakeDamage(damage);
                Debug.Log("UFO zadało obrażenia graczowi!");

                PlayAttackSound();

                ignorePlayerUntil = Time.time + ignorePlayerTime;
                isChasing = false;
                moveDirection = GetRandomDirection();
                nextDirectionChangeTime = Time.time + wanderChangeInterval;
            }
        }
    }

    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (Time.time < ignorePlayerUntil)
        {
            isChasing = false;
            return;
        }
        isChasing = distanceToPlayer <= detectionRange;
    }

    void AvoidWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("wall") || hit.collider.CompareTag("floor"))
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
            }
        }
    }

    void MoveAndRotate()
    {
        Vector3 targetDirection;

        if (isChasing)
        {
            targetDirection = (playerTransform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, targetDirection, out RaycastHit hit, raycastDistance))
            {
                if (hit.collider.CompareTag("wall"))
                {
                    moveDirection = Vector3.ProjectOnPlane(targetDirection, hit.normal).normalized;
                }
                else
                {
                    moveDirection = targetDirection;
                }
            }
            else
            {
                moveDirection = targetDirection;
            }
        }
        else
        {
            if (Time.time >= nextDirectionChangeTime)
            {
                moveDirection = GetRandomDirection();
                nextDirectionChangeTime = Time.time + wanderChangeInterval;
            }
        }

        // Ruch fizyczny
        Vector3 velocity = moveDirection * speed;
        rb.velocity = new Vector3(velocity.x, 0f, velocity.z);

        // Ograniczenie wysokości
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        if (Mathf.Abs(transform.position.y - clampedY) > 0.01f)
        {
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }

        // Rotacja
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    Vector3 GetRandomDirection()
    {
        Vector3 newDirection;
        int attempts = 0;
        do
        {
            newDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            attempts++;
            if (attempts > 10)
            {
                newDirection = -moveDirection;
                break;
            }
        } while (Physics.Raycast(transform.position, newDirection, raycastDistance));

        return newDirection;
    }

    void PlayAttackSound()
    {
        if (ufoAttackSound != null)
        {
            audioSource.PlayOneShot(ufoAttackSound);
        }
        else
        {
            Debug.LogWarning("UFO attack sound not assigned!");
        }
    }
}
