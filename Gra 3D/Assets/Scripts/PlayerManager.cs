using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject player;
    public GameObject uiInterfejs;
    public GameObject uiPause;
    //public GameObject uiOpcje;
    public Camera playerCamera; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (player != null)
                DontDestroyOnLoad(player);

            if (uiInterfejs != null)
                DontDestroyOnLoad(uiInterfejs);

            if (uiPause != null)
                DontDestroyOnLoad(uiPause);

            //if (uiOpcje != null)
                //DontDestroyOnLoad(uiOpcje);

            if (playerCamera == null)
                playerCamera = Camera.main;

            if (playerCamera != null)
                DontDestroyOnLoad(playerCamera.gameObject);

            // Zarejestruj siê na zdarzenie zmiany sceny
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Odœwie¿enie kamery
        if (Camera.main != playerCamera)
        {
            playerCamera = Camera.main;

            if (playerCamera != null)
                DontDestroyOnLoad(playerCamera.gameObject);
        }

        // Przenieœ gracza do StartPoint jeœli istnieje
        GameObject startPoint = GameObject.Find("StartPoint");
        if (startPoint != null && player != null)
        {
            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;
        }
    }
}
