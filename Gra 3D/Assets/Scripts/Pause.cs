using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Pause : MonoBehaviour
{
    public Canvas PauseCanvas;
    private bool isPaused = false;
    public GameObject opcjePanel;
    public GameObject StartButton;

    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider enemyHealthSlider;

    [System.Serializable]
    public class GameData
    {
        public float playerPosX;
        public float playerPosY;
        public float playerPosZ;
        public string levelName;

        public float currentHP;
        public float maxHP;

        public float currentShield;
        public float maxShield;

        public float ufoHP;
        public float ufoMaxHP;
    }

    void Start()
    {
        float hp = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        float maxHp = PlayerPrefs.GetFloat("MaxHealth", 100f);

        float shield = PlayerPrefs.GetFloat("PlayerShield", 0f);
        float maxShield = PlayerPrefs.GetFloat("MaxShield", 0f);

        float x = PlayerPrefs.GetFloat("PlayerPosX", transform.position.x);
        float y = PlayerPrefs.GetFloat("PlayerPosY", transform.position.y);
        float z = PlayerPrefs.GetFloat("PlayerPosZ", transform.position.z);

        transform.position = new Vector3(x, y, z);

        // np. jeœli masz PlayerHealth.cs:
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetHealth(hp, maxHp);
        }
        else
        {
            Debug.LogWarning("Brak komponentu PlayerHealth!");
        }

        PlayerShield playerShield = GetComponent<PlayerShield>();  // <-- tu by³a literówka
        if (playerShield != null)
        {
            playerShield.SetShield(shield, maxShield);
        }
        else
        {
            Debug.LogWarning("Brak komponentu PlayerShield!");
        }


        PauseCanvas.gameObject.SetActive(false);
        opcjePanel.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        PauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Gra zatrzymana, kursor aktywny.");
    }

    public void Resume()
    {
        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void OptionsGame()
    {
        if (opcjePanel != null)
        {
            bool isActive = opcjePanel.gameObject.activeSelf;
            bool Active = StartButton.gameObject.activeSelf;
            opcjePanel.gameObject.SetActive(!isActive);
            StartButton.gameObject.SetActive(!Active);
        }
    }

    public void HideOptions()
    {
        opcjePanel.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        PauseCanvas.gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        Debug.Log("Start zapisu...");

        GameData data = new GameData();

        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            data.playerPosX = pos.x;
            data.playerPosY = pos.y;
            data.playerPosZ = pos.z;
            Debug.Log("Zapisano pozycjê gracza.");
        }
        else
        {
            Debug.LogWarning("Nie znaleziono gracza!");
        }

        data.levelName = SceneManager.GetActiveScene().name;

        if (healthSlider == null || shieldSlider == null || enemyHealthSlider == null)
        {
            Debug.LogWarning("Brakuje sliderów!");
            return;
        }

        data.currentHP = healthSlider.value;
        data.maxHP = healthSlider.maxValue;

        data.currentShield = shieldSlider.value;
        data.maxShield = shieldSlider.maxValue;

        data.ufoHP = enemyHealthSlider.value;
        data.ufoMaxHP = enemyHealthSlider.maxValue;

        string json = JsonUtility.ToJson(data, true);

        // Œcie¿ka do pulpitu
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "savegame.json");

        File.WriteAllText(filePath, json);

        Debug.Log("Gra zapisana na pulpicie: " + filePath);

        // Zapisz dane do PlayerPrefs
        PlayerPrefs.SetFloat("PlayerHealth", healthSlider.value);
        PlayerPrefs.SetFloat("MaxHealth", healthSlider.maxValue);

        PlayerPrefs.SetFloat("PlayerShield", shieldSlider.value);
        PlayerPrefs.SetFloat("MaxShield", shieldSlider.maxValue);

        PlayerPrefs.SetFloat("EnemyHealth", enemyHealthSlider.value);
        PlayerPrefs.SetFloat("EnemyMaxHealth", enemyHealthSlider.maxValue);

        PlayerPrefs.SetString("SavedScene", data.levelName);
        PlayerPrefs.Save();
    }
}
