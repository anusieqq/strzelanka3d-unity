using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Statystyki gracza")]
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float currentShield;
    public float maxShield;

    public Slider healthSlider;
    public Slider shieldSlider;

    private float shieldDrainRate = 5f;
    private float shieldRegenDelay = 1f;

    public static PlayerController Instance { get; private set; }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        KeyRebindInput.OnBindingsChanged += OnBindingsChanged;
    }

    private void Start()
    {
        LoadPlayerData();
        StartCoroutine(ShieldDrainCoroutine());
    }

    private void Update()
    {
        Debug.Log($"Player position (Update): {transform.position}");
    }

    private void OnBindingsChanged()
    {
        Debug.Log("Bindings changed, re-enabling InputActions.");
        inputActions.Player.Disable();
        inputActions.Player.Enable();
    }

    public void LoadPlayerData()
    {
        // ZnajdŸ StartPoint
        GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        Vector3 defaultPosition = new Vector3(14.25f, -10.0f, 68.9f);
        Vector3 targetPosition = defaultPosition;

        // SprawdŸ, czy pozycja zosta³a ju¿ ustawiona przez PlayerStartPositioner
        if (PlayerPrefs.GetInt("PositionSetByPositioner", 0) == 1 &&
            SceneManager.GetActiveScene().name == "BUILDING")
        {
            // Jeœli tak, wczytaj tylko statystyki
            currentHP = PlayerPrefs.GetFloat("PlayerHealth", currentHP);
            maxHP = PlayerPrefs.GetFloat("MaxHealth", maxHP);
            currentShield = PlayerPrefs.GetFloat("PlayerShield", currentShield);
            maxShield = PlayerPrefs.GetFloat("MaxShield", maxShield);
            Debug.Log("Pozycja ustawiona przez PlayerStartPositioner, wczytano tylko statystyki.");
            return;
        }

        // Ustaw pozycjê na podstawie StartPointa, jeœli istnieje
        if (startPoint != null)
        {
            targetPosition = startPoint.transform.position;
            Debug.Log($"LoadPlayerData: Znaleziono StartPoint, u¿ywanie pozycji: {targetPosition}");
        }
        else if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            // Wczytaj zapisan¹ pozycjê z PlayerPrefs
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            targetPosition = new Vector3(x, y, z);
            Debug.Log($"LoadPlayerData: Wczytano pozycjê z PlayerPrefs: {targetPosition}");
        }
        else
        {
            Debug.LogWarning($"LoadPlayerData: StartPoint nie znaleziony, u¿ywanie domyœlnej pozycji: {targetPosition}");
        }

        // Ustaw pozycjê z uwzglêdnieniem kolizji z pod³o¿em
        if (Physics.Raycast(targetPosition + Vector3.up * 20f, Vector3.down, out RaycastHit hit, 40f))
        {
            transform.position = hit.point + Vector3.up * 1.5f;
            Debug.Log($"LoadPlayerData: Ustawiono pozycjê na podstawie Raycast: {transform.position}");
        }
        else
        {
            transform.position = targetPosition;
            Debug.Log($"LoadPlayerData: Ustawiono pozycjê bez Raycast: {transform.position}");
        }

        // Wczytaj statystyki
        currentHP = PlayerPrefs.GetFloat("PlayerHealth", currentHP);
        maxHP = PlayerPrefs.GetFloat("MaxHealth", maxHP);
        currentShield = PlayerPrefs.GetFloat("PlayerShield", currentShield);
        maxShield = PlayerPrefs.GetFloat("MaxShield", maxShield);
    }

    public void SavePlayerData()
    {
        Vector3 defaultPosition = new Vector3(14.25f, 3.8f, 68.9f);
        Vector3 safePosition = transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 2f, ~LayerMask.GetMask("Player")))
        {
            safePosition = hit.point + Vector3.up * 0.5f;
            Debug.Log($"Adjusted player position to ground: {safePosition}, hit point: {hit.point}, hit collider: {hit.collider.name}");
        }
        else
        {
            Debug.LogWarning("No ground detected under player, using transform position.");
        }

        if (safePosition.magnitude > 500f || float.IsNaN(safePosition.x) || float.IsNaN(safePosition.y) || float.IsNaN(safePosition.z) ||
            Mathf.Abs(safePosition.y - defaultPosition.y) > 10f)
        {
            Debug.LogWarning($"Invalid player position {safePosition}, using default position: {defaultPosition}");
            safePosition = defaultPosition;
        }

        PlayerPrefs.SetFloat("PlayerPosX", safePosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", safePosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", safePosition.z);
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

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
            inputActions.Dispose();
        }
        KeyRebindInput.OnBindingsChanged -= OnBindingsChanged;
    }
}