using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }
    public bool IsServerRunning { get; private set; } = false;
    public Button serverButton;
    public TMP_Text serverText;
    private bool isPendriveCollected = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log($"ServerManager Awake: Instance = {(Instance != null)}, na obiekcie: {gameObject.name}");
    }

    void Start()
    {
        if (serverButton != null)
        {
            serverButton.gameObject.SetActive(false);
            serverButton.onClick.RemoveAllListeners();
            serverButton.onClick.AddListener(StartServer);
            Debug.Log("ServerManager: ServerButton przypisany i pod³¹czony do StartServer.");
        }
        else
        {
            Debug.LogError($"ServerManager: ServerButton nie jest przypisany w {gameObject.name}");
        }

        if (serverText != null)
        {
            serverText.text = "Serwer";
            serverText.gameObject.SetActive(true);
            Debug.Log($"ServerManager: Napis 'Serwer' wyœwietlony na {serverText.gameObject.name}");
        }
        else
        {
            Debug.LogError($"ServerManager: ServerText nie jest przypisany w {gameObject.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ServerManager: OnTriggerEnter na {gameObject.name}, kolizja z: {other.gameObject.name}, tag: {other.tag}");
        Debug.Log($"ServerManager: Warunki - isPendriveCollected: {isPendriveCollected}, IsServerRunning: {IsServerRunning}");
        if (other.CompareTag("player"))
        {
            if (isPendriveCollected && !IsServerRunning)
            {
                if (serverButton != null)
                {
                    serverButton.gameObject.SetActive(true);
                    Debug.Log("ServerManager: Wykryto kolizjê z graczem, pendrive zebrany, serwer nie uruchomiony, pokazano serverButton.");
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Debug.LogError("ServerManager: ServerButton jest null, nie mo¿na pokazaæ przycisku!");
                }
                if (serverText != null)
                {
                    serverText.text = "Uruchom serwer!";
                    Debug.Log("ServerManager: Wyœwietlono komunikat 'Uruchom serwer!'");
                }
            }
            else if (!isPendriveCollected)
            {
                if (serverText != null)
                {
                    serverText.text = "Zdob¹dŸ pendrive'a!";
                    Debug.Log("ServerManager: Wyœwietlono komunikat 'Zdob¹dŸ pendrive'a!'");
                }
            }
            else if (IsServerRunning)
            {
                if (serverText != null)
                {
                    serverText.text = "Serwer uruchomiony";
                    Debug.Log("ServerManager: Wyœwietlono komunikat 'Serwer uruchomiony'");
                }
            }
        }
        else
        {
            Debug.LogWarning($"ServerManager: Kolizja z obiektem bez tagu 'player': {other.tag}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (serverButton != null)
            {
                serverButton.gameObject.SetActive(false);
                Debug.Log("ServerManager: Gracz opuœci³ obiekt serwera, ukryto serverButton.");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (serverText != null && !IsServerRunning)
            {
                serverText.text = "Serwer";
                Debug.Log("ServerManager: Przywrócono napis 'Serwer' po opuszczeniu obszaru.");
            }
        }
    }

    public void SetPendriveCollected()
    {
        isPendriveCollected = true;
        Debug.Log($"ServerManager: Pendrive zebrany. isPendriveCollected = {isPendriveCollected}");
    }

    public void StartServer()
    {
        if (!isPendriveCollected)
        {
            Debug.LogWarning("ServerManager: Nie mo¿esz uruchomiæ serwera bez pendrive'a!");
            return;
        }

        IsServerRunning = true;
        if (serverButton != null)
        {
            serverButton.gameObject.SetActive(false);
            Debug.Log("ServerManager: Serwer uruchomiony, ukryto serverButton.");
        }
        if (serverText != null)
        {
            serverText.text = "Serwer uruchomiony";
            Debug.Log("ServerManager: Napis zmieniony na 'Serwer uruchomiony'.");
        }
        Debug.Log($"ServerManager: Serwer uruchomiony! IsServerRunning = {IsServerRunning}");
    }
}