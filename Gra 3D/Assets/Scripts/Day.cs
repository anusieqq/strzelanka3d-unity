using UnityEngine;
using TMPro;

public class Day : MonoBehaviour
{
    public static Day Instance;

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
    public float gameTimeInMinutes = 6f * 60f;
    public float timeOfDay = 0.25f;

    [Header("Time Speed Control")]
    private float minSpeed = 0.049f;
    private float maxSpeed = 1.0f;
    private float changeCooldown = 0.5f;
    private float changeTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameTimeInMinutes = PlayerPrefs.GetFloat("GameTimeInMinutes", 6f * 60f);
        timeOfDay = PlayerPrefs.GetFloat("TimeOfDay", 0.25f);
        gameMinutesPerSecond = PlayerPrefs.GetFloat("TimeMultiplier", 0.1f);
        gameMinutesPerSecond = Mathf.Clamp(Mathf.Round(gameMinutesPerSecond * 10f) / 10f, minSpeed, maxSpeed);

        // Dynamiczne znajdowanie UI, jeœli nie przypisane
        if (timeDisplay == null)
        {
            timeDisplay = GameObject.Find("TimeDisplay")?.GetComponent<TextMeshProUGUI>();
            if (timeDisplay == null)
                Debug.LogError("Nie znaleziono timeDisplay w scenie!");
        }

        if (speedDisplay == null)
        {
            speedDisplay = GameObject.Find("SpeedDisplay")?.GetComponent<TextMeshProUGUI>();
            if (speedDisplay == null)
                Debug.LogError("Nie znaleziono speedDisplay w scenie!");
        }

        if (timeDisplay != null)
        {
            int gameHours = Mathf.FloorToInt(gameTimeInMinutes / 60f);
            int gameMinutes = Mathf.FloorToInt(gameTimeInMinutes % 60f);
            timeDisplay.text = $"Godzina: {gameHours:00}:{gameMinutes:00}";
            Debug.Log($"Zainicjowano czas gry w UI: {timeDisplay.text}");
        }

        if (speedDisplay != null)
        {
            speedDisplay.text = $"x{gameMinutesPerSecond:F1}";
            Debug.Log($"Zainicjowano mno¿nik czasu w UI: {speedDisplay.text}");
        }

        UpdateSunAndSkybox();
    }

    private void UpdateSunAndSkybox()
    {
        float sunAngle = (timeOfDay * 360f) - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
        sun.color = lightColorGradient.Evaluate(timeOfDay);
        sun.intensity = lightIntensityCurve.Evaluate(timeOfDay);

        if (proceduralSkybox != null)
        {
            proceduralSkybox.SetColor("_SkyTint", skyTintGradient.Evaluate(timeOfDay));
            proceduralSkybox.SetFloat("_Exposure", skyExposureCurve.Evaluate(timeOfDay));
        }
    }

    void Update()
    {
        changeTimer += Time.deltaTime;

        bool speedChanged = false;

        if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
        {
            if (changeTimer >= changeCooldown)
            {
                gameMinutesPerSecond += 0.1f;
                speedChanged = true;
            }
        }

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
            gameMinutesPerSecond = Mathf.Round(gameMinutesPerSecond * 10f) / 10f;
            changeTimer = 0f;

            PlayerPrefs.SetFloat("TimeMultiplier", gameMinutesPerSecond);
            PlayerPrefs.Save();

            if (speedDisplay != null)
            {
                speedDisplay.text = $"x{gameMinutesPerSecond:F1}";
            }
        }

        if (gameMinutesPerSecond == 0f)
        {
            gameMinutesPerSecond = minSpeed;
            PlayerPrefs.SetFloat("TimeMultiplier", gameMinutesPerSecond);
            PlayerPrefs.Save();
        }

        gameTimeInMinutes += Time.deltaTime * gameMinutesPerSecond * 10;
        if (gameTimeInMinutes >= 1440f)
            gameTimeInMinutes -= 1440f;

        PlayerPrefs.SetFloat("GameTimeInMinutes", gameTimeInMinutes);
        PlayerPrefs.SetFloat("TimeOfDay", timeOfDay);
        PlayerPrefs.Save();

        int gameHours = Mathf.FloorToInt(gameTimeInMinutes / 60f);
        int gameMinutes = Mathf.FloorToInt(gameTimeInMinutes % 60f);
        string formattedTime = $"{gameHours:00}:{gameMinutes:00}";

        if (timeDisplay != null)
            timeDisplay.text = $"Godzina: {formattedTime}";

        if (speedDisplay != null && !speedChanged)
            speedDisplay.text = $"x{gameMinutesPerSecond:F1}";

        timeOfDay = gameTimeInMinutes / 1440f;
        UpdateSunAndSkybox();
    }

    public void ResetTime()
    {
        gameTimeInMinutes = 6f * 60f;
        gameMinutesPerSecond = 0.1f;
        timeOfDay = 0.25f;

        PlayerPrefs.SetFloat("GameTimeInMinutes", gameTimeInMinutes);
        PlayerPrefs.SetFloat("TimeOfDay", timeOfDay);
        PlayerPrefs.SetFloat("TimeMultiplier", gameMinutesPerSecond);
        PlayerPrefs.Save();

        if (timeDisplay != null)
            timeDisplay.text = $"Godzina: 06:00";

        if (speedDisplay != null)
            speedDisplay.text = $"x0.1";

        UpdateSunAndSkybox();
    }
}