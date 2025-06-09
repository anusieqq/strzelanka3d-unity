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
                Debug.Log("MenuCanvas w³¹czony");
            }
            else
            {
                Debug.LogWarning("Brak przypisanego menuCanvas!");
            }
        }
        else
        {
            // ZnajdŸ obiekt z komponentem Pause i wywo³aj odpowiednie metody
            Pause pauseScript = FindObjectOfType<Pause>();
            if (pauseScript != null)
            {
                // Najpierw ukryj opcje
                if (AudioManager.Instance != null && AudioManager.Instance.optionsPanel != null)
                {
                    AudioManager.Instance.optionsPanel.SetActive(false);
                }

                // Nastêpnie poka¿ menu pauzy
                pauseScript.HideOptions(); 
            }
            else
            {
                Debug.LogWarning("Brak skryptu Pause w scenie!");
            }
        }
    }
}