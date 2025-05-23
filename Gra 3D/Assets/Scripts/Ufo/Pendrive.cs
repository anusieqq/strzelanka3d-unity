using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pendrive : MonoBehaviour
{
    public Button startButton;             
    public Button YesButton;             
    public Button NoButton;             
    public Slider uploadSlider;           
    public GameObject gameOverCanvas;     

    private bool isUploading = false;

    void Start()
    {
        // Ukryj UI na starcie
        uploadSlider.gameObject.SetActive(false);
        uploadSlider.value = 0;

        startButton.gameObject.SetActive(false);
        gameOverCanvas.SetActive(false);

        // Dodaj akcjê do przycisku
        startButton.onClick.AddListener(StartUpload);
        YesButton.onClick.AddListener(LoadMenu);
        NoButton.onClick.AddListener(ExitGame);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            startButton.gameObject.SetActive(true);
            Debug.Log("Wykryto kolizjê z graczem");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player") && !isUploading)
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public void StartUpload()
    {
        isUploading = true;

        startButton.gameObject.SetActive(false);
        uploadSlider.gameObject.SetActive(true);
        uploadSlider.value = 0;

        StartCoroutine(UploadProgress());
    }

    IEnumerator UploadProgress()
    {
        float duration = 10f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            uploadSlider.value = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        uploadSlider.value = 1f;
        Debug.Log("Wgrywanie zakoñczone!");

        yield return new WaitForSeconds(1f);

        // Pokazujemy ekran koñcowy
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
