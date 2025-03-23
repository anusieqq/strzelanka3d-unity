using UnityEngine;

public class UFOMovement : MonoBehaviour
{
    public float speed = 5f; // Pr�dko�� ruchu UFO
    public float rotationSpeed = 100f; // Pr�dko�� obrotu
    public float movementRange = 100f; // Zasi�g ruchu
    public int damage = 10; // Ilo�� zadawanego obra�enia
    private Vector3 targetPosition;
    private float timeUntilNextChange;

    void Start()
    {
        SetNewTargetPosition();
        timeUntilNextChange = Random.Range(2f, 5f);
    }

    void Update()
    {
        MoveAndRotate();

        timeUntilNextChange -= Time.deltaTime;
        if (timeUntilNextChange <= 0)
        {
            SetNewTargetPosition();
            timeUntilNextChange = Random.Range(2f, 5f);
        }
    }

    void MoveAndRotate()
    {
        // P�ynny ruch do przodu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // P�ynny obr�t tylko wok� osi Y
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Ignorujemy zmiany w osi Y
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    void SetNewTargetPosition()
    {
        // Losowa pozycja w przestrzeni 2D (tylko XZ)
        float randomX = Random.Range(-movementRange, movementRange);
        float randomZ = Random.Range(-movementRange, movementRange);
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Interaction playerInteraction = other.GetComponent<Interaction>();
            if (playerInteraction != null)
            {
                playerInteraction.TakeDamage(damage);
            }
        }
    }
}
