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
            //    DontDestroyOnLoad(uiOpcje);

            if (playerCamera == null)
                playerCamera = Camera.main;

            if (playerCamera != null)
                DontDestroyOnLoad(playerCamera.gameObject);

            // Zarejestruj si� na zdarzenie zmiany sceny
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("PlayerManager utworzony i ustawiony jako DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("Duplikat PlayerManager, niszczenie.");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("PlayerManager zniszczony.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Od�wie�enie kamery
        if (Camera.main != playerCamera)
        {
            playerCamera = Camera.main;

            if (playerCamera != null)
                DontDestroyOnLoad(playerCamera.gameObject);
            Debug.Log("Kamera od�wie�ona w PlayerManager.");
        }

        // Przenie� gracza do StartPoint je�li istnieje
        GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        if (startPoint != null && player != null)
        {
            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;
            Debug.Log($"PlayerManager: Ustawiono pozycj� gracza na StartPoint: {startPoint.transform.position}, rotacja: {startPoint.transform.rotation}");
        }
        else
        {
            Vector3 defaultPosition = new Vector3(14.25f, -10.0f, 68.9f);
            Quaternion defaultRotation = Quaternion.Euler(0, -180, 0);
            if (player != null)
            {
                player.transform.position = defaultPosition;
                player.transform.rotation = defaultRotation;
                Debug.LogWarning($"PlayerManager: StartPoint nie znaleziony, ustawiono domy�ln� pozycj�: {defaultPosition}, rotacja: {defaultRotation}");
            }
            else
            {
                Debug.LogError("PlayerManager: Obiekt gracza nie istnieje!");
            }
        }
    }
}