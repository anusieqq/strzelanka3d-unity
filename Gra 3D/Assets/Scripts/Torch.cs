using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour
{
    public float damage = 10f; // Warto�� domy�lna, u�ywana w TakeDamage(string sourceTag)
    public float range = 2f; // Zasi�g ataku
    public float attackRate = 0.5f; // Cz�stotliwo�� ataku
    private float nextTimeToAttack = 0f;
    public LayerMask enemyLayer; // Warstwa dla wrog�w

    private AudioSource audioSource;

    [Header("Input System")]
    public InputActionAsset inputActionAsset; // Pole widoczne w Inspectorze
    private InputAction fireTorchAttackAction;

    void Awake()
    {
        // Sprawd�, czy InputActionAsset jest przypisany
        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w Torch! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // Znajd� akcj� FireTorchAttack
        fireTorchAttackAction = inputActionAsset.FindAction("Player/Fire Torch Attack");

        if (fireTorchAttackAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/FireTorchAttack w InputActionAsset! Sprawd� nazw� akcji w pliku InputActionAsset.");
        }
        else
        {
            Debug.Log("Znaleziono akcj� Player/FireTorchAttack w Torch.");
        }

        // Sprawd� tag
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag != "Torch")
        {
            Debug.LogWarning($"GameObject z Torch ma nieprawid�owy tag: {gameObject.tag}. Ustawiono tag 'Torch'.");
            gameObject.tag = "Torch";
        }

        // Subskrybuj zdarzenie zmiany binding�w
        KeyRebindInput.OnBindingsChanged += RefreshAction;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Brak AudioSource na GameObject z Torch. Dodaj AudioSource, je�li chcesz odtwarza� d�wi�ki.");
        }

        // W��cz akcje inputu
        if (inputActionAsset != null)
        {
            inputActionAsset.Enable();
            Debug.Log("InputActionAsset w��czone w Torch.");
        }
    }

    void OnDestroy()
    {
        // Wy��cz akcje inputu
        if (inputActionAsset != null)
        {
            inputActionAsset.Disable();
            Debug.Log("InputActionAsset wy��czone w Torch.");
        }

        // Odsubskrybuj zdarzenie
        KeyRebindInput.OnBindingsChanged -= RefreshAction;
    }

    void Update()
    {
        if (fireTorchAttackAction == null)
        {
            Debug.LogWarning("fireTorchAttackAction jest null w Torch! Sprawd� konfiguracj� InputActionAsset.");
            return;
        }

        if (fireTorchAttackAction.WasPressedThisFrame() && Time.time >= nextTimeToAttack)
        {
            Debug.Log("Akcja ataku pochodni� (FireTorchAttack) wyzwolona w Torch!");
            Attack();
            nextTimeToAttack = Time.time + attackRate;
        }
    }

    void Attack()
    {
        // Sprawd� warstw�
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("Warstwa enemyLayer nie jest ustawiona w Torch! Ustaw warstw� wrog�w w Inspectorze.");
        }

        // Znajd� wszystkich wrog�w w zasi�gu
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        Debug.Log($"Torch: Wykryto {hitColliders.Length} collider�w w zasi�gu {range} na warstwie {LayerMask.LayerToName(enemyLayer.value)}");

        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // U�yj metody TakeDamage z jednym argumentem, aby zadawa� obra�enia jak pistolet
                enemy.TakeDamage(gameObject.tag);
                Debug.Log($"Torch: Zadano obra�enia wrogowi {hitCollider.name} z tagiem {gameObject.tag}");
            }
            else
            {
                Debug.Log($"Torch: Collider {hitCollider.name} nie ma komponentu Enemy.");
            }
        }

        // Odtw�rz d�wi�k ataku
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

    private void RefreshAction()
    {
        if (inputActionAsset != null)
        {
            fireTorchAttackAction = inputActionAsset.FindAction("Player/FireTorchAttack");
            if (fireTorchAttackAction == null)
            {
                Debug.LogError("Nie mo�na od�wie�y� akcji FireTorchAttack w Torch. Sprawd� nazw� akcji w InputActionAsset.");
            }
            else
            {
                Debug.Log("Od�wie�ono akcj� FireTorchAttack w Torch.");
            }
        }
    }
}