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
    public TextMeshProUGUI saveConfirmationText;

    private GameObject Opcje;
    private bool isPaused = false;
    private bool isLoading = false;
    private static Dictionary<string, Enemy> allEnemies = new Dictionary<string, Enemy>();
    private AudioManager audioManager;
    public static Pause Instance;

    public bool IsLoading() => isLoading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            allEnemies.Clear();
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                if (!string.IsNullOrEmpty(enemy.enemyId) && !allEnemies.ContainsKey(enemy.enemyId))
                {
                    allEnemies.Add(enemy.enemyId, enemy);
                    Debug.Log($"Dodano wroga z ID: {enemy.enemyId} ({enemy.gameObject.name})");
                }
                else if (string.IsNullOrEmpty(enemy.enemyId))
                {
                    Debug.LogError($"Wróg {enemy.gameObject.name} nie ma ustawionego enemyId!");
                }
            }
            Debug.Log($"Zainicjalizowano {allEnemies.Count} wrogów na starcie gry.");
        }
        else
        {
            Debug.LogWarning("Próba utworzenia kolejnej instancji Pause. Niszczenie duplikatu.");
            Destroy(gameObject);
        }

        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
        }
    }

    private void Start()
    {
        InitializeUI();

        audioManager = AudioManager.Instance;
        if (AudioManager.Instance != null && AudioManager.Instance.optionsPanel != null)
        {
            Opcje = AudioManager.Instance.optionsPanel;
            Opcje.SetActive(false);
            Debug.Log("Opcje zainicjalizowane i ustawione na nieaktywne.");
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance lub optionsPanel nie jest dostêpny.");
        }

        if (PlayerPrefs.HasKey("SavedScene") && !isLoading)
        {
            Debug.Log("Znalezione SavedScene w PlayerPrefs, ³adowanie danych gracza.");
            LoadPlayerData();
        }
        else
        {
            Debug.Log("Brak zapisanego stanu gry lub trwa ³adowanie, pomijanie LoadPlayerData.");
        }
    }

    private void InitializeUI()
    {
        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
            Debug.Log("PauseCanvas zainicjalizowany i ustawiony na nieaktywny.");
        }
        else
        {
            Debug.LogError("PauseCanvas nie jest przypisany!");
        }

        if (healthSlider != null)
            healthSlider.value = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        else
            Debug.LogError("healthSlider nie jest przypisany!");

        if (shieldSlider != null)
            shieldSlider.value = PlayerPrefs.GetFloat("PlayerShield", 0f);
        else
            Debug.LogError("shieldSlider nie jest przypisany!");

        if (enemyHealthSlider != null)
            enemyHealthSlider.value = PlayerPrefs.GetFloat("EnemyHealth", 100f);
        else
            Debug.LogError("enemyHealthSlider nie jest przypisany!");

        if (gameTimeText != null)
        {
            float gameTime = PlayerPrefs.GetFloat("GameTimeInMinutes", 6f * 60f);
            gameTimeText.text = FormatTime(gameTime);
            Debug.Log($"Zainicjalizowano czas gry: {gameTimeText.text}");
        }
        else
        {
            Debug.LogError("gameTimeText nie jest przypisany!");
        }

        if (multiplierText != null)
        {
            float multiplier = PlayerPrefs.GetFloat("TimeMultiplier", 0.1f);
            multiplierText.text = $"x{multiplier:F1}";
            Debug.Log($"Zainicjalizowano mno¿nik czasu: {multiplier}");
        }
        else
        {
            Debug.LogError("multiplierText nie jest przypisany!");
        }

        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(false);
            saveConfirmationText.text = "Gra zapisana";
            Debug.Log("saveConfirmationText zainicjalizowany i ustawiony na nieaktywny.");
        }
        else
        {
            Debug.LogError("saveConfirmationText nie jest przypisany!");
        }
    }

    public void LoadPlayerData()
    {
        isLoading = true;
        Debug.Log("Rozpoczêto ³adowanie danych gracza.");

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
            Debug.Log($"Za³adowano {killedEnemyIds.Length} zabitych wrogów: {string.Join(", ", killedEnemyIds)}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("B³¹d podczas deserializacji KilledEnemyIds: " + e.Message);
            killedEnemyIds = new string[0];
        }

        GameObject playerObject = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
        if (playerObject == null)
        {
            Debug.LogWarning("Nie znaleziono gracza, tworzenie nowego gracza.");
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
            Debug.Log($"Znaleziono istniej¹cego gracza: {playerObject.name}");
        }

        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObject == null)
        {
            Debug.LogWarning("Nie znaleziono kamery z tagiem 'MainCamera', tworzenie nowej kamery.");
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
            Debug.Log($"Znaleziono kamerê: {cameraObject.name}");
            if (cameraObject.GetComponent<Camera>() == null)
            {
                Debug.LogError("Obiekt MainCamera nie ma komponentu Camera!");
                cameraObject.AddComponent<Camera>().enabled = true;
            }
            else if (!cameraObject.GetComponent<Camera>().enabled)
            {
                cameraObject.GetComponent<Camera>().enabled = true;
                Debug.Log("W³¹czono kamerê.");
            }
        }

        Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 21.12359f);
        Vector3 savedPosition = defaultPosition;

        if (PlayerPrefs.HasKey("playerPosX"))
        {
            savedPosition = new Vector3(
                PlayerPrefs.GetFloat("playerPosX", defaultPosition.x),
                PlayerPrefs.GetFloat("playerPosY", defaultPosition.y),
                PlayerPrefs.GetFloat("playerPosZ", defaultPosition.z)
            );
            Debug.Log($"Za³adowano zapisan¹ pozycjê z PlayerPrefs: {savedPosition}");

            if (savedPosition.magnitude > 1000f || float.IsNaN(savedPosition.x) || float.IsNaN(savedPosition.y) || float.IsNaN(savedPosition.z))
            {
                Debug.LogWarning($"Nieprawid³owa zapisana pozycja {savedPosition}, u¿ywanie domyœlnej pozycji: {defaultPosition}");
                savedPosition = defaultPosition;
            }
        }
        else
        {
            Debug.Log("Brak zapisanej pozycji w PlayerPrefs, u¿ywanie domyœlnej pozycji.");
        }

        Vector3 rayStart = savedPosition + Vector3.up * 20f;
        Debug.DrawRay(rayStart, Vector3.down * 40f, Color.red, 5f);
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 40f, ~LayerMask.GetMask("Player")))
        {
            playerObject.transform.position = hit.point + Vector3.up * 1.5f;
            Debug.Log($"Ustawiono pozycjê gracza z kontrol¹ kolizji: {playerObject.transform.position}, punkt trafienia: {hit.point}, trafiony collider: {hit.collider.name}");
        }
        else
        {
            playerObject.transform.position = savedPosition;
            Debug.Log($"Ustawiono pozycjê gracza bez kolizji: {playerObject.transform.position}");
        }

        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("Zresetowano prêdkoœæ i prêdkoœæ k¹tow¹ Rigidbody gracza.");
        }

        if (cameraObject != null)
        {
            Moving movingScript = playerObject.GetComponent<Moving>();
            if (movingScript != null)
            {
                movingScript.SetCamera(cameraObject.GetComponent<Camera>());
                Debug.Log("Przypisano kamerê do Moving.cs: " + cameraObject.name);
            }
            else
            {
                Debug.LogWarning("Moving.cs nie znaleziono na graczu! Ustawianie kamery rêcznie.");
                Vector3 cameraOffset = new Vector3(0f, 2f, -5f);
                cameraObject.transform.position = playerObject.transform.position + cameraOffset;
                cameraObject.transform.LookAt(playerObject.transform.position + Vector3.up * 1.5f);
            }
        }

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Debug.Log($"Znaleziono {enemies.Length} wrogów na scenie przed usuniêciem.");
        foreach (Enemy enemy in enemies)
        {
            Debug.Log($"Wróg na scenie: {enemy.enemyId} ({enemy.gameObject.name})");
            if (killedEnemyIds.Contains(enemy.enemyId))
            {
                Debug.Log($"Usuwanie wroga z ID: {enemy.enemyId} ({enemy.gameObject.name})");
                DestroyImmediate(enemy.gameObject);
            }
        }

        enemies = FindObjectsOfType<Enemy>();
        Debug.Log($"Pozosta³o {enemies.Length} wrogów na scenie po usuniêciu.");

        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetHealth(hp, maxHp);
            playerController.SetShield(shield, maxShield);
            Debug.Log($"Ustawiono zdrowie: {hp}/{maxHp}, tarcza: {shield}/{maxShield}");
        }
        else
        {
            Debug.LogError("PlayerController nie znaleziono! Zdrowie i tarcza nie zaktualizowane.");
        }

        Gun gun = FindObjectOfType<Gun>();
        if (gun != null)
        {
            gun.SetAmmo(ammoCount, reserveAmmoCount);
            Debug.Log($"Ustawiono amunicjê: {ammoCount}, rezerwa: {reserveAmmoCount}");
        }
        else
        {
            Debug.LogError("Gun nie znaleziono podczas ³adowania danych!");
        }

        if (Day.Instance != null)
        {
            Day.Instance.gameTimeInMinutes = gameTimeInMinutes;
            Day.Instance.timeOfDay = timeOfDay;
            Day.Instance.gameMinutesPerSecond = timeMultiplier;
            Debug.Log($"Za³adowano czas gry: {gameTimeInMinutes} min, wysokoœæ s³oñca: {timeOfDay}, mno¿nik czasu: {timeMultiplier}");
        }
        else
        {
            Debug.LogWarning("Day.Instance nie znaleziono, czas gry i mno¿nik nie zaktualizowane.");
        }

        if (gameTimeText != null)
        {
            gameTimeText.text = FormatTime(gameTimeInMinutes);
            Debug.Log($"Za³adowano czas gry w UI: {gameTimeText.text}");
        }
        if (multiplierText != null)
        {
            multiplierText.text = $"x{timeMultiplier:F1}";
            Debug.Log($"Za³adowano mno¿nik czasu w UI: {multiplierText.text}");
        }

        AdjustEnemyCount(enemyCount);

        isLoading = false;
        Debug.Log("Zakoñczono ³adowanie danych gracza.");
    }

    private void AdjustEnemyCount(int targetCount)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int currentCount = enemies.Length;

        if (currentCount > targetCount)
        {
            for (int i = targetCount; i < currentCount; i++)
            {
                Debug.Log($"Usuwanie nadmiarowego wroga: {enemies[i].name}");
                Destroy(enemies[i]);
            }
        }
        Debug.Log($"Po AdjustEnemyCount: {enemies.Length} wrogów na scenie.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isLoading)
        {
            Debug.Log($"Naciœniêto Escape, isPaused: {isPaused}, isLoading: {isLoading}");
            if (isPaused)
                Resume();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Wywo³ano PauseGame.");
        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(true);
            Debug.Log("PauseCanvas ustawiony na aktywny.");
            Canvas canvas = PauseCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 10;
                Debug.Log("Canvas sortingOrder ustawiony na 10.");
            }
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Wy³¹cz rotacjê kamery
            GameObject playerObject = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
            if (playerObject != null)
            {
                Moving movingScript = playerObject.GetComponent<Moving>();
                if (movingScript != null)
                {
                    movingScript.enabled = false; // Wy³¹cz skrypt Moving, aby zatrzymaæ rotacjê kamery
                    Debug.Log("Rotacja kamery wy³¹czona (skrypt Moving wy³¹czony).");
                }
                else
                {
                    Debug.LogWarning("Skrypt Moving nie znaleziony, nie mo¿na wy³¹czyæ rotacji kamery.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerController.Instance nie znaleziony, nie mo¿na wy³¹czyæ rotacji kamery.");
            }
        }
        else
        {
            Debug.LogError("PauseCanvas nie jest przypisany, nie mo¿na otworzyæ panelu pauzy!");
        }
    }

    public void Resume()
    {
        Debug.Log("Wywo³ano Resume.");
        if (Opcje != null)
        {
            Opcje.SetActive(false);
            Debug.Log("Opcje wy³¹czone.");
        }

        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
            Debug.Log("PauseCanvas wy³¹czony.");
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // W³¹cz rotacjê kamery
            GameObject playerObject = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
            if (playerObject != null)
            {
                Moving movingScript = playerObject.GetComponent<Moving>();
                if (movingScript != null)
                {
                    movingScript.enabled = true; // W³¹cz skrypt Moving, aby przywróciæ rotacjê kamery
                    Debug.Log("Rotacja kamery w³¹czona (skrypt Moving w³¹czony).");
                }
                else
                {
                    Debug.LogWarning("Skrypt Moving nie znaleziony, nie mo¿na w³¹czyæ rotacji kamery.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerController.Instance nie znaleziony, nie mo¿na w³¹czyæ rotacji kamery.");
            }
        }
        else
        {
            Debug.LogError("PauseCanvas nie jest przypisany, nie mo¿na wznowiæ gry!");
        }
    }

    public void OptionsGame()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(true);
            PauseCanvas.SetActive(false);
            Debug.Log("Otwarto panel opcji.");
            if (audioManager != null)
            {
                audioManager.StopMusic();
                audioManager.PlayMenuMusic();
                Debug.Log("Zatrzymano bie¿¹c¹ muzykê i odtworzono muzykê menu w panelu opcji.");
            }
        }
        else
        {
            Debug.LogError("Opcje nie s¹ przypisane!");
        }
    }

    public void HideOptions()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(false);
            PauseCanvas.SetActive(true);
            Debug.Log("Ukryto panel opcji, pokazano PauseCanvas.");
            if (audioManager != null)
            {
                audioManager.RestoreSceneMusic();
                Debug.Log("Przywrócono muzykê sceny po zamkniêciu panelu opcji.");
            }
        }
        else
        {
            Debug.LogError("Opcje nie s¹ przypisane!");
        }
    }

    public void SaveGame()
    {
        Debug.Log("Wywo³ano SaveGame.");
        GameData data = new GameData();

        GameObject player = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;
        Vector3 defaultPosition = new Vector3(-50.80672f, 9f, 21.12359f);

        if (player != null)
        {
            Vector3 safePosition = player.transform.position;
            if (Physics.Raycast(player.transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 2f, ~LayerMask.GetMask("Player")))
            {
                safePosition = hit.point + Vector3.up * 0.5f;
                Debug.Log($"Dostosowano pozycjê gracza do pod³o¿a: {safePosition}, punkt trafienia: {hit.point}, trafiony collider: {hit.collider.name}");
            }
            else
            {
                Debug.LogWarning("Nie wykryto pod³o¿a pod graczem, u¿ywanie pozycji transform.");
            }

            if (safePosition.magnitude > 500f || float.IsNaN(safePosition.x) || float.IsNaN(safePosition.y) || float.IsNaN(safePosition.z))
            {
                Debug.LogWarning($"Nieprawid³owa pozycja gracza {safePosition}, u¿ywanie domyœlnej pozycji: {defaultPosition}");
                safePosition = defaultPosition;
            }
            data.playerPosX = safePosition.x;
            data.playerPosY = safePosition.y;
            data.playerPosZ = safePosition.z;
            Debug.Log($"Zapisano pozycjê gracza: {safePosition}, GameObject: {player.name}, Tag: {player.tag}");
        }
        else
        {
            Debug.LogError("Gracz nie znaleziony podczas zapisu! U¿ywanie domyœlnej pozycji.");
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
        Debug.Log($"Aktualna liczba aktywnych wrogów: {data.enemyCount}");

        List<string> killedEnemyIds = new List<string>();
        List<string> activeEnemyIds = new List<string>();

        Enemy[] currentEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in currentEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                activeEnemyIds.Add(enemy.enemyId);
                Debug.Log($"Aktywny wróg: {enemy.enemyId} ({enemy.gameObject.name})");
            }
        }

        foreach (var enemyEntry in allEnemies)
        {
            if (!activeEnemyIds.Contains(enemyEntry.Key))
            {
                killedEnemyIds.Add(enemyEntry.Key);
                Debug.Log($"Zabity wróg: {enemyEntry.Key}");
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
            Debug.Log($"Zapisano czas gry: {data.gameTimeInMinutes} min, wysokoœæ s³oñca: {data.timeOfDay}, mno¿nik: {data.timeMultiplier}");
        }
        else
        {
            data.gameTimeInMinutes = 6f * 60f;
            data.timeOfDay = 0.25f;
            data.timeMultiplier = 0.1f;
            Debug.LogWarning("Day.Instance nie znaleziono. U¿ywanie domyœlnych wartoœci dla czasu gry i mno¿nika.");
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

        Debug.Log("Gra zapisana. Zabici wrogowie: " + JsonUtility.ToJson(new KilledEnemyIdsWrapper { killedEnemyIds = data.killedEnemyIds }));

        // Wyœwietl komunikat potwierdzenia zapisu
        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(true);
            StartCoroutine(HideSaveConfirmationText());
            Debug.Log("Wyœwietlono komunikat potwierdzenia zapisu.");
        }
        else
        {
            Debug.LogError("saveConfirmationText nie jest przypisany, nie mo¿na wyœwietliæ komunikatu potwierdzenia!");
        }
    }

    private System.Collections.IEnumerator HideSaveConfirmationText()
    {
        yield return new WaitForSecondsRealtime(2f); // U¿ywamy WaitForSecondsRealtime, poniewa¿ gra jest w trybie pauzy (Time.timeScale = 0)
        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(false);
            Debug.Log("Ukryto komunikat potwierdzenia zapisu.");
        }
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