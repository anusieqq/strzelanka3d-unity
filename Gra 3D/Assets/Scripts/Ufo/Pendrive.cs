using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Pendrive : MonoBehaviour
{
    public Button startButton;
    public Button YesButton;
    public Button NoButton;
    public Slider uploadSlider;
    public GameObject gameOverCanvas;
    public TMP_Text messageText;

    private bool isUploading = false;
    private bool ispendrive = false;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (uploadSlider != null)
        {
            uploadSlider.gameObject.SetActive(false);
            uploadSlider.value = 0;
        }

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            startButton.onClick.AddListener(StartUpload);
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (YesButton != null)
        {
            YesButton.onClick.AddListener(LoadMenu);
        }

        if (NoButton != null)
        {
            NoButton.onClick.AddListener(ExitGame);
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        Debug.Log("Pendrive Start: ispendrive = " + ispendrive);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            if (ispendrive)
            {
                if (ServerManager.Instance != null && ServerManager.Instance.IsServerRunning)
                {
                    startButton?.gameObject.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    messageText?.gameObject.SetActive(false);
                }
                else
                {
                    messageText.text = "Uruchom serwer!";
                    messageText.gameObject.SetActive(true);
                }
            }
            else
            {
                messageText.text = "Zdobądź pendrive'a!";
                messageText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            startButton?.gameObject.SetActive(false);
            messageText?.gameObject.SetActive(false);
        }
    }

    public void StartUpload()
    {
        if (!ispendrive || ServerManager.Instance == null || !ServerManager.Instance.IsServerRunning)
        {
            Debug.LogWarning("Nie można wgrać – brak pendrive'a lub serwera.");
            return;
        }

        isUploading = true;
        startButton?.gameObject.SetActive(false);
        uploadSlider?.gameObject.SetActive(true);
        messageText?.gameObject.SetActive(false);
        StartCoroutine(UploadProgress());
    }

    IEnumerator UploadProgress()
    {
        float duration = 10f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            if (uploadSlider != null)
                uploadSlider.value = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        uploadSlider.value = 1f;
        Debug.Log("Wgrywanie zakończone!");

        PlayerPrefs.SetInt("ResetGame", 1);
        PlayerPrefs.Save();
        Debug.Log("Flaga ResetGame ustawiona na true w Pendrive.cs");

        yield return new WaitForSeconds(1f);

        if (gameOverCanvas != null)
        {
            Time.timeScale = 0f;
            gameOverCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LoadMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            Destroy(AudioManager.Instance.gameObject);
            Debug.Log("AudioManager destroyed for reset.");
        }

        if (Pause.Instance != null)
        {
            Destroy(Pause.Instance.gameObject);
            Debug.Log("Pause destroyed for reset.");
        }

        if (PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
            Debug.Log("PlayerController destroyed for reset.");
        }

        if (Day.Instance != null)
        {
            Day.Instance.ResetTime(); // Resetuj czas przed zniszczeniem
            Destroy(Day.Instance.gameObject);
            Debug.Log("Day destroyed for reset.");
        }

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared for reset.");

        Debug.Log("Ładowanie sceny Menu...");
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Debug.Log("Wyjście z gry...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetPendriveCollected()
    {
        ispendrive = true;
        Debug.Log("Pendrive zebrany - można wgrywać pliki. ispendrive = " + ispendrive);
    }
}