using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour
{
    public float damage = 10f; // Wartoœæ domyœlna, u¿ywana w TakeDamage(string sourceTag)
    public float range = 2f; // Zasiêg ataku
    public float attackRate = 0.5f; // Czêstotliwoœæ ataku
    private float nextTimeToAttack = 0f;
    public LayerMask enemyLayer; // Warstwa dla wrogów

    private AudioSource audioSource;

    [Header("Input System")]
    public InputActionAsset inputActionAsset; // Pole widoczne w Inspectorze
    private InputAction fireTorchAttackAction;

    void Awake()
    {
        // SprawdŸ, czy InputActionAsset jest przypisany
        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w Torch! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // ZnajdŸ akcjê FireTorchAttack
        fireTorchAttackAction = inputActionAsset.FindAction("Player/Fire Torch Attack");

        if (fireTorchAttackAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/FireTorchAttack w InputActionAsset! SprawdŸ nazwê akcji w pliku InputActionAsset.");
        }
        else
        {
            Debug.Log("Znaleziono akcjê Player/FireTorchAttack w Torch.");
        }

        // SprawdŸ tag
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag != "Torch")
        {
            Debug.LogWarning($"GameObject z Torch ma nieprawid³owy tag: {gameObject.tag}. Ustawiono tag 'Torch'.");
            gameObject.tag = "Torch";
        }

        // Subskrybuj zdarzenie zmiany bindingów
        KeyRebindInput.OnBindingsChanged += RefreshAction;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Brak AudioSource na GameObject z Torch. Dodaj AudioSource, jeœli chcesz odtwarzaæ dŸwiêki.");
        }

        // W³¹cz akcje inputu
        if (inputActionAsset != null)
        {
            inputActionAsset.Enable();
            Debug.Log("InputActionAsset w³¹czone w Torch.");
        }
    }

    void OnDestroy()
    {
        // Wy³¹cz akcje inputu
        if (inputActionAsset != null)
        {
            inputActionAsset.Disable();
            Debug.Log("InputActionAsset wy³¹czone w Torch.");
        }

        // Odsubskrybuj zdarzenie
        KeyRebindInput.OnBindingsChanged -= RefreshAction;
    }

    void Update()
    {
        if (fireTorchAttackAction == null)
        {
            Debug.LogWarning("fireTorchAttackAction jest null w Torch! SprawdŸ konfiguracjê InputActionAsset.");
            return;
        }

        if (fireTorchAttackAction.WasPressedThisFrame() && Time.time >= nextTimeToAttack)
        {
            Debug.Log("Akcja ataku pochodni¹ (FireTorchAttack) wyzwolona w Torch!");
            Attack();
            nextTimeToAttack = Time.time + attackRate;
        }
    }

    void Attack()
    {
        // SprawdŸ warstwê
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("Warstwa enemyLayer nie jest ustawiona w Torch! Ustaw warstwê wrogów w Inspectorze.");
        }

        // ZnajdŸ wszystkich wrogów w zasiêgu
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        Debug.Log($"Torch: Wykryto {hitColliders.Length} colliderów w zasiêgu {range} na warstwie {LayerMask.LayerToName(enemyLayer.value)}");

        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // U¿yj metody TakeDamage z jednym argumentem, aby zadawaæ obra¿enia jak pistolet
                enemy.TakeDamage(gameObject.tag);
                Debug.Log($"Torch: Zadano obra¿enia wrogowi {hitCollider.name} z tagiem {gameObject.tag}");
            }
            else
            {
                Debug.Log($"Torch: Collider {hitCollider.name} nie ma komponentu Enemy.");
            }
        }

        // Odtwórz dŸwiêk ataku
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
                Debug.LogError("Nie mo¿na odœwie¿yæ akcji FireTorchAttack w Torch. SprawdŸ nazwê akcji w InputActionAsset.");
            }
            else
            {
                Debug.Log("Odœwie¿ono akcjê FireTorchAttack w Torch.");
            }
        }
    }
}