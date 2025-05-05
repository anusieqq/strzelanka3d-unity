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

    private Transform playerTransform;
    private Vector3 moveDirection;
    private bool isChasing = false;
    private float nextDirectionChangeTime = 0f;
    private float ignorePlayerUntil = 0f;

    public AudioClip ufoAttackSound;
    private AudioSource audioSource;

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

        // Dodaj Rigidbody, jeśli go nie ma
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Dodaj Collider, jeśli go nie ma
        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.5f;
        }
    }

    void Update()
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

        // Kolizja przed ruchem (ściany)
        if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("wall") || hit.collider.CompareTag("floor"))
            {
                // Ślizganie się po ścianie
                moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;

                // Delikatnie odepchnij UFO od ściany
                transform.position += hit.normal * 0.05f;
            }
        }

        // Sprawdzenie podłogi pod UFO
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("floor"))
            {
                float desiredHeight = hit.point.y + 0.5f; // dostosuj 0.5f jeśli UFO ma inną wysokość
                if (transform.position.y < desiredHeight)
                {
                    transform.position = new Vector3(transform.position.x, desiredHeight, transform.position.z);
                }
            }
        }

        // Sprawdzenie sufitu nad UFO
        if (Physics.Raycast(transform.position, Vector3.up, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("floor"))
            {
                float ceilingLimit = hit.point.y - 0.5f; // odstęp od sufitu
                if (transform.position.y > ceilingLimit)
                {
                    transform.position = new Vector3(transform.position.x, ceilingLimit, transform.position.z);
                }
            }
        }
    }


    void MoveAndRotate()
    {
        Vector3 targetDirection;

        if (isChasing)
        {
            targetDirection = (playerTransform.position - transform.position).normalized;

            // Jeśli coś blokuje ruch, spróbuj przesunąć się wzdłuż przeszkody
            if (Physics.Raycast(transform.position, targetDirection, out RaycastHit hit, raycastDistance))
            {
                if (hit.collider.CompareTag("wall"))
                {
                    // Ślizganie się po ścianie
                    moveDirection = Vector3.ProjectOnPlane(targetDirection, hit.normal).normalized;

                    // Delikatnie odepchnij UFO od ściany, by nie utknęło
                    transform.position += hit.normal * 0.05f;
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

        // Ruch
        transform.position += moveDirection * speed * Time.deltaTime;

        // Rotacja
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        }
        while (Physics.Raycast(transform.position, newDirection, raycastDistance));

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
