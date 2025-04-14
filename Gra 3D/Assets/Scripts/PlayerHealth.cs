using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    public float currentHealth;
    public float maxHealth;

    public void SetHealth(float hp, float maxHp)
    {
        currentHealth = hp;
        maxHealth = maxHp;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = hp;
        }
    }
}
