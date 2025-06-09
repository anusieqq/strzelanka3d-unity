using UnityEngine;
using UnityEngine.SceneManagement;

public class Opcje : MonoBehaviour
{
    public GameObject menuCanvas;

    public GameObject PauseCanvas;

    private void Start()
    {
       
    }

    public void UniversalBackButton()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + currentScene);

        if (currentScene == "Menu")
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(true);
                Debug.Log("MenuCanvas w��czony");
            }
            else
            {
                Debug.LogWarning("Brak przypisanego menuCanvas!");
            }
        }
        else
        {
            // Znajd� obiekt z komponentem Pause i wywo�aj odpowiednie metody
            Pause pauseScript = FindObjectOfType<Pause>();
            if (pauseScript != null)
            {
                // Najpierw ukryj opcje
                if (AudioManager.Instance != null && AudioManager.Instance.optionsPanel != null)
                {
                    AudioManager.Instance.optionsPanel.SetActive(false);
                }

                // Nast�pnie poka� menu pauzy
                pauseScript.HideOptions(); 
            }
            else
            {
                Debug.LogWarning("Brak skryptu Pause w scenie!");
            }
        }
    }
}