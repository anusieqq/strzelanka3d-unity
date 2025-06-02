using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Statystyki gracza")]
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float currentShield;
    public float maxShield;

    [Header("Ruch gracza")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 movementInput;

    public Slider healthSlider;
    public Slider shieldSlider;

    private float shieldDrainRate = 5f;
    private float shieldRegenDelay = 1f;

    // Singleton
    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        // Singleton: zapewnij jedn¹ instancjê gracza
        if (Instance == null)
        {
            Instance = this;
            // Stosuj DontDestroyOnLoad tylko w scenach innych ni¿ pierwsza
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Menu")
            {
                DontDestroyOnLoad(gameObject);
                Debug.Log("Player set to DontDestroyOnLoad.");
            }
        }
        else
        {
            Debug.Log($"Destroying duplicate player: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found! Ensure a camera with 'MainCamera' tag exists.");
        }
    }

    private void Start()
    {
        LoadPlayerData();
        StartCoroutine(ShieldDrainCoroutine());
    }

    private void Update()
    {
        GetInput();
        // Debugowanie pozycji gracza w czasie rzeczywistym
        Debug.Log($"Player position (Update): {transform.position}");
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void MovePlayer()
    {
        if (movementInput.magnitude >= 0.1f && mainCamera != null)
        {
            Vector3 moveDirection = mainCamera.transform.TransformDirection(movementInput);
            moveDirection.y = 0f;

            Vector3 newPosition = rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("playerPosX");
            float y = PlayerPrefs.GetFloat("playerPosY");
            float z = PlayerPrefs.GetFloat("playerPosZ");
            Vector3 savedPosition = new Vector3(x, y, z);
            Debug.Log($"Loaded player position from PlayerPrefs: {savedPosition}");

            // Domyœlna pozycja
            Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 1f);

            // Walidacja zapisanej pozycji
            if (savedPosition.magnitude > 1000f || float.IsNaN(savedPosition.x) || float.IsNaN(savedPosition.y) || float.IsNaN(savedPosition.z))
            {
                Debug.LogWarning($"Invalid saved position {savedPosition}, using default position: {defaultPosition}");
                savedPosition = defaultPosition;
            }

            // Sprawdzenie kolizji
            Vector3 rayStart = savedPosition + Vector3.up * 20f;
            Debug.DrawRay(rayStart, Vector3.down * 40f, Color.red, 5f); // Wizualizacja promienia
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 40f, ~LayerMask.GetMask("Player")))
            {
                transform.position = hit.point + Vector3.up * 1.5f; // Zwiêkszony offset
                Debug.Log($"Set player position with collision check: {transform.position}, hit point: {hit.point}, hit collider: {hit.collider.name}");
            }
            else
            {
                transform.position = savedPosition;
                Debug.Log($"No ground detected, set player position: {transform.position}");
            }

            // Reset Rigidbody
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log("Reset player Rigidbody velocity and angular velocity.");
            }
        }
        else
        {
            Debug.Log("No saved position in PlayerPrefs; using default position.");
            transform.position = new Vector3(-50.80672f, 9f, 1f);
        }

        currentHP = PlayerPrefs.GetFloat("PlayerHealth", currentHP);
        maxHP = PlayerPrefs.GetFloat("MaxHealth", maxHP);
        currentShield = PlayerPrefs.GetFloat("PlayerShield", currentShield);
        maxShield = PlayerPrefs.GetFloat("MaxShield", maxShield);

        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
            healthSlider.maxValue = maxHP;
        }
        else
        {
            Debug.LogWarning("healthSlider is not assigned in PlayerController!");
        }

        if (shieldSlider != null)
        {
            shieldSlider.value = currentShield;
            shieldSlider.maxValue = maxShield;
        }
        else
        {
            Debug.LogWarning("shieldSlider is not assigned in PlayerController!");
        }
    }

    public void SavePlayerData()
    {
        // Domyœlna pozycja
        Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 1f);
        Vector3 safePosition = transform.position;

        // Sprawdzenie pozycji wzglêdem pod³ogi
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 2f, ~LayerMask.GetMask("Player")))
        {
            safePosition = hit.point + Vector3.up * 0.5f; // Pozycja na stopy gracza
            Debug.Log($"Adjusted player position to ground: {safePosition}, hit point: {hit.point}, hit collider: {hit.collider.name}");
        }
        else
        {
            Debug.LogWarning("No ground detected under player, using transform position.");
        }

        // Walidacja pozycji przed zapisem
        if (safePosition.magnitude > 500f || float.IsNaN(safePosition.x) || float.IsNaN(safePosition.y) || float.IsNaN(safePosition.z) ||
            Mathf.Abs(safePosition.y - defaultPosition.y) > 10f)
        {
            Debug.LogWarning($"Invalid player position {safePosition}, using default position: {defaultPosition}");
            safePosition = defaultPosition;
        }

        PlayerPrefs.SetFloat("playerPosX", safePosition.x);
        PlayerPrefs.SetFloat("playerPosY", safePosition.y);
        PlayerPrefs.SetFloat("playerPosZ", safePosition.z);
        PlayerPrefs.SetFloat("PlayerHealth", currentHP);
        PlayerPrefs.SetFloat("PlayerShield", currentShield);
        PlayerPrefs.SetFloat("MaxHealth", maxHP);
        PlayerPrefs.SetFloat("MaxShield", maxShield);
        PlayerPrefs.Save();
        Debug.Log($"Saved player data - Pos: {safePosition}, HP: {currentHP}, Shield: {currentShield}");
    }

    private IEnumerator ShieldDrainCoroutine()
    {
        while (true)
        {
            if (currentShield > 0)
            {
                currentShield -= shieldDrainRate * Time.deltaTime;
                currentShield = Mathf.Clamp(currentShield, 0, maxShield);

                if (shieldSlider != null)
                {
                    shieldSlider.value = currentShield;
                }
            }
            yield return new WaitForSeconds(shieldRegenDelay);
        }
    }

    public void SetHealth(float hp, float maxHp)
    {
        currentHP = hp;
        maxHP = maxHp;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = hp;
        }
    }

    public void SetShield(float shield, float maxShieldValue)
    {
        currentShield = shield;
        maxShield = maxShieldValue;

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShieldValue;
            shieldSlider.value = shield;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentShield > 0)
        {
            float leftoverDamage = damage - currentShield;
            currentShield -= damage;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);

            if (shieldSlider != null)
                shieldSlider.value = currentShield;

            if (leftoverDamage > 0)
            {
                currentHP -= leftoverDamage;
                currentHP = Mathf.Clamp(currentHP, 0, maxHP);

                if (healthSlider != null)
                    healthSlider.value = currentHP;
            }
        }
        else
        {
            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if (healthSlider != null)
                healthSlider.value = currentHP;
        }

        Debug.Log($"[PlayerController] HP: {currentHP}, Shield: {currentShield}");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}               