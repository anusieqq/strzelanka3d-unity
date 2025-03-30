using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Gun gunScript; // Referencja do skryptu Gun

    private int playerHealth = 80; // Pocz¹tkowe zdrowie gracza
    private float shieldStrength = 0; // Pocz¹tkowa si³a tarczy
    private Coroutine shieldCoroutine;

    public Text ammoText; // Tekst amunicji
    public Slider healthSlider; // Pasek zdrowia
    public Slider shieldSlider; // Pasek tarczy

    void Start()
    {
        // Ustawienie maksymalnych wartoœci pasków
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
    }

    void Update()
    {
  
    }

    // Funkcja wywo³ywana przy kolizji z innymi obiektami
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            playerHealth += 10;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            Debug.Log("Health picked up! Current Health: " + playerHealth);
            UpdateHealthSlider();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ammo"))
        {
            // Sprawdzamy, czy aktualny zapas jest mniejszy ni¿ maksymalny
            if (gunScript.reserveAmmo < gunScript.maxReserveAmmo)
            {
                int ammoToAdd = 5; // Iloœæ dodawanej amunicji

                // Obliczamy now¹ wartoœæ i upewniamy siê, ¿e nie przekroczy limitu
                gunScript.reserveAmmo += ammoToAdd;
                if (gunScript.reserveAmmo > gunScript.maxReserveAmmo)
                {
                    gunScript.reserveAmmo = gunScript.maxReserveAmmo;
                }

                Debug.Log("Ammo picked up! Current Ammo in reserve: " + gunScript.reserveAmmo);

                // Aktualizacja ammo
                gunScript.UpdateAmmoText();

                // Usuniêcie obiektu amunicji
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
            Destroy(collision.gameObject);
        }
    }


    // Funkcja do aktualizowania tekstu z amunicj¹
    void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + gunScript.ammoCount.ToString() + "/" + gunScript.reserveAmmo.ToString();
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

    // Funkcja do zadawania obra¿eñ graczowi
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

    IEnumerator DecreaseShieldOverTime()
    {
        while (shieldStrength > 0)
        {
            shieldStrength -= 5; // Zmniejsza tarczê co sekundê
            shieldStrength = Mathf.Clamp(shieldStrength, 0, 100);
            UpdateShieldSlider();
            Debug.Log("Shield decreasing... Current Shield: " + shieldStrength);
            yield return new WaitForSeconds(1f);
        }
    }
}
