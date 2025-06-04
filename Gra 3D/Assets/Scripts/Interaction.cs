using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Gun gunScript;

    private int playerHealth = 80;

    public Text ammoText;
    public Text enemyCountText;
    public Slider healthSlider;
    public Slider shieldSlider;
    public GameObject gameOverPanel;
    public GameObject Jeszczerazbutton;
    public GameObject Wyjdźbutton;
    public Text Przegrałeś;

    public AudioClip bonusSound;
    private AudioSource audioSource;

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        playerHealth = PlayerPrefs.GetInt("PlayerHealth", 80); // Wczytaj zdrowie z PlayerPrefs
        UpdateHealthSlider();
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        healthSlider.wholeNumbers = true;

        shieldSlider.maxValue = 100;
        shieldSlider.minValue = 0;
        shieldSlider.wholeNumbers = true;

        playerHealth = Mathf.Clamp(playerHealth, 0, 100);
        UpdateHealthSlider();

        // Wczytaj amunicję i liczbę przeciwników
        InitializeAmmoAndEnemyCount();
        UpdateAmmoText();
        UpdateEnemyCount();
    }

    void Update()
    {
        UpdateEnemyCount();
    }

    void InitializeAmmoAndEnemyCount()
    {
        // Wczytaj amunicję z PlayerPrefs
        if (gunScript != null)
        {
            int ammoCount = PlayerPrefs.GetInt("AmmoCount", 30);
            int reserveAmmo = PlayerPrefs.GetInt("ReserveAmmo", 90);
            gunScript.SetAmmo(ammoCount, reserveAmmo);
        }
        else
        {
            Debug.LogError("gunScript nie jest przypisany w skrypcie Interaction!");
        }

        // Liczba przeciwników jest aktualizowana w Pause.cs (AdjustEnemyCount),
        // więc tutaj tylko aktualizujemy UI
        int enemyCount = PlayerPrefs.GetInt("EnemyCount", 10);
        enemyCountText.text = "Enemies: " + enemyCount.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            playerHealth += 10;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            UpdateHealthSlider();
            PlayBonusSound();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ammo"))
        {
            if (gunScript.reserveAmmo < gunScript.maxReserveAmmo)
            {
                int ammoToAdd = 5;
                gunScript.reserveAmmo += ammoToAdd;
                gunScript.reserveAmmo = Mathf.Clamp(gunScript.reserveAmmo, 0, gunScript.maxReserveAmmo);
                UpdateAmmoText();
                PlayBonusSound();
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("shield"))
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                pc.SetShield(100f, pc.maxShield);
            }
            PlayBonusSound();
            Destroy(collision.gameObject);
        }
    }

    void UpdateAmmoText()
    {
        if (gunScript != null)
        {
            ammoText.text = "Ammo: " + gunScript.ammoCount.ToString() + "/" + gunScript.reserveAmmo.ToString();
        }
        else
        {
            Debug.LogError("gunScript nie jest przypisany, nie można zaktualizować tekstu amunicji!");
        }
    }

    void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth;
    }

    public void TakeDamage(int damage)
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc != null && pc.currentShield > 0)
        {
            float remainingDamage = Mathf.Max(0f, damage - pc.currentShield);
            pc.currentShield = Mathf.Max(0f, pc.currentShield - damage);
            shieldSlider.value = pc.currentShield;

            if (remainingDamage > 0)
            {
                playerHealth -= (int)remainingDamage;
                UpdateHealthSlider();
            }
        }
        else
        {
            playerHealth -= damage;
            UpdateHealthSlider();
        }

        playerHealth = Mathf.Clamp(playerHealth, 0, 100);

        if (playerHealth <= 0 && gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Jeszczerazbutton.SetActive(true);
            Wyjdźbutton.SetActive(true);
            Przegrałeś.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            StartCoroutine(FlashDamagePanel());
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("EnemyCount", 10); // Reset do domyślnej liczby przeciwników
        PlayerPrefs.SetInt("AmmoCount", 30);  // Reset amunicji w magazynku
        PlayerPrefs.SetInt("ReserveAmmo", 90); // Reset zapasowej amunicji
        PlayerPrefs.SetInt("PlayerHealth", 80); // Reset zdrowia
        PlayerPrefs.SetString("KilledEnemyIds", "{\"killedEnemyIds\":[]}"); // Reset listy zabitych wrogów
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator FlashDamagePanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Jeszczerazbutton.SetActive(false);
            Wyjdźbutton.SetActive(false);
            Przegrałeś.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);

        if (playerHealth > 0 && gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void UpdateEnemyCount()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemyCountText.text = "Enemies: " + enemyCount.ToString();
    }

    void PlayBonusSound()
    {
        if (bonusSound != null)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
                audioSource.PlayOneShot(bonusSound);
        }
    }
}