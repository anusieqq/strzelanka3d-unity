using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : MonoBehaviour
{
    public float currentShield = 0;
    public float maxShield = 100;
    public Slider shieldSlider;
    public float decayRate = 5f;
    public float decayInterval = 1f; // Czas w sekundach miêdzy ka¿dym spadkiem tarczy

    private float decayTimer = 0f;

    void Start()
    {
        // Za³aduj dane tarczy z PlayerPrefs, jeœli s¹ dostêpne
        currentShield = PlayerPrefs.GetFloat("PlayerShield", maxShield);  // Domyœlnie ustawiamy na maxShield, jeœli nie ma danych
        maxShield = PlayerPrefs.GetFloat("MaxShield", 100f);  // Domyœlna wartoœæ maxShield to 100

        // Ustawienie slidera, jeœli jest przypisany
        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = currentShield;
        }
    }

    public void SetShield(float current, float max)
    {
        currentShield = current;
        maxShield = max;

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = currentShield;
        }
    }

    void Update()
    {
        if (currentShield > 0)
        {
            decayTimer += Time.deltaTime;

            if (decayTimer >= decayInterval)
            {
                currentShield -= decayRate;
                currentShield = Mathf.Max(0, currentShield); // Zapewniamy, ¿e tarcza nie stanie siê ujemna

                if (shieldSlider != null)
                    shieldSlider.value = currentShield;

                decayTimer = 0f; // Resetujemy timer, ¿eby spadek by³ skokowy
            }
        }
    }
}