using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Pause : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject PauseCanvas;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider enemyHealthSlider;

    private GameObject Opcje;
    private bool isPaused = false;
    private CharacterController characterController;
    private bool isLoading = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        InitializeUI();

        if (AudioManager.Instance != null && AudioManager.Instance.uiOpcje != null)
        {
            Opcje = AudioManager.Instance.uiOpcje;
            Opcje.SetActive(false);
        }

        Invoke("DelayedLoad", 0.1f);
    }

    private void DelayedLoad()
    {
        isLoading = true;
        LoadPlayerData();
        isLoading = false;
    }

    private void InitializeUI()
    {
        PauseCanvas.gameObject.SetActive(false);
    }

    private void LoadPlayerData()
    {
        if (characterController != null)
            characterController.enabled = false;

        float hp = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        float maxHp = PlayerPrefs.GetFloat("MaxHealth", 100f);
        float shield = PlayerPrefs.GetFloat("PlayerShield", 0f);
        float maxShield = PlayerPrefs.GetFloat("MaxShield", 0f);

        Vector3 savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PlayerPosX", transform.position.x),
            PlayerPrefs.GetFloat("PlayerPosY", transform.position.y),
            PlayerPrefs.GetFloat("PlayerPosZ", transform.position.z)
        );

        if (Physics.Raycast(savedPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
        {
            transform.position = hit.point + Vector3.up * 0.2f;
        }
        else
        {
            transform.position = savedPosition + Vector3.up * 1f;
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetHealth(hp, maxHp);
            playerController.SetShield(shield, maxShield);
        }

        if (characterController != null)
            characterController.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isLoading)
        {
            if (isPaused)
                Resume();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (Opcje != null)
            Opcje.SetActive(false);

        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OptionsGame()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(true);
            PauseCanvas.SetActive(false);
        }
    }

    public void HideOptions()
    {
        if (Opcje != null)
        {
            Opcje.SetActive(false);
            PauseCanvas.SetActive(true);
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        GameObject player = GameObject.FindGameObjectWithTag("player");

        if (player != null)
        {
            Vector3 safePosition = player.transform.position + Vector3.up * 1f;
            data.playerPosX = safePosition.x;
            data.playerPosY = safePosition.y;
            data.playerPosZ = safePosition.z;
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
        PlayerPrefs.SetFloat("PlayerPosX", data.playerPosX);
        PlayerPrefs.SetFloat("PlayerPosY", data.playerPosY);
        PlayerPrefs.SetFloat("PlayerPosZ", data.playerPosZ);
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
