using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Pause : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas PauseCanvas;
    public GameObject opcjePanel;
    public GameObject StartButton;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider enemyHealthSlider;

    private bool isPaused = false;

    private void Start()
    {
        InitializeUI();
        LoadPlayerData();
    }

    private void InitializeUI()
    {
        PauseCanvas.gameObject.SetActive(false);
        opcjePanel.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
    }

    private void LoadPlayerData()
    {
        float hp = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        float maxHp = PlayerPrefs.GetFloat("MaxHealth", 100f);
        float shield = PlayerPrefs.GetFloat("PlayerShield", 0f);
        float maxShield = PlayerPrefs.GetFloat("MaxShield", 0f);

        transform.position = new Vector3(
            PlayerPrefs.GetFloat("PlayerPosX", transform.position.x),
            PlayerPrefs.GetFloat("PlayerPosY", transform.position.y),
            PlayerPrefs.GetFloat("PlayerPosZ", transform.position.z)
        );

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetHealth(hp, maxHp);
            playerController.SetShield(shield, maxShield);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        PauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OptionsGame()
    {
        opcjePanel.gameObject.SetActive(!opcjePanel.gameObject.activeSelf);
        StartButton.gameObject.SetActive(!StartButton.gameObject.activeSelf);
        PauseCanvas.gameObject.SetActive(false);
    }

    public void HideOptions()
    {
        opcjePanel.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        PauseCanvas.gameObject.SetActive(true);
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            data.playerPosX = player.transform.position.x;
            data.playerPosY = player.transform.position.y;
            data.playerPosZ = player.transform.position.z;
        }

        data.levelName = SceneManager.GetActiveScene().name;
        data.currentHP = healthSlider.value;
        data.maxHP = healthSlider.maxValue;
        data.currentShield = shieldSlider.value;
        data.maxShield = shieldSlider.maxValue;
        data.ufoHP = enemyHealthSlider.value;
        data.ufoMaxHP = enemyHealthSlider.maxValue;

        string json = JsonUtility.ToJson(data, true);
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        File.WriteAllText(Path.Combine(desktopPath, "savegame.json"), json);

        PlayerPrefs.SetFloat("PlayerHealth", data.currentHP);
        PlayerPrefs.SetFloat("MaxHealth", data.maxHP);
        PlayerPrefs.SetFloat("PlayerShield", data.currentShield);
        PlayerPrefs.SetFloat("MaxShield", data.maxShield);
        PlayerPrefs.SetFloat("EnemyHealth", data.ufoHP);
        PlayerPrefs.SetFloat("EnemyMaxHealth", data.ufoMaxHP);
        PlayerPrefs.SetString("SavedScene", data.levelName);
        PlayerPrefs.Save();
    }

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
}
