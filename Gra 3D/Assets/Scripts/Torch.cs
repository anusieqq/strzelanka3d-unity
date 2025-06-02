using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour
{
    public float damage = 10f;
    public float range = 2f;
    public float attackRate = 0.5f;
    private float nextTimeToAttack = 0f;
    public LayerMask enemyLayer; // Warstwa dla wrog�w

    private AudioSource audioSource;

    [Header("Input System")]
    public InputActionAsset inputActions; // Referencja do InputActionAsset
    private InputAction attackAction;

    void Awake()
    {
        // Sprawd�, czy InputActionAsset jest przypisany
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w Torch! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // Znajd� akcj� Shoot
        attackAction = inputActions.FindAction("Player/Shoot");

        // Sprawd�, czy akcja zosta�a znaleziona
        if (attackAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Shoot w InputActionAsset! Sprawd� nazw� akcji.");
        }

        // Sprawd�, czy tag jest ustawiony
        if (string.IsNullOrEmpty(gameObject.tag))
        {
            Debug.LogWarning("GameObject z Torch nie ma ustawionego tagu! Ustaw tag 'Torch' w Inspectorze.");
            gameObject.tag = "Torch"; // Domy�lny tag, je�li brak
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Brak AudioSource na GameObject z Torch. Dodaj AudioSource, je�li chcesz odtwarza� d�wi�ki.");
        }

        // W��cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w��czone w Torch.");
        }
    }

    void OnDestroy()
    {
        // Wy��cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("InputActions wy��czone w Torch.");
        }
    }

    void Update()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame() && Time.time >= nextTimeToAttack)
        {
            Debug.Log("Akcja ataku wyzwolona w Torch!");
            Attack();
            nextTimeToAttack = Time.time + attackRate;
        }
        else if (attackAction == null)
        {
            Debug.LogWarning("attackAction jest null w Torch!");
        }
    }

    void Attack()
    {
        // Znajd� wszystkich wrog�w w zasi�gu na okre�lonej warstwie
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        Debug.Log($"Torch: Wykryto {hitColliders.Length} collider�w w zasi�gu {range} na warstwie {LayerMask.LayerToName(enemyLayer.value)}");

        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(gameObject.tag, damage);
                Debug.Log($"Torch: Zadano {damage} obra�e� wrogowi {hitCollider.name} z tagiem {gameObject.tag}");
            }
            else
            {
                Debug.Log($"Torch: Collider {hitCollider.name} nie ma komponentu Enemy");
            }
        }

        // Odtw�rz d�wi�k ataku, je�li AudioSource istnieje
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}