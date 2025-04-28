using UnityEngine;
using UnityEngine.UI;

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
        audioSource = GetComponent<AudioSource>();  // Sprawdź, czy przypisano komponent AudioSource

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not found on UFO. Adding one.");
            audioSource = gameObject.AddComponent<AudioSource>(); // Dodajemy AudioSource, jeśli go nie ma
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

                // Dźwięk uderzenia
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

        transform.position += moveDirection * speed * Time.deltaTime;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void AvoidWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("wall") || (hit.collider.CompareTag("floor")))
            {
                moveDirection = GetRandomDirection();
                Debug.Log("UFO unika ściany, zmiana kierunku");
            }
        }
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
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
