using UnityEngine;

public class UFOMovement : MonoBehaviour
{
    public float speed = 5f; // Prędkość ruchu UFO
    public float rotationSpeed = 5f; // Prędkość obrotu
    public float detectionRange = 5f; // Zasięg wykrywania gracza
    public float wanderChangeInterval = 3f; // Co ile sekund UFO zmienia losowy kierunek
    public int damage = 10; // Ilość obrażeń zadawanych graczowi
    public float ignorePlayerTime = 2f; // Czas ignorowania gracza po zadaniu obrażeń
    public float raycastDistance = 2f; // Dystans do sprawdzenia przeszkód (ścian)

    private Transform playerTransform;
    private Vector3 moveDirection;
    private bool isChasing = false;
    private float nextDirectionChangeTime = 0f;
    private float ignorePlayerUntil = 0f;

    void Start()
    {
        // Znajdź gracza
        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Losowy początkowy kierunek ruchu
        moveDirection = GetRandomDirection();
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

    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // UFO ignoruje gracza przez pewien czas po zadaniu obrażeń
        if (Time.time < ignorePlayerUntil)
        {
            isChasing = false;
            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }

    void MoveAndRotate()
    {
        if (isChasing)
        {
            // Goni gracza
            moveDirection = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            // Poruszanie się losowe, jeśli nie ściga gracza
            if (Time.time >= nextDirectionChangeTime)
            {
                moveDirection = GetRandomDirection();
                nextDirectionChangeTime = Time.time + wanderChangeInterval;
            }
        }

        // Ruch w aktualnym kierunku
        transform.position += moveDirection * speed * Time.deltaTime;

        // Obrót w stronę kierunku ruchu
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
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

                // UFO ignoruje gracza przez określony czas i porusza się dalej
                ignorePlayerUntil = Time.time + ignorePlayerTime;
                isChasing = false;
                moveDirection = GetRandomDirection();
                nextDirectionChangeTime = Time.time + wanderChangeInterval;
            }
        }
    }

    // Funkcja sprawdzająca kolizje z ścianami za pomocą Raycasta
    void AvoidWalls()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("wall"))
            {
                // Jeśli ściana zostanie wykryta, zmienia kierunek ruchu
                moveDirection = GetRandomDirection();
                Debug.Log("UFO unika ściany, zmiana kierunku");
            }
        }
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
