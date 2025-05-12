using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    public Light torchLight;           // Przypisz punktowe œwiat³o w edytorze Unity
    public float minIntensity = 0.5f;  // Minimalna intensywnoœæ œwiat³a
    public float maxIntensity = 5.0f;  // Maksymalna intensywnoœæ œwiat³a
    public float lightSpeed = 1.5f;    // Szybkoœæ migotania œwiat³a
    public Color baseColor = new Color(1.0f, 0.5f, 0.2f); // Kolor p³omienia

    void Update()
    {
        // Ustal intensywnoœæ œwiat³a w zale¿noœci od czasu, symuluj¹c migotanie p³omienia
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Sin(Time.time * lightSpeed) * 0.5f + 0.5f);
        torchLight.intensity = intensity;

        // Ustal kolor œwiat³a na podstawie intensywnoœci
        torchLight.color = baseColor * intensity;
    }
}
