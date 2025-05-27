using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthSlider; // Pasek zdrowia w Inspectorze

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(string sourceTag)
    {
        float damage = 0f;

        if (sourceTag == "pistol")
        {
            damage = 20f;
        }
        else if (sourceTag == "Torch")
        {
            damage = 10f;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        Debug.Log($"Enemy hit by {sourceTag}, damage: {damage}, HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Destroy(gameObject); // Usuwa przeciwnika po œmierci
    }
}