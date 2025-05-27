using UnityEngine;

public class BearMovement : MonoBehaviour
{
    public float baseSpeed = 3f;
    public float chaseSpeed = 6f;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float chaseRange = 3f;
    public float wanderChangeInterval = 3f;
    public int damage = 20;
    public float ignorePlayerTime = 2f;

    public AudioClip bearRoarSound;
    private AudioSource audioSource;

    private Transform playerTransform;
    private Vector3 moveDirection;
    private bool isChasing = false;
    private float nextDirectionChangeTime = 0f;
    private float ignorePlayerUntil = 0f;

    private Rigidbody rb;

    public void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        moveDirection = GetRandomDirection();

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.drag = 2f;

        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            CheckPlayerDistance();
        }

        MoveAndRotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("player"))
        {
            Interaction playerInteraction = collision.collider.GetComponent<Interaction>();
            if (playerInteraction != null)
            {
                playerInteraction.TakeDamage(damage);
                Debug.Log("Niedüwiedü zaatakowa≥ gracza (kolizja)!");

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

    void MoveAndRotate()
    {
        if (isChasing)
        {
            moveDirection = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            if (Time.time >= nextDirectionChangeTime)
            {
                moveDirection = GetRandomDirection();
                nextDirectionChangeTime = Time.time + wanderChangeInterval;
            }
        }

        float speed = isChasing && Vector3.Distance(transform.position, playerTransform.position) <= chaseRange ? chaseSpeed : baseSpeed;
        Vector3 velocity = moveDirection * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // zachowaj oú Y (grawitacja)

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
        } while (newDirection == Vector3.zero);

        return newDirection;
    }

    void PlayAttackSound()
    {
        if (bearRoarSound != null)
        {
            audioSource.PlayOneShot(bearRoarSound);
        }
        else
        {
            Debug.LogWarning("Brak przypisanego düwiÍku ataku niedüwiedzia!");
        }
    }
}
