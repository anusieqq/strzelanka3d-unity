using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class Menu : MonoBehaviour
{
    public Text textComponent;
    public string fullText;
    private float typingSpeed = 0.005f;

    public ScrollRect scrollRect;
    private float scrollSpeed = 0.065f;
    private float scrollDelay = 0.5f;

    public GameObject Rozpocznij;
    public GameObject StartButton;
    public GameObject Opcje;
    public GameObject Wczytaj;
    public GameObject Wyjdź;

    public Canvas menu;
    public Canvas fabuła;
    public Canvas opcjePanel;

   

    public AudioClip typingSound;
    public AudioClip buttonClickSound;
    public AudioClip backgroundMusic;
    public AudioClip gameStartSound;

    private AudioSource audioSource;

    public Slider typingSlider;
    public Slider buttonClickSlider;
    public Slider backgroundMusicSlider;
    public Slider gameStartSlider;

    public Toggle typingMuteToggle;
    public Toggle buttonClickMuteToggle;
    public Toggle backgroundMusicMuteToggle;
    public Toggle gameStartMuteToggle;

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
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("Brak AudioSource!");

        fabuła.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        opcjePanel.gameObject.SetActive(false);

        Rozpocznij.SetActive(false);
        Rozpocznij.GetComponent<Button>().onClick.AddListener(LoadGameScene);
        StartButton.GetComponent<Button>().onClick.AddListener(StartGame);
        Opcje.GetComponent<Button>().onClick.AddListener(OptionsGame);
        Wczytaj.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoadGameAndScene())); // Poprawiona metoda
        Wyjdź.GetComponent<Button>().onClick.AddListener(ExitGame);
    }

    // Poprawiona metoda LoadGameScene
    public void LoadGameScene()
    {
        
        SceneManager.LoadScene("BUILDING");  
    }

    public void StartGame()
    {
        PlaySound(buttonClickSound);

        if (fabuła != null)
        {
            menu.gameObject.SetActive(false);
            fabuła.gameObject.SetActive(true);
            StartCoroutine(DelayedTypingStart());
        }
    }


    IEnumerator LoadGameAndScene()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/savegame.json";
        Debug.Log("Próba wczytania z: " + path);


        if (File.Exists(path))
        {
            Debug.Log("Znaleziono plik!");

            string json = File.ReadAllText(path);
            Debug.Log("Zawartość pliku JSON: " + json);

            GameData data = JsonUtility.FromJson<GameData>(json);

            if (data == null)
            {
                Debug.LogError("Nie udało się zdeserializować JSON!");
                yield break;
            }

            // Zapisz dane tymczasowo w PlayerPrefs – do odczytu po załadowaniu sceny
            PlayerPrefs.SetFloat("PlayerHealth", data.currentHP);
            PlayerPrefs.SetFloat("MaxHealth", data.maxHP);

            PlayerPrefs.SetFloat("PlayerShield", data.currentShield);
            PlayerPrefs.SetFloat("MaxShield", data.maxShield);

            PlayerPrefs.SetFloat("EnemyHealth", data.ufoHP);
            PlayerPrefs.SetFloat("EnemyMaxHealth", data.ufoMaxHP);

            PlayerPrefs.SetFloat("PlayerPosX", data.playerPosX);
            PlayerPrefs.SetFloat("PlayerPosY", data.playerPosY);
            PlayerPrefs.SetFloat("PlayerPosZ", data.playerPosZ);

            PlayerPrefs.Save();

            // Załaduj scenę – dane zostaną odczytane później
            SceneManager.LoadScene(data.levelName);
        }
        else
        {
            Debug.LogWarning("Nie znaleziono pliku zapisu!");
        }

        yield return null;
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OptionsGame()
    {
        opcjePanel.gameObject.SetActive(!opcjePanel.gameObject.activeSelf);
        menu.gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log($"Odtwarzanie dźwięku: {clip.name}");
        }
        else
        {
            Debug.LogWarning("Nie ma przypisanego dźwięku do odtworzenia.");
        }
    }

    IEnumerator DelayedTypingStart()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        scrollRect.verticalNormalizedPosition = 1f;

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";
        float timer = 0f;

        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            timer += typingSpeed;

            // Odtwarzamy dźwięk podczas typowania
            if (typingSound != null)
            {
                PlaySound(typingSound);
            }

            if (scrollRect != null && timer >= scrollDelay)
            {
                timer = 0f;
                StartCoroutine(SmoothScroll());
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        if (scrollRect != null)
        {
            yield return StartCoroutine(SmoothScrollToEnd());
        }

        Rozpocznij.SetActive(true);
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

}
