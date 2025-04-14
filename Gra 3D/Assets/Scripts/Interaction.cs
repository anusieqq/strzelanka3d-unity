using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Gun gunScript; // Referencja do skryptu Gun

    private int playerHealth = 80; // Pocz�tkowe zdrowie gracza
    private float shieldStrength = 0; // Pocz�tkowa si�a tarczy
    private Coroutine shieldCoroutine;

    public Text ammoText; // Tekst amunicji
    public Text enemyCountText; // Tekst licznika przeciwnik�w
    public Slider healthSlider; // Pasek zdrowia
    public Slider shieldSlider; // Pasek tarczy
    public GameObject gameOverPanel; // Panel ko�ca gry
    public GameObject Jeszczerazbutton;
    public GameObject Wyjd�button;
    public Text Przegra�e�;

    public AudioClip bonusSound;
    private AudioSource audioSource;

    void Start()
    {
        // Ukrywamy panel i tekst na pocz�tku gry
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        

        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        healthSlider.wholeNumbers = false;

        shieldSlider.maxValue = 100;
        shieldSlider.minValue = 0;
        shieldSlider.wholeNumbers = false;
        shieldSlider.value = shieldStrength;

        playerHealth = Mathf.Clamp(playerHealth, 0, 100);
        UpdateHealthSlider();
        UpdateAmmoText();
        UpdateEnemyCount();
    }

    void Update()
    {
        UpdateEnemyCount();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            playerHealth += 10;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            Debug.Log("Health picked up! Current Health: " + playerHealth);
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
                Debug.Log("Ammo picked up! Current Ammo in reserve: " + gunScript.reserveAmmo);
                gunScript.UpdateAmmoText();
                PlayBonusSound();
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("Ammo reserve is full!");
            }
        }
        else if (collision.gameObject.CompareTag("shield"))
        {
            shieldStrength = 100;
            Debug.Log("Shield picked up! Current Shield: " + shieldStrength);
            UpdateShieldSlider();
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
            shieldCoroutine = StartCoroutine(DecreaseShieldOverTime());
            PlayBonusSound();
            Destroy(collision.gameObject);
        }
    }

    void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + gunScript.ammoCount.ToString() + "/" + gunScript.reserveAmmo.ToString();
    }

    void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth;
        Debug.Log("Health Slider Updated: " + healthSlider.value);
    }

    void UpdateShieldSlider()
    {
        shieldSlider.value = shieldStrength;
    }

    public void TakeDamage(int damage)
    {
        if (shieldStrength > 0)
        {
            shieldStrength -= damage;
            shieldStrength = Mathf.Clamp(shieldStrength, 0, 100);
            Debug.Log("Shield absorbed damage! Current Shield: " + shieldStrength);
            UpdateShieldSlider();
        }
        else
        {
            playerHealth -= damage;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            UpdateHealthSlider();
            Debug.Log("Player damaged! Current Health: " + playerHealth);

            if (playerHealth <= 0 && gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Jeszczerazbutton.SetActive(true);
                Wyjd�button.SetActive(true);
                Przegra�e�.gameObject.SetActive(true); // Poka� napis "Przegra�e�"
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Kursor zosta� wy��czony");
                Time.timeScale = 0;
            }
            else
            {
                StartCoroutine(FlashDamagePanel()); // Poka� panel na chwil�
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("BUILDING");
        
    }

    public void QuitGame()
    {
        // Mo�esz tu doda� kod do wyj�cia z gry, np.:
        Application.Quit(); // Zamkni�cie gry
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    IEnumerator FlashDamagePanel()
    {
        // Poka� panel z miganiem
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Jeszczerazbutton.SetActive(false);
            Wyjd�button.SetActive(false);
            Przegra�e�.gameObject.SetActive(false); // Poka� napis "Przegra�e�"
        }

        yield return new WaitForSeconds(0.3f); // Czas trwania efektu migania

        // Ukryj panel, je�li gracz nie zgin��
        if (playerHealth > 0 && gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    IEnumerator DecreaseShieldOverTime()
    {
        while (shieldStrength > 0)
        {
            shieldStrength -= 5;
            shieldStrength = Mathf.Clamp(shieldStrength, 0, 100);
            UpdateShieldSlider();
            Debug.Log("Shield decreasing... Current Shield: " + shieldStrength);
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateEnemyCount()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("ufo").Length;
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
