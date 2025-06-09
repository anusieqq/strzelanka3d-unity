using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UfoLightingSystem : MonoBehaviour
{
    [System.Serializable]
    public class UfoLightPair
    {
        public GameObject lightObject; // GameObject kulki z Rendererem
        public GameObject lightSource; // GameObject ze œwiat³em (Light)
        public string colorName; // Wspólny kolor dla kulki i œwiat³a
        [HideInInspector] public bool isOn; // Stan migania
        private Renderer renderer;
        private Light lightComponent;

        public void Initialize()
        {
            if (lightObject == null || lightSource == null) return;

            renderer = lightObject.GetComponent<Renderer>();
            lightComponent = lightSource.GetComponent<Light>();
            isOn = false;
            UpdateVisualState();
        }

        public void Toggle()
        {
            isOn = !isOn;
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (lightComponent != null)
            {
                lightComponent.enabled = isOn;
                if (isOn && ColorUtility.TryParseHtmlString(colorName, out Color parsedColor))
                {
                    lightComponent.color = parsedColor;
                }
            }
            if (renderer != null)
            {
                renderer.material.color = isOn && ColorUtility.TryParseHtmlString(colorName, out Color parsedColor) ? parsedColor : Color.black;
            }

            Debug.Log($"{colorName} light pair is {(isOn ? "ON" : "OFF")}");
        }

        public override string ToString()
        {
            return $"{colorName} light pair is {(isOn ? "ON" : "OFF")}";
        }
    }

    [SerializeField] private List<UfoLightPair> lightPairs = new List<UfoLightPair>(); // Lista par kulka-œwiat³o
    private bool isRunning;
    private System.Random random = new System.Random();

    void Start()
    {
        // Inicjalizacja wszystkich par
        foreach (var pair in lightPairs)
        {
            pair.Initialize();
        }

        // Wyœwietlenie pocz¹tkowego stanu
        DisplayLightsStatus();

        // Rozpoczêcie migania
        StartFlashing(0.5f);
    }

    public void StartFlashing(float flashIntervalSeconds = 0.5f)
    {
        if (lightPairs.Count == 0)
        {
            Debug.LogWarning("Brak par kulka-œwiat³o do migania! Dodaj je w Inspectorze.");
            return;
        }

        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(FlashingCoroutine(flashIntervalSeconds));
        }
    }

    private IEnumerator FlashingCoroutine(float flashIntervalSeconds)
    {
        while (isRunning)
        {
            foreach (var pair in lightPairs)
            {
                if (random.Next(0, 2) == 1) // Losowe miganie (50% szansy)
                {
                    pair.Toggle();
                }
            }
            Debug.Log("---");
            yield return new WaitForSeconds(flashIntervalSeconds);
        }
    }

    public void StopFlashing()
    {
        isRunning = false;
    }

    public void DisplayLightsStatus()
    {
        if (lightPairs.Count == 0)
        {
            Debug.Log("Lista par kulka-œwiat³o jest pusta!");
            return;
        }

        foreach (var pair in lightPairs)
        {
            Debug.Log(pair);
        }
        Debug.Log("---");
    }
}