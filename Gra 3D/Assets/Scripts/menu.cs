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
    private float scrollDelay = 0.5f;
    private bool isTyping = false;

    [Header("UI Elements")]
    public GameObject Rozpocznij;
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
        PlayerPrefs.Save();
        SceneManager.LoadScene("BUILDING");
    }

    public void StartGame()
    {
        StartButton.GetComponent<Button>().interactable = false;

        if (fabula != null)
        {
            fabula.gameObject.SetActive(true);
            StartCoroutine(DelayedTypingStart());
            menu.gameObject.SetActive(false);
        }
    }

    IEnumerator LoadGameAndScene()
    {
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "savegame.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);

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

            SceneManager.LoadScene(data.levelName);
        }
        yield return null;
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
        if (isTyping) yield break;
        isTyping = true;

        textComponent.text = "";
        float timer = 0f;

        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            timer += typingSpeed;

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
        isTyping = false;
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
}
