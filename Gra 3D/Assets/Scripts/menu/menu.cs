using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Animacja tekstu")]
    public TMP_Text textComponent;
    public string fullText;
    public ScrollRect scrollRect;
    private float typingSpeed = 0.005f;
    private float scrollSpeed = 0.065f;
    private float scrollDelay = 1.0f;
    private bool isTyping = false;
    private bool skipRequested = false;

    [Header("Elementy UI")]
    public GameObject Rozpocznij;
    public GameObject Pomiń;
    public GameObject StartButton;
    public GameObject Opcje;
    public GameObject Wczytaj;
    public GameObject Wyjdz;
    public Canvas menu;
    public Canvas fabula;
    public Canvas opcjePanel;

    private AudioManager audioManager;

    private void Start()
    {
        // Inicjalizacja UI przy starcie
        InitializeUI();
        Pomiń.SetActive(false);

        // Znajdź i uruchom muzykę menu
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayMenuMusic(); // Upewnij się, że AudioManager ma tę metodę
        }
        else
        {
            Debug.LogWarning("AudioManager nie znaleziony!");
        }

        // Utwórz PlayerStartPositioner jeśli nie istnieje
        if (FindObjectOfType<PlayerStartPositioner>() == null)
        {
            GameObject manager = new GameObject("PlayerStartPositioner");
            manager.AddComponent<PlayerStartPositioner>();
            Debug.Log("Utworzono PlayerStartPositioner.");
        }
    }

    private void InitializeUI()
    {
        // Ukryj fabułę i panel opcji, pokaż menu
        fabula.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        opcjePanel.gameObject.SetActive(false);
        Rozpocznij.SetActive(false);

        // Przypisz metody do przycisków
        if (Rozpocznij != null) Rozpocznij.GetComponent<Button>().onClick.AddListener(StartNewGame);
        if (StartButton != null) StartButton.GetComponent<Button>().onClick.AddListener(StartGame);
        if (Opcje != null) Opcje.GetComponent<Button>().onClick.AddListener(OptionsGame);
        if (Wczytaj != null) Wczytaj.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoadGameAndScene()));
        if (Wyjdz != null) Wyjdz.GetComponent<Button>().onClick.AddListener(ExitGame);
    }

    public void StartNewGame()
    {
        // Wyczyść zapisane dane i ustaw domyślne wartości
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("EnemyCount", 10);
        PlayerPrefs.SetInt("AmmoCount", 30);
        PlayerPrefs.SetInt("ReserveAmmo", 90);
        PlayerPrefs.SetFloat("PlayerPosX", 14.25f);
        PlayerPrefs.SetFloat("PlayerPosY", 3.8f);
        PlayerPrefs.SetFloat("PlayerPosZ", 68.9f);
        PlayerPrefs.Save();
        Debug.Log($"StartNewGame: Zapisano domyślną pozycję gracza: (14.25, 3.8, 68.9)");

        // Zatrzymaj muzykę menu
        if (audioManager != null)
        {
            audioManager.StopMenuMusic();
        }

        // Ustaw pozycję gracza i załaduj scenę
        PlayerStartPositioner positioner = FindObjectOfType<PlayerStartPositioner>();
        if (positioner != null)
        {
            positioner.PositionPlayerOnSceneLoad("BUILDING");
            Debug.Log("StartNewGame: Wywołano PlayerStartPositioner.");
        }
        else
        {
            SceneManager.LoadScene("BUILDING");
            Debug.LogWarning("StartNewGame: PlayerStartPositioner nie znaleziony, ładowanie sceny bezpośrednio.");
        }
    }

    public void StartGame()
    {
        // Rozpocznij grę - pokaż fabułę
        if (StartButton != null) StartButton.GetComponent<Button>().interactable = false;

        if (fabula != null)
        {
            if (audioManager != null)
            {
                audioManager.StopMenuMusic();
            }
            fabula.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            skipRequested = false;
            StartCoroutine(TypeText());
            StartCoroutine(ShowSkipButtonAfterDelay());
        }
    }

    IEnumerator ShowSkipButtonAfterDelay()
    {
        // Pokazuje przycisk pominięcia po 2 sekundach
        yield return new WaitForSeconds(2f);
        if (Pomiń != null) Pomiń.SetActive(true);
    }

    IEnumerator TypeText()
    {
        // Animacja pisania tekstu
        if (isTyping) yield break;
        isTyping = true;

        textComponent.text = "";
        float timer = 0f;

        scrollRect.verticalNormalizedPosition = 1f;
        yield return new WaitForEndOfFrame();

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

        if (Rozpocznij != null) Rozpocznij.SetActive(true);
        if (Pomiń != null) Pomiń.SetActive(false);
        isTyping = false;
        skipRequested = false;
    }

    public void SkipTyping()
    {
        // Pomija animację tekstu
        skipRequested = true;
        if (Pomiń != null) Pomiń.GetComponent<Image>().color = Color.red;
        StartCoroutine(ResetButtonColor());
    }

    IEnumerator ResetButtonColor()
    {
        // Resetuje kolor przycisku po pominięciu
        yield return new WaitForSeconds(0.3f);
        if (Pomiń != null) Pomiń.GetComponent<Image>().color = Color.white;
    }

    IEnumerator SmoothScroll()
    {
        // Płynne przewijanie tekstu
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
        // Przewija tekst do końca
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

    IEnumerator LoadGameAndScene()
    {
        // Ładuje zapisaną grę
        if (audioManager != null)
        {
            audioManager.StopMenuMusic();
        }

        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "savegame.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("Plik zapisu nie znaleziony: " + path);
            yield break;
        }

        string json = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // Ustaw zapisane wartości w PlayerPrefs
        PlayerPrefs.SetFloat("PlayerHealth", data.currentHP);
        PlayerPrefs.SetFloat("MaxHealth", data.maxHP);
        PlayerPrefs.SetFloat("PlayerShield", data.currentShield);
        PlayerPrefs.SetFloat("MaxShield", data.maxShield);
        PlayerPrefs.SetFloat("PlayerPosX", data.playerPosX);
        PlayerPrefs.SetFloat("PlayerPosY", data.playerPosY);
        PlayerPrefs.SetFloat("PlayerPosZ", data.playerPosZ);
        PlayerPrefs.Save();
        Debug.Log($"LoadGameAndScene: Wczytano dane gry, pozycja: ({data.playerPosX}, {data.playerPosY}, {data.playerPosZ})");

        // Użyj PlayerStartPositioner do ustawienia gracza
        PlayerStartPositioner positioner = FindObjectOfType<PlayerStartPositioner>();
        if (positioner == null)
        {
            GameObject manager = new GameObject("PlayerStartPositioner");
            positioner = manager.AddComponent<PlayerStartPositioner>();
            Debug.Log("LoadGameAndScene: Utworzono nowy PlayerStartPositioner.");
        }
        positioner.PositionPlayerOnSceneLoad(data.levelName);
    }

    public void ExitGame()
    {
        // Wyjście z gry
        if (audioManager != null)
        {
            audioManager.StopMenuMusic();
        }
        Application.Quit();
    }

    public void OptionsGame()
    {
        // Otwiera panel opcji
        opcjePanel.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        if (audioManager != null)
        {
            audioManager.PlayMenuMusic();
        }
    }

    public void BackToMenu()
    {
        // Powrót do menu z panelu opcji
        opcjePanel.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        if (audioManager != null)
        {
            audioManager.PlayMenuMusic();
        }
    }

    [System.Serializable]
    public class GameData
    {
        // Klasa do przechowywania danych zapisu gry
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