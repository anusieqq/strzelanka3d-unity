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
    public Button serverButton; 
    public Slider uploadSlider;
    public GameObject gameOverCanvas;
    public TMP_Text messageText; 

    private bool isUploading = false;
    private bool ispendrive = false;
    private bool isServerRunning = false; 

    void Start()
    {
        if (uploadSlider != null)
        {
            uploadSlider.gameObject.SetActive(false);
            uploadSlider.value = 0;
        }
        else
        {
            Debug.LogError("UploadSlider nie jest przypisany!");
        }

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            startButton.onClick.AddListener(StartUpload);
        }
        else
        {
            Debug.LogError("StartButton nie jest przypisany!");
        }

        if (serverButton != null)
        {
            serverButton.gameObject.SetActive(false);
            serverButton.onClick.AddListener(StartServer);
        }
        else
        {
            Debug.LogError("ServerButton nie jest przypisany!");
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverCanvas nie jest przypisany!");
        }

        if (YesButton != null)
        {
            YesButton.onClick.AddListener(LoadMenu);
        }
        else
        {
            Debug.LogError("YesButton nie jest przypisany!");
        }

        if (NoButton != null)
        {
            NoButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("NoButton nie jest przypisany!");
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("MessageText nie jest przypisany!");
        }

        Debug.Log("Pendrive Start: ispendrive = " + ispendrive + ", isServerRunning = " + isServerRunning);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            if (ispendrive)
            {
                if (isServerRunning)
                {
                    if (startButton != null)
                    {
                        startButton.gameObject.SetActive(true);
                        Debug.Log("Wykryto kolizjê z graczem, pendrive zebrany, serwer uruchomiony, pokazano startButton.");
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    if (messageText != null)
                    {
                        messageText.gameObject.SetActive(false); 
                    }
                }
                else
                {
                    if (serverButton != null)
                    {
                        serverButton.gameObject.SetActive(true);
                        Debug.Log("Wykryto kolizjê z graczem, pendrive zebrany, serwer nie uruchomiony, pokazano serverButton.");
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    if (messageText != null)
                    {
                        messageText.text = "Uruchom serwer!";
                        messageText.gameObject.SetActive(true);
                        Debug.Log("Pokazano komunikat: Uruchom serwer!");
                    }
                }
            }
            else
            {
                if (messageText != null)
                {
                    messageText.text = "Zdob¹dŸ pendrive'a!";
                    messageText.gameObject.SetActive(true);
                    Debug.Log("Wykryto kolizjê z graczem, brak pendrive'a, pokazano komunikat.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Warunki nie spe³nione: isUploading = " + isUploading + ", ispendrive = " + ispendrive + ", isServerRunning = " + isServerRunning);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            if (startButton != null)
            {
                startButton.gameObject.SetActive(false);
                Debug.Log("Gracz opuœci³ UploadPoint, ukryto startButton.");
            }
            if (serverButton != null)
            {
                serverButton.gameObject.SetActive(false);
                Debug.Log("Gracz opuœci³ UploadPoint, ukryto serverButton.");
            }
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
                Debug.Log("Gracz opuœci³ UploadPoint, ukryto komunikat.");
            }
        }
    }

    public void StartServer()
    {
        if (!ispendrive)
        {
            Debug.LogWarning("Nie mo¿esz uruchomiæ serwera bez pendrive'a!");
            return;
        }

        isServerRunning = true;
        if (serverButton != null)
        {
            serverButton.gameObject.SetActive(false);
            Debug.Log("Serwer uruchomiony, ukryto serverButton.");
        }
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
            Debug.Log("Pokazano startButton po uruchomieniu serwera.");
        }
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
            Debug.Log("Ukryto komunikat po uruchomieniu serwera.");
        }
    }

    public void StartUpload()
    {
        if (!ispendrive)
        {
            Debug.LogWarning("Nie masz pendrive'a! Wgrywanie zablokowane.");
            return;
        }
        if (!isServerRunning)
        {
            Debug.LogWarning("Serwer nie jest uruchomiony! Wgrywanie zablokowane.");
            return;
        }

        isUploading = true;

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
        if (uploadSlider != null)
        {
            uploadSlider.gameObject.SetActive(true);
            uploadSlider.value = 0;
        }
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false); 
        }

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
            {
                uploadSlider.value = Mathf.Lerp(0f, 1f, time / duration);
            }
            yield return null;
        }

        if (uploadSlider != null)
        {
            uploadSlider.value = 1f;
        }
        Debug.Log("Wgrywanie zakoñczone!");

        yield return new WaitForSeconds(1f);

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("GameOverCanvas aktywowany!");
        }
    }

    public void LoadMenu()
    {
        Debug.Log("£adowanie sceny Menu...");
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Debug.Log("Wyjœcie z gry...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetPendriveCollected()
    {
        ispendrive = true;
        Debug.Log("Pendrive zebrany - mo¿na wgrywaæ pliki. ispendrive = " + ispendrive);
    }
}