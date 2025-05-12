using UnityEngine;
using TMPro;

public class Day : MonoBehaviour
{
    public static Day Instance;  // Singleton

    [Header("Sun Settings")]
    public Light sun;

    [Header("Skybox Settings")]
    public Material proceduralSkybox;
    public Gradient skyTintGradient;
    public AnimationCurve skyExposureCurve;

    [Header("Sun Light Settings")]
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;

    [Header("UI")]
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI speedDisplay;

    [Header("Game Clock Settings")]
    public float gameMinutesPerSecond = 0.1f;
    private float gameTimeInMinutes = 6f * 60f;

    private float timeOfDay = 0.25f;

    [Header("Time Speed Control")]
    private float minSpeed = 0.049f;
    private float maxSpeed = 1.0f;
    private float changeCooldown = 0.5f;
    private float changeTimer = 0f;

    private void Awake()
    {
        // Singleton: jeœli instancja ju¿ istnieje, zniszcz ten obiekt
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Zapobiegamy zniszczeniu przy zmianie sceny
        }
        else
        {
            Destroy(gameObject);  // Zniszczenie duplikatu, jeœli ju¿ istnieje
        }
    }

    void Update()
    {
        changeTimer += Time.deltaTime;

        bool speedChanged = false;

        // Zwiêkszanie prêdkoœci
        if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
        {
            if (changeTimer >= changeCooldown)
            {
                gameMinutesPerSecond += 0.1f;
                speedChanged = true;
            }
        }

        // Zmniejszanie prêdkoœci
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            if (changeTimer >= changeCooldown)
            {
                gameMinutesPerSecond -= 0.1f;
                speedChanged = true;
            }
        }

        if (speedChanged)
        {
            gameMinutesPerSecond = Mathf.Clamp(gameMinutesPerSecond, minSpeed, maxSpeed);
            gameMinutesPerSecond = Mathf.Round(gameMinutesPerSecond * 10f) / 10f; // Zaokr¹glenie do 0.1
            changeTimer = 0f;
        }

        // === Zapewnienie minimalnej prêdkoœci, gdy gameMinutesPerSecond jest bliskie 0 ===
        if (gameMinutesPerSecond == 0f)
        {
            gameMinutesPerSecond = minSpeed;  // Ustaw minimaln¹ prêdkoœæ, gdy 0
        }

        // === Aktualizacja czasu gry ===
        gameTimeInMinutes += Time.deltaTime * gameMinutesPerSecond * 10;
        if (gameTimeInMinutes >= 1440f)
            gameTimeInMinutes -= 1440f;

        int gameHours = Mathf.FloorToInt(gameTimeInMinutes / 60f);
        int gameMinutes = Mathf.FloorToInt(gameTimeInMinutes % 60f);
        string formattedTime = $"{gameHours:00}:{gameMinutes:00}";

        if (timeDisplay != null)
            timeDisplay.text = $"Godzina: {formattedTime}";

        if (speedDisplay != null)
            speedDisplay.text = $"x{gameMinutesPerSecond:F1}";

        // === Obrót s³oñca ===
        timeOfDay = gameTimeInMinutes / 1440f;
        float sunAngle = (timeOfDay * 360f) - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        // === Œwiat³o i skybox ===
        sun.color = lightColorGradient.Evaluate(timeOfDay);
        sun.intensity = lightIntensityCurve.Evaluate(timeOfDay);

        if (proceduralSkybox != null)
        {
            proceduralSkybox.SetColor("_SkyTint", skyTintGradient.Evaluate(timeOfDay));
            proceduralSkybox.SetFloat("_Exposure", skyExposureCurve.Evaluate(timeOfDay));
        }

        // === Debug (opcjonalnie) ===
        //Debug.Log($"Godzina: {formattedTime} | x{gameMinutesPerSecond:F1} | K¹t s³oñca: {sunAngle:F1}°");
    }
}
