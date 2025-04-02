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
    public Text enemyCountText; // Tekst licznika przeciwników
    public Slider healthSlider; // Pasek zdrowia
    public Slider shieldSlider; // Pasek tarczy

    void Start()
    {
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
            Debug.Log("Player damaged! Current Health: " + playerHealth);
            UpdateHealthSlider();
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
}
