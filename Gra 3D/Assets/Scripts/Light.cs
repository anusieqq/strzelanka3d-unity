using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    public Light torchLight;           // Przypisz punktowe �wiat�o w edytorze Unity
    public float minIntensity = 0.5f;  // Minimalna intensywno�� �wiat�a
    public float maxIntensity = 5.0f;  // Maksymalna intensywno�� �wiat�a
    public float lightSpeed = 1.5f;    // Szybko�� migotania �wiat�a
    public Color baseColor = new Color(1.0f, 0.5f, 0.2f); // Kolor p�omienia

    void Update()
    {
        // Ustal intensywno�� �wiat�a w zale�no�ci od czasu, symuluj�c migotanie p�omienia
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Sin(Time.time * lightSpeed) * 0.5f + 0.5f);
        torchLight.intensity = intensity;

        // Ustal kolor �wiat�a na podstawie intensywno�ci
        torchLight.color = baseColor * intensity;
    }
}
