using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] public string enemyId; // R�cznie przypisane ID w Inspectorze

    public Slider healthSlider; // Pasek zdrowia w Inspectorze

    void Awake()
    {
        // Walidacja, czy enemyId jest ustawione
        if (string.IsNullOrEmpty(enemyId))
        {
            Debug.LogError($"Wr�g {gameObject.name} nie ma ustawionego enemyId w Inspectorze!");
        }
        else
        {
            Debug.Log($"Wr�g {gameObject.name} ma ID: {enemyId}");
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Metoda dla Gun.cs (jeden argument)
    public void TakeDamage(string sourceTag)
    {
        float damage = sourceTag == "pistol" ? 20f : 10f; // Domy�lne obra�enia dla innych tag�w
        TakeDamage(sourceTag, damage);
    }

    // Metoda dla Torch.cs (dwa argumenty)
    public void TakeDamage(string sourceTag, float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        Debug.Log($"Wr�g {enemyId} ({gameObject.name}) otrzyma� obra�enia od {sourceTag}, damage: {damage}, HP: {currentHealth}");

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
        else
        {
            Debug.LogWarning($"Brak przypisanego Slidera zdrowia dla wroga {gameObject.name}");
        }
    }

    void Die()
    {
        Debug.Log($"Wr�g {enemyId} ({gameObject.name}) zosta� zniszczony.");
        Destroy(gameObject);
    }
}