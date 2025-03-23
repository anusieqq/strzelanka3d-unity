using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Do obs�ugi UI (Text, Slider)

public class Interaction : MonoBehaviour
{
    // Zmienne do przechowywania stanu gracza
    private int playerHealth = 80; // Pocz�tkowe zdrowie gracza
    private int ammoCount = 10; // Na starcie gracz ma 10 amunicji
    private float shieldStrength = 0;
    private Coroutine shieldCoroutine;

    // Referencje do komponent�w UI
    public Text ammoText; // Tekst amunicji
    public Slider healthSlider; // Pasek zdrowia
    public Slider shieldSlider; // Pasek tarczy

    // Start jest wywo�ywane na pocz�tku gry
    void Start()
    {
        // Ustawienie maksymalnych warto�ci pask�w
        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        healthSlider.wholeNumbers = false;

        shieldSlider.maxValue = 100;
        shieldSlider.minValue = 0;
        shieldSlider.wholeNumbers = false;
        shieldSlider.value = shieldStrength;

        // Pocz�tkowe ustawienie zdrowia i paska zdrowia
        playerHealth = Mathf.Clamp(playerHealth, 0, 100);
        UpdateHealthSlider();

        // Wy�wietlenie pocz�tkowej ilo�ci amunicji w UI
        UpdateAmmoText();
    }

    // Funkcja wywo�ywana przy kolizji z innymi obiektami
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            // Dodaj zdrowie graczowi
            playerHealth += 10;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            Debug.Log("Health picked up! Current Health: " + playerHealth);

            // Zaktualizuj pasek �ycia
            UpdateHealthSlider();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ammo"))
        {
            // Dodaj amunicj�
            ammoCount += 5;
            Debug.Log("Ammo picked up! Current Ammo: " + ammoCount);

            // Zaktualizuj tekst na ekranie
            UpdateAmmoText();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("shield"))
        {
            // Ustaw tarcz� na 100
            shieldStrength = 100;
            Debug.Log("Shield picked up! Current Shield: " + shieldStrength);

            // Zaktualizuj pasek tarczy
            UpdateShieldSlider();

            // Uruchom proces zmniejszania tarczy
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
            shieldCoroutine = StartCoroutine(DecreaseShieldOverTime());

            Destroy(collision.gameObject);
        }
    }

    // Funkcja do aktualizowania tekstu z amunicj�
    void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString();
    }

    // Funkcja do aktualizowania paska zdrowia
    void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth;
        Debug.Log("Health Slider Updated: " + healthSlider.value);
    }

    // Funkcja do aktualizowania paska tarczy
    void UpdateShieldSlider()
    {
        shieldSlider.value = shieldStrength;
    }

    // Funkcja do zadawania obra�e� graczowi
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
            Debug.Log("Player damaged! Current Health: " + playerHealth);
            UpdateHealthSlider();
        }
    }

    // Korutyna do zmniejszania tarczy w czasie
    IEnumerator DecreaseShieldOverTime()
    {
        while (shieldStrength > 0)
        {
            shieldStrength -= 5; // Zmniejsza tarcz� co sekund�
            shieldStrength = Mathf.Clamp(shieldStrength, 0, 100);
            UpdateShieldSlider();
            Debug.Log("Shield decreasing... Current Shield: " + shieldStrength);
            yield return new WaitForSeconds(1f);
        }
    }
}
