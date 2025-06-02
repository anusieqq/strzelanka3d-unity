using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class Menu : MonoBehaviour
{
    [Header("Text Animation")]
    public Text textComponent;
    public string fullText;
    public ScrollRect scrollRect;
    private float typingSpeed = 0.005f;
    private float scrollSpeed = 0.065f;
    private float scrollDelay = 1.0f;
    private bool isTyping = false;
    private bool skipRequested = false;

    [Header("UI Elements")]
    public GameObject Rozpocznij;
    public GameObject Pomiń;
    public GameObject StartButton;
    public GameObject Opcje;
    public GameObject Wczytaj;
    public GameObject Wyjdz;
    public Canvas menu;
    public Canvas fabula;
    public Canvas opcjePanel;

    private void Start()
    {
        InitializeUI();
        Pomiń.SetActive(false);
    }

    private void InitializeUI()
    {
        fabula.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        opcjePanel.gameObject.SetActive(false);
        Rozpocznij.SetActive(false);

        Rozpocznij.GetComponent<Button>().onClick.AddListener(StartNewGame);
        StartButton.GetComponent<Button>().onClick.AddListener(StartGame);
        Opcje.GetComponent<Button>().onClick.AddListener(OptionsGame);
        Wczytaj.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoadGameAndScene()));
        Wyjdz.GetComponent<Button>().onClick.AddListener(ExitGame);
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("EnemyCount", 10);
        PlayerPrefs.SetInt("AmmoCount", 30);
        PlayerPrefs.SetInt("ReserveAmmo", 90);
        PlayerPrefs.Save();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("BUILDING");
    }

    public void StartGame()
    {
        StartButton.GetComponent<Button>().interactable = false;

        if (fabula != null)
        {
            fabula.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            skipRequested = false;
            StartCoroutine(TypeText());
            StartCoroutine(ShowSkipButtonAfterDelay());
        }
    }

    IEnumerator ShowSkipButtonAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        Pomiń.SetActive(true);
    }

    IEnumerator LoadGameAndScene()
    {
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "savegame.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("Brak pliku zapisu gry: " + path);
            yield break;
        }

        // Wczytaj dane z pliku JSON
        string json = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(json);
        if (data == null || string.IsNullOrEmpty(data.levelName))
        {
            Debug.LogError("Invalid or missing levelName in savegame.json, loading default scene.");
            SceneManager.LoadScene("BUILDING");
            yield break;
        }

        // Zapisz dane do PlayerPrefs
        PlayerPrefs.SetFloat("PlayerHealth", data.currentHP);
        PlayerPrefs.SetFloat("MaxHealth", data.maxHP);
        PlayerPrefs.SetFloat("PlayerShield", data.currentShield);
        PlayerPrefs.SetFloat("MaxShield", data.maxShield);
        PlayerPrefs.SetFloat("EnemyHealth", data.ufoHP);
        PlayerPrefs.SetFloat("EnemyMaxHP", data.ufoMaxHP);
        PlayerPrefs.SetFloat("PlayerPosX", data.playerPosX);
        PlayerPrefs.SetFloat("PlayerPosY", data.playerPosY);
        PlayerPrefs.SetFloat("PlayerPosZ", data.playerPosZ);
        PlayerPrefs.SetString("SavedScene", data.levelName);
        PlayerPrefs.SetInt("EnemyCount", data.enemyCount);
        PlayerPrefs.SetInt("AmmoCount", data.ammoCount);
        PlayerPrefs.SetInt("ReserveAmmo", data.reserveAmmo);
        PlayerPrefs.SetString("KilledEnemyIds", JsonUtility.ToJson(new Pause.KilledEnemyIdsWrapper { killedEnemyIds = data.killedEnemyIds }));
        PlayerPrefs.SetFloat("GameTimeInMinutes", data.gameTimeInMinutes);
        PlayerPrefs.SetFloat("TimeOfDay", data.timeOfDay);
        PlayerPrefs.SetFloat("TimeMultiplier", data.timeMultiplier);
        PlayerPrefs.Save();

        Debug.Log($"Wczytuję zapis gry dla sceny: {data.levelName}");

        if (data.levelName == "BUILDING")
        {
            // Ładuj BUILDING bezpośrednio
            AsyncOperation asyncLoadBuilding = SceneManager.LoadSceneAsync("BUILDING", LoadSceneMode.Single);
            while (!asyncLoadBuilding.isDone)
            {
                Debug.Log($"Loading BUILDING progress: {asyncLoadBuilding.progress}");
                yield return null;
            }

            // Czekaj na pełne załadowanie sceny
            yield return new WaitForEndOfFrame();

            // Wywołaj LoadPlayerData
            Pause pauseScript = FindObjectOfType<Pause>();
            if (pauseScript != null)
            {
                Debug.Log("Znaleziono Pause.cs na scenie BUILDING, wywołuję LoadPlayerData.");
                pauseScript.LoadPlayerData();
            }
            else
            {
                Debug.LogError("Nie znaleziono obiektu z Pause.cs na scenie BUILDING!");
            }
        }
        else
        {
            // Ładuj BUILDING w tle
            AsyncOperation asyncLoadBuilding = SceneManager.LoadSceneAsync("BUILDING", LoadSceneMode.Additive);
            while (!asyncLoadBuilding.isDone)
            {
                Debug.Log($"Loading BUILDING in background progress: {asyncLoadBuilding.progress}");
                yield return null;
            }

            // Upewnij się, że gracz i kamera są dostępne
            GameObject playerObject = GameObject.FindGameObjectWithTag("player");
            if (playerObject == null)
            {
                Debug.LogWarning("Player not found in BUILDING, creating new player.");
                playerObject = new GameObject("Player");
                playerObject.tag = "player";
                playerObject.AddComponent<PlayerController>();
                DontDestroyOnLoad(playerObject);
            }

            GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObject == null)
            {
                Debug.LogWarning("MainCamera not found in BUILDING, creating new camera.");
                cameraObject = new GameObject("MainCamera");
                cameraObject.tag = "MainCamera";
                cameraObject.AddComponent<Camera>().enabled = true;
                DontDestroyOnLoad(cameraObject);
            }

            // Ładuj docelową scenę
            AsyncOperation asyncLoadTarget = SceneManager.LoadSceneAsync(data.levelName, LoadSceneMode.Single);
            while (!asyncLoadTarget.isDone)
            {
                Debug.Log($"Loading {data.levelName} progress: {asyncLoadTarget.progress}");
                yield return null;
            }

            // Czekaj na pełne załadowanie sceny
            yield return new WaitForEndOfFrame();

            // Upewnij się, że gracz i kamera są nadal dostępne
            playerObject = GameObject.FindGameObjectWithTag("player");
            if (playerObject == null)
            {
                Debug.LogWarning("Player not found after loading target scene, creating new player.");
                playerObject = new GameObject("Player");
                playerObject.tag = "player";
                playerObject.AddComponent<PlayerController>();
                DontDestroyOnLoad(playerObject);
            }

            cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObject == null)
            {
                Debug.LogWarning("MainCamera not found after loading target scene, creating new camera.");
                cameraObject = new GameObject("MainCamera");
                cameraObject.tag = "MainCamera";
                cameraObject.AddComponent<Camera>().enabled = true;
                DontDestroyOnLoad(cameraObject);
            }

            // Wywołaj LoadPlayerData
            Pause pauseScript = FindObjectOfType<Pause>();
            if (pauseScript != null)
            {
                Debug.Log($"Znaleziono Pause.cs na scenie {data.levelName}, wywołuję LoadPlayerData.");
                pauseScript.LoadPlayerData();
            }
            else
            {
                Debug.LogError($"Nie znaleziono obiektu z Pause.cs na scenie {data.levelName}!");
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OptionsGame()
    {
        opcjePanel.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        opcjePanel.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    IEnumerator TypeText()
    {
        if (isTyping) yield break;
        isTyping = true;

        textComponent.text = "";
        float timer = 0f;

        scrollRect.verticalNormalizedPosition = 1f;
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        foreach (char letter in fullText)
        {
            if (skipRequested)
            {
                textComponent.text = fullText;
                break;
            }

            textComponent.text += letter;
            timer += typingSpeed;

            if (scrollRect != null && timer >= scrollDelay)
            {
                timer = 0f;
                yield return SmoothScroll();
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        yield return SmoothScrollToEnd();

        Rozpocznij.SetActive(true);
        Pomiń.SetActive(false);
        isTyping = false;
        skipRequested = false;
    }

    public void SkipTyping()
    {
        Debug.Log("SkipTyping called");
        if (!isTyping) Debug.Log("But not typing now!");
        skipRequested = true;

        Pomiń.GetComponent<Image>().color = Color.red;
        StartCoroutine(ResetButtonColor());
    }

    IEnumerator ResetButtonColor()
    {
        yield return new WaitForSeconds(0.3f);
        Pomiń.GetComponent<Image>().color = Color.white;
    }

    IEnumerator SmoothScroll()
    {
        float targetPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollSpeed, 0f, 1f);
        float duration = 0.3f;
        float elapsedTime = 0f;
        float start = scrollRect.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = targetPosition;
    }

    IEnumerator SmoothScrollToEnd()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        float start = scrollRect.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BUILDING")
        {
            if (Day.Instance != null)
            {
                Day.Instance.ResetTime();
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
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
}