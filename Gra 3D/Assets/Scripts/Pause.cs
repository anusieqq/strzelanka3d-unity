using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Pause : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject PauseCanvas;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI multiplierText;

    private GameObject Opcje;
    private bool isPaused = false;
    private bool isLoading = false;
    private static Dictionary<string, Enemy> allEnemies = new Dictionary<string, Enemy>();

    public bool IsLoading() => isLoading;

    private void Awake()
    {
        allEnemies.Clear();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (!string.IsNullOrEmpty(enemy.enemyId) && !allEnemies.ContainsKey(enemy.enemyId))
            {
                allEnemies.Add(enemy.enemyId, enemy);
                Debug.Log($"Added enemy with ID: {enemy.enemyId} ({enemy.gameObject.name})");
            }
            else if (string.IsNullOrEmpty(enemy.enemyId))
            {
                Debug.LogError($"Enemy {enemy.gameObject.name} has no enemyId set!");
            }
        }
        Debug.Log($"Initialized {allEnemies.Count} enemies at game start.");

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeUI();

        if (AudioManager.Instance != null && AudioManager.Instance.uiOpcje != null)
        {
            Opcje = AudioManager.Instance.uiOpcje;
            Opcje.SetActive(false);
            Debug.Log("Options initialized and set to inactive.");
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance or uiOpcje is not available.");
        }

        if (PlayerPrefs.HasKey("SavedScene") && !isLoading)
        {
            Debug.Log("SavedScene found in PlayerPrefs, loading player data.");
            LoadPlayerData();
        }
        else
        {
            Debug.Log("No saved game or loading in progress, skipping LoadPlayerData.");
        }
    }

    private void InitializeUI()
    {
        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
            Debug.Log("PauseCanvas initialized and set to inactive.");
        }
        else
        {
            Debug.LogError("PauseCanvas is not assigned!");
        }

        if (healthSlider != null)
            healthSlider.value = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        else
            Debug.LogError("healthSlider is not assigned!");

        if (shieldSlider != null)
            shieldSlider.value = PlayerPrefs.GetFloat("PlayerShield", 0f);
        else
            Debug.LogError("shieldSlider is not assigned!");

        if (enemyHealthSlider != null)
            enemyHealthSlider.value = PlayerPrefs.GetFloat("EnemyHealth", 100f);
        else
            Debug.LogError("enemyHealthSlider is not assigned!");

        if (gameTimeText != null)
        {
            float gameTime = PlayerPrefs.GetFloat("GameTimeInMinutes", 6f * 60f);
            gameTimeText.text = FormatTime(gameTime);
            Debug.Log($"Initialized game time: {gameTimeText.text}");
        }
        else
        {
            Debug.LogError("gameTimeText is not assigned!");
        }

        if (multiplierText != null)
        {
            float multiplier = PlayerPrefs.GetFloat("TimeMultiplier", 0.1f);
            multiplierText.text = $"x{multiplier:F1}";
            Debug.Log($"Initialized time multiplier: {multiplier}");
        }
        else
        {
            Debug.LogError("multiplierText is not assigned!");
        }
    }

    public void LoadPlayerData()
    {
        isLoading = true;
        Debug.Log("Started loading player data.");

        float hp = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        float maxHp = PlayerPrefs.GetFloat("MaxHealth", 100f);
        float shield = PlayerPrefs.GetFloat("PlayerShield", 0f);
        float maxShield = PlayerPrefs.GetFloat("MaxShield", 0f);
        int ammoCount = PlayerPrefs.GetInt("AmmoCount", 30);
        int reserveAmmoCount = PlayerPrefs.GetInt("ReserveAmmo", 90);
        int enemyCount = PlayerPrefs.GetInt("EnemyCount", 10);
        float gameTimeInMinutes = PlayerPrefs.GetFloat("GameTimeInMinutes", 6f * 60f);
        float timeOfDay = PlayerPrefs.GetFloat("TimeOfDay", 0.25f);
        float timeMultiplier = PlayerPrefs.GetFloat("TimeMultiplier", 0.1f);

        timeMultiplier = Mathf.Clamp(Mathf.Round(timeMultiplier * 10f) / 10f, 0f, 1f);

        string killedEnemyIdsJson = PlayerPrefs.GetString("KilledEnemyIds", "{\"killedEnemyIds\":[]}");
        string[] killedEnemyIds;
        try
        {
            KilledEnemyIdsWrapper wrapper = JsonUtility.FromJson<KilledEnemyIdsWrapper>(killedEnemyIdsJson);
            killedEnemyIds = wrapper != null ? wrapper.killedEnemyIds : new string[0];
            Debug.Log($"Loaded {killedEnemyIds.Length} killed enemies: {string.Join(", ", killedEnemyIds)}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error deserializing KilledEnemyIds: " + e.Message);
            killedEnemyIds = new string[0];
        }

        // U¿yj singletona PlayerController
        GameObject playerObject = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
        if (playerObject == null)
        {
            Debug.LogWarning("No player found, creating new player.");
            playerObject = new GameObject("Player");
            playerObject.tag = "player";
            playerObject.AddComponent<PlayerController>();
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                DontDestroyOnLoad(playerObject);
            }
        }
        else
        {
            Debug.Log($"Found existing player: {playerObject.name}");
        }

        // ZnajdŸ kamerê
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObject == null)
        {
            Debug.LogWarning("No camera found with tag 'MainCamera', creating new camera.");
            cameraObject = new GameObject("MainCamera");
            cameraObject.tag = "MainCamera";
            Camera tempCamera = cameraObject.AddComponent<Camera>();
            tempCamera.enabled = true;
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                DontDestroyOnLoad(cameraObject);
            }
        }
        else
        {
            Debug.Log($"Found camera: {cameraObject.name}");
            if (cameraObject.GetComponent<Camera>() == null)
            {
                Debug.LogError("MainCamera object has no Camera component!");
                cameraObject.AddComponent<Camera>().enabled = true;
            }
            else if (!cameraObject.GetComponent<Camera>().enabled)
            {
                cameraObject.GetComponent<Camera>().enabled = true;
                Debug.Log("Enabled camera.");
            }
        }

        // Domyœlna pozycja (y: 9)
        Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 21.12359f);
        Vector3 savedPosition = defaultPosition;

        if (PlayerPrefs.HasKey("playerPosX"))
        {
            savedPosition = new Vector3(
                PlayerPrefs.GetFloat("playerPosX", defaultPosition.x),
                PlayerPrefs.GetFloat("playerPosY", defaultPosition.y),
                PlayerPrefs.GetFloat("playerPosZ", defaultPosition.z)
            );
            Debug.Log($"Loaded saved position from PlayerPrefs: {savedPosition}");

            // Walidacja zapisanej pozycji
            if (savedPosition.magnitude > 1000f || float.IsNaN(savedPosition.x) || float.IsNaN(savedPosition.y) || float.IsNaN(savedPosition.z))
            {
                Debug.LogWarning($"Invalid saved position {savedPosition}, using default position: {defaultPosition}");
                savedPosition = defaultPosition;
            }
        }
        else
        {
            Debug.Log("No saved position in PlayerPrefs, using default position.");
        }

        // Sprawdzenie kolizji
        Vector3 rayStart = savedPosition + Vector3.up * 20f;
        Debug.DrawRay(rayStart, Vector3.down * 40f, Color.red, 5f); // Wizualizacja promienia
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 40f, ~LayerMask.GetMask("Player")))
        {
            playerObject.transform.position = hit.point + Vector3.up * 1.5f; // Zwiêkszony offset
            Debug.Log($"Set player position with collision check: {playerObject.transform.position}, hit point: {hit.point}, hit collider: {hit.collider.name}");
        }
        else
        {
            playerObject.transform.position = savedPosition;
            Debug.Log($"Set player position without collision: {playerObject.transform.position}");
        }

        // Reset Rigidbody
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("Reset player Rigidbody velocity and angular velocity.");
        }

        // Ustaw kamerê
        if (cameraObject != null)
        {
            Moving movingScript = playerObject.GetComponent<Moving>();
            if (movingScript != null)
            {
                movingScript.SetCamera(cameraObject.GetComponent<Camera>());
                Debug.Log("Assigned camera to Moving.cs.");
            }
            else
            {
                Debug.LogWarning("Moving.cs not found on player! Setting camera manually.");
                Vector3 cameraOffset = new Vector3(0f, 2f, -5f);
                cameraObject.transform.position = playerObject.transform.position + cameraOffset;
                cameraObject.transform.LookAt(playerObject.transform.position + Vector3.up * 1.5f);
            }
        }

        // Usuñ zabitych wrogów
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Debug.Log($"Found {enemies.Length} enemies on scene before removal.");
        foreach (Enemy enemy in enemies)
        {
            Debug.Log($"Enemy on scene: {enemy.enemyId} ({enemy.gameObject.name})");
            if (killedEnemyIds.Contains(enemy.enemyId))
            {
                Debug.Log($"Removing enemy with ID: {enemy.enemyId} ({enemy.gameObject.name})");
                DestroyImmediate(enemy.gameObject);
            }
        }

        enemies = FindObjectsOfType<Enemy>();
        Debug.Log($"Remaining {enemies.Length} enemies on scene after removal.");

        // Ustaw zdrowie i tarczê
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetHealth(hp, maxHp);
            playerController.SetShield(shield, maxShield);
            Debug.Log($"Set health: {hp}/{maxHp}, shield: {shield}/{maxShield}");
        }
        else
        {
            Debug.LogError("PlayerController not found! Health and shield not updated.");
        }

        // Ustaw amunicjê
        Gun gun = FindObjectOfType<Gun>();
        if (gun != null)
        {
            gun.SetAmmo(ammoCount, reserveAmmoCount);
            Debug.Log($"Set ammo: {ammoCount}, reserve: {reserveAmmoCount}");
        }
        else
        {
            Debug.LogError("Gun not found during data loading!");
        }

        // Ustaw czas gry
        if (Day.Instance != null)
        {
            Day.Instance.gameTimeInMinutes = gameTimeInMinutes;
            Day.Instance.timeOfDay = timeOfDay;
            Day.Instance.gameMinutesPerSecond = timeMultiplier;
            Debug.Log($"Loaded game time: {gameTimeInMinutes} min, sun height: {timeOfDay}, time multiplier: {timeMultiplier}");
        }
        else
        {
            Debug.LogWarning("Day.Instance not found, game time and multiplier not updated.");
        }

        // Zaktualizuj UI
        if (gameTimeText != null)
        {
            gameTimeText.text = FormatTime(gameTimeInMinutes);
            Debug.Log($"Loaded game time in UI: {gameTimeText.text}");
        }
        if (multiplierText != null)
        {
            multiplierText.text = $"x{timeMultiplier:F1}";
            Debug.Log($"Loaded time multiplier in UI: {multiplierText.text}");
        }

        AdjustEnemyCount(enemyCount);

        isLoading = false;
        Debug.Log("Finished loading player data.");
    }

    private void AdjustEnemyCount(int targetCount)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int currentCount = enemies.Length;

        if (currentCount > targetCount)
        {
            for (int i = targetCount; i < currentCount; i++)
            {
                Debug.Log($"Removing excess enemy: {enemies[i].name}");
                Destroy(enemies[i]);
            }
        }
        Debug.Log($"After AdjustEnemyCount: {enemies.Length} enemies on scene.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isLoading)
        {
            Debug.Log($"Escape pressed, isPaused: {isPaused}, isLoading: {isLoading}");
            if (isPaused)
                Resume();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame called.");
        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(true);
            Debug.Log("PauseCanvas set to active.");
            Canvas canvas = PauseCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 10;
                Debug.Log("Canvas sortingOrder set to 10.");
            }
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogError("PauseCanvas is not assigned, cannot open pause panel!");
        }
    }

    public void Resume()
    {
        Debug.Log("Resume called.");
        if (Opcje != null)
        {
            Opcje.SetActive(false);
            Debug.Log("Options disabled.");
        }

        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
            Debug.Log("PauseCanvas disabled.");
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogError("PauseCanvas is not assigned, cannot resume game!");
        }
    }

    public void OptionsGame()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(true);
            PauseCanvas.SetActive(false);
            Debug.Log("Opened options panel.");
        }
        else
        {
            Debug.LogError("Options are not assigned!");
        }
    }

    public void HideOptions()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(false);
            PauseCanvas.SetActive(true);
            Debug.Log("Hid options panel, showed PauseCanvas.");
        }
        else
        {
            Debug.LogError("Options are not assigned!");
        }
    }

    public void SaveGame()
    {
        Debug.Log("SaveGame called.");
        GameData data = new GameData();

        // U¿yj singletona PlayerController
        GameObject player = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
        Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 21.12359f); // y: 9

        if (player != null)
        {
            Vector3 safePosition = player.transform.position;
            // Sprawdzenie pozycji wzglêdem pod³ogi
            if (Physics.Raycast(player.transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 2f, ~LayerMask.GetMask("Player")))
            {
                safePosition = hit.point + Vector3.up * 0.5f; // Pozycja na stopy
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
            data.playerPosX = safePosition.x;
            data.playerPosY = safePosition.y;
            data.playerPosZ = safePosition.z;
            Debug.Log($"Saved player position: {safePosition}, GameObject: {player.name}, Tag: {player.tag}");
        }
        else
        {
            Debug.LogError("Player not found during save! Using default position.");
            data.playerPosX = defaultPosition.x;
            data.playerPosY = defaultPosition.y;
            data.playerPosZ = defaultPosition.z;
        }

        data.levelName = SceneManager.GetActiveScene().name;
        data.currentHP = healthSlider != null ? healthSlider.value : 100f;
        data.maxHP = healthSlider != null ? healthSlider.maxValue : 100f;
        data.currentShield = shieldSlider != null ? shieldSlider.value : 0f;
        data.maxShield = shieldSlider != null ? shieldSlider.maxValue : 0f;
        data.ufoHP = enemyHealthSlider != null ? enemyHealthSlider.value : 100f;
        data.ufoMaxHP = enemyHealthSlider != null ? enemyHealthSlider.maxValue : 100f;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        data.enemyCount = enemies.Length;
        Debug.Log($"Current active enemy count: {data.enemyCount}");

        List<string> killedEnemyIds = new List<string>();
        List<string> activeEnemyIds = new List<string>();

        Enemy[] currentEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in currentEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                activeEnemyIds.Add(enemy.enemyId);
                Debug.Log($"Active enemy: {enemy.enemyId} ({enemy.gameObject.name})");
            }
        }

        foreach (var enemyEntry in allEnemies)
        {
            if (!activeEnemyIds.Contains(enemyEntry.Key))
            {
                killedEnemyIds.Add(enemyEntry.Key);
                Debug.Log($"Killed enemy: {enemyEntry.Key}");
            }
        }
        data.killedEnemyIds = killedEnemyIds.ToArray();

        Gun gun = FindObjectOfType<Gun>();
        data.ammoCount = gun != null ? gun.ammoCount : 30;
        data.reserveAmmo = gun != null ? gun.reserveAmmo : 90;

        if (Day.Instance != null)
        {
            data.gameTimeInMinutes = Day.Instance.gameTimeInMinutes;
            data.timeOfDay = Day.Instance.timeOfDay;
            data.timeMultiplier = Day.Instance.gameMinutesPerSecond;
            Debug.Log($"Saved game time: {data.gameTimeInMinutes} min, sun height: {data.timeOfDay}, multiplier: {data.timeMultiplier}");
        }
        else
        {
            data.gameTimeInMinutes = 6f * 60f;
            data.timeOfDay = 0.25f;
            data.timeMultiplier = 0.1f;
            Debug.LogWarning("Day.Instance not found. Using default values for game time and multiplier.");
        }

        string json = JsonUtility.ToJson(data, true);
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        File.WriteAllText(Path.Combine(desktopPath, "savegame.json"), json);

        PlayerPrefs.SetFloat("PlayerHealth", data.currentHP);
        PlayerPrefs.SetFloat("MaxHealth", data.maxHP);
        PlayerPrefs.SetFloat("PlayerShield", data.currentShield);
        PlayerPrefs.SetFloat("MaxShield", data.maxShield);
        PlayerPrefs.SetFloat("playerPosX", data.playerPosX);
        PlayerPrefs.SetFloat("playerPosY", data.playerPosY);
        PlayerPrefs.SetFloat("playerPosZ", data.playerPosZ);
        PlayerPrefs.SetFloat("EnemyHealth", data.ufoHP);
        PlayerPrefs.SetFloat("EnemyMaxHP", data.ufoMaxHP);
        PlayerPrefs.SetString("SavedScene", data.levelName);
        PlayerPrefs.SetInt("EnemyCount", data.enemyCount);
        PlayerPrefs.SetInt("AmmoCount", data.ammoCount);
        PlayerPrefs.SetInt("ReserveAmmo", data.reserveAmmo);
        PlayerPrefs.SetString("KilledEnemyIds", JsonUtility.ToJson(new KilledEnemyIdsWrapper { killedEnemyIds = data.killedEnemyIds }));
        PlayerPrefs.SetFloat("GameTimeInMinutes", data.gameTimeInMinutes);
        PlayerPrefs.SetFloat("TimeOfDay", data.timeOfDay);
        PlayerPrefs.SetFloat("TimeMultiplier", data.timeMultiplier);
        PlayerPrefs.Save();

        Debug.Log("Game saved. Killed enemies: " + JsonUtility.ToJson(new KilledEnemyIdsWrapper { killedEnemyIds = data.killedEnemyIds }));
    }

    private string FormatTime(float minutes)
    {
        int gameHours = Mathf.FloorToInt(minutes / 60f);
        int gameMinutes = Mathf.FloorToInt(minutes % 60f);
        return $"{gameHours:00}:{gameMinutes:00}";
    }

    [System.Serializable]
    public class GameData
    {
        public float playerPosX;
        public float playerPosY;
        public float playerPosZ;
        public string levelName;
        public float currentHP;
        public float maxHP;
        public float currentShield;
        public float maxShield;
        public float ufoHP;
        public float ufoMaxHP;
        public int enemyCount;
        public int ammoCount;
        public int reserveAmmo;
        public string[] killedEnemyIds;
        public float gameTimeInMinutes;
        public float timeOfDay;
        public float timeMultiplier;
    }

    [System.Serializable]
    public class KilledEnemyIdsWrapper
    {
        public string[] killedEnemyIds;
    }
}