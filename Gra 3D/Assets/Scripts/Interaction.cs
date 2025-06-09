using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIReferences
{
    public Text ammoText;
    public Text enemyCountText;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Canvas gameOverCanvas;
    public GameObject gameOverPanel;
    public Button retryButton;
    public Button quitButton;
    public TextMeshProUGUI gameOverText;
}

public class Interaction : MonoBehaviour
{
    public static Interaction Instance { get; private set; }

    [Header("Konfiguracja")]
    public Gun gunScript;
    public UIReferences ui;
    public AudioClip bonusSound;
    [SerializeField] private float shieldDepletionRate = 5f; // Ilość punktów tarczy traconej na sekundę

    public int playerHealth = 100;
    private bool isInitialized = false;
    private AudioSource audioSource;
    private Coroutine shieldDepletionCoroutine; // Do zarządzania ubywaniem tarczy

    #region Cykl życia Unity

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePersistentUI();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        playerHealth = PlayerPrefs.GetInt("PlayerHealth", 80);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!isInitialized)
        {
            InitializeGame();
            isInitialized = true;
        }
    }

    private void Update()
    {
        UpdateEnemyCount();
    }

    #endregion

    #region Inicjalizacja

    private void InitializePersistentUI()
    {
        if (ui.gameOverCanvas != null)
        {
            DontDestroyOnLoad(ui.gameOverCanvas.gameObject);

            if (ui.retryButton != null)
                ui.retryButton.onClick.AddListener(RestartGame);

            if (ui.quitButton != null)
                ui.quitButton.onClick.AddListener(QuitGame);

            if (ui.gameOverPanel != null)
            {
                ui.gameOverPanel.SetActive(false);
                SetGameOverElementsActive(false);
            }
        }

        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
            DontDestroyOnLoad(eventSystem.gameObject);
    }

    private void InitializeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (ui.healthSlider != null)
        {
            ui.healthSlider.maxValue = 100;
            ui.healthSlider.minValue = 0;
            ui.healthSlider.wholeNumbers = true;
            ui.healthSlider.value = playerHealth;
        }

        if (ui.shieldSlider != null)
        {
            ui.shieldSlider.maxValue = 100;
            ui.shieldSlider.minValue = 0;
            ui.shieldSlider.wholeNumbers = true;
        }

        if (gunScript != null)
        {
            int ammoCount = PlayerPrefs.GetInt("AmmoCount", 30);
            int reserveAmmo = PlayerPrefs.GetInt("ReserveAmmo", 90);
            gunScript.SetAmmo(ammoCount, reserveAmmo);
            UpdateAmmoText();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateSceneUIReferences();
    }

    private void UpdateSceneUIReferences()
    {
        if (ui.ammoText == null)
            ui.ammoText = GameObject.Find("AmmoText")?.GetComponent<Text>();

        if (ui.enemyCountText == null)
            ui.enemyCountText = GameObject.Find("EnemyCountText")?.GetComponent<Text>();

        if (ui.healthSlider == null)
            ui.healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();

        if (ui.shieldSlider == null)
            ui.shieldSlider = GameObject.Find("ShieldSlider")?.GetComponent<Slider>();

        UpdateAmmoText();
        UpdateEnemyCount();
        UpdateHealthSlider();
    }

    #endregion

    #region Logika gry

    public void TakeDamage(int damage)
    {
        PlayerController pc = PlayerController.Instance;

        if (pc != null && pc.currentShield > 0)
        {
            // Odejmij obrażenia tylko od tarczy
            pc.currentShield = Mathf.Max(0f, pc.currentShield - damage);

            if (ui.shieldSlider != null)
                ui.shieldSlider.value = pc.currentShield;
        }
        else
        {
            // Jeśli nie ma tarczy, redukuj zdrowie
            playerHealth -= damage;
            UpdateHealthSlider();
        }

        playerHealth = Mathf.Clamp(playerHealth, 0, 100);

        if (playerHealth <= 0)
        {
            ShowGameOverScreen();
        }
        else
        {
            StartCoroutine(FlashDamageIndicator());
        }
    }

    private IEnumerator FlashDamageIndicator()
    {
        if (ui.gameOverPanel != null)
        {
            ui.gameOverPanel.SetActive(true);
            SetGameOverElementsActive(false);
            yield return new WaitForSeconds(0.3f);

            if (playerHealth > 0)
                ui.gameOverPanel.SetActive(false);
        }
    }

    private void ShowGameOverScreen()
    {
        if (ui.gameOverPanel != null)
        {
            ui.gameOverPanel.SetActive(true);
            SetGameOverElementsActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    private void SetGameOverElementsActive(bool active)
    {
        if (ui.retryButton != null)
            ui.retryButton.gameObject.SetActive(active);

        if (ui.quitButton != null)
            ui.quitButton.gameObject.SetActive(active);

        if (ui.gameOverText != null)
            ui.gameOverText.gameObject.SetActive(active);
    }

    #endregion

    #region Aktualizacje UI

    private void UpdateAmmoText()
    {
        if (gunScript != null && ui.ammoText != null)
        {
            ui.ammoText.text = $"Amunicja: {gunScript.ammoCount}/{gunScript.reserveAmmo}";
        }
    }

    private void UpdateEnemyCount()
    {
        if (ui.enemyCountText != null)
        {
            int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            ui.enemyCountText.text = $"Wrogowie: {enemyCount}";
        }
    }

    private void UpdateHealthSlider()
    {
        if (ui.healthSlider != null)
        {
            ui.healthSlider.value = playerHealth;
        }
    }

    #endregion

    #region Akcje przycisków

    public void RestartGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.DeleteAll();

        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (obj.scene.buildIndex == -1) // Obiekty DontDestroyOnLoad
            {
                Destroy(obj);
            }
        }

        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Zbieranie przedmiotów

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            playerHealth = Mathf.Clamp(playerHealth + 10, 0, 100);
            UpdateHealthSlider();
            PlaySound(bonusSound);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ammo") && gunScript != null)
        {
            gunScript.reserveAmmo = Mathf.Clamp(gunScript.reserveAmmo + 5, 0, gunScript.maxReserveAmmo);
            UpdateAmmoText();
            PlaySound(bonusSound);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("shield"))
        {
            var pc = PlayerController.Instance;
            if (pc != null)
            {
                pc.SetShield(100f, pc.maxShield);
                if (ui.shieldSlider != null)
                    ui.shieldSlider.value = pc.currentShield;
                PlaySound(bonusSound);
                Destroy(collision.gameObject);
                // Uruchom korutynę do ubywania tarczy
                if (shieldDepletionCoroutine != null)
                    StopCoroutine(shieldDepletionCoroutine);
                shieldDepletionCoroutine = StartCoroutine(DepleteShieldOverTime());
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator DepleteShieldOverTime()
    {
        PlayerController pc = PlayerController.Instance;
        while (pc != null && pc.currentShield > 0)
        {
            pc.currentShield = Mathf.Max(0f, pc.currentShield - shieldDepletionRate * Time.deltaTime);
            if (ui.shieldSlider != null)
                ui.shieldSlider.value = pc.currentShield;
            yield return null; // Czekaj na następną klatkę
        }
    }

    #endregion
}