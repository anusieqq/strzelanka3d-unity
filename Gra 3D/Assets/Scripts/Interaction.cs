using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Do obs³ugi UI (Text, Slider)

public class Interaction : MonoBehaviour
{
    // Zmienne do przechowywania stanu gracza
    private int playerHealth = 80; // Pocz¹tkowe zdrowie gracza
    private int ammoCount = 10; // Na starcie gracz ma 10 amunicji
    private int shieldStrength = 0;

    // Referencje do komponentów UI
    public Text ammoText; // Tekst amunicji
    public Slider healthSlider; // Pasek zdrowia

    // Start jest wywo³ywane na pocz¹tku gry
    void Start()
    {
        // Ustawienie maksymalnej wartoœci paska zdrowia
        healthSlider.maxValue = 100;
        healthSlider.minValue = 0; // Upewniamy siê, ¿e minimalna wartoœæ to 0
        healthSlider.wholeNumbers = false; // Umo¿liwienie wartoœci zmiennoprzecinkowych

        // Pocz¹tkowe ustawienie zdrowia i paska zdrowia
        playerHealth = Mathf.Clamp(playerHealth, 0, 100);
        UpdateHealthSlider();

        // Wyœwietlenie pocz¹tkowej iloœci amunicji w UI
        UpdateAmmoText();
    }

    // Funkcja wywo³ywana przy kolizji z innymi obiektami
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            // Dodaj zdrowie graczowi
            playerHealth += 10;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100); // Zapobiega przekroczeniu 100
            Debug.Log("Health picked up! Current Health: " + playerHealth);

            // Zaktualizuj pasek ¿ycia
            UpdateHealthSlider();

            Destroy(collision.gameObject); // Usuñ przedmiot po podniesieniu
        }
        else if (collision.gameObject.CompareTag("ammo"))
        {
            // Dodaj amunicjê
            ammoCount += 5;
            Debug.Log("Ammo picked up! Current Ammo: " + ammoCount);

            // Zaktualizuj tekst na ekranie
            UpdateAmmoText();

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("shield"))
        {
            // Dodaj tarczê
            shieldStrength += 1;
            Debug.Log("Shield picked up! Current Shield: " + shieldStrength);
            Destroy(collision.gameObject);
        }
    }

    // Funkcja do aktualizowania tekstu z amunicj¹
    void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString();
    }

    // Funkcja do aktualizowania paska zdrowia
    void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth; // Ustawienie wartoœci Slidera
        Debug.Log("Health Slider Updated: " + healthSlider.value);
    }

    // Funkcja do zadawania obra¿eñ graczowi
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        playerHealth = Mathf.Clamp(playerHealth, 0, 100); // Zapewnia, ¿e nie spadnie poni¿ej 0
        Debug.Log("Player damaged! Current Health: " + playerHealth);

        // Aktualizacja paska zdrowia po otrzymaniu obra¿eñ
        UpdateHealthSlider();
    }
}
