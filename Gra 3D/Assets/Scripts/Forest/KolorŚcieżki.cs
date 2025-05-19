using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class KolorŚcieżki : MonoBehaviour
{
    public GameObject buttonOpen;
    public GameObject buttonStart;
    public GameObject CanvasPanel;
    public Animator boxanimator;
    public Light[] signalLights;
    public Transform startPathPoint;

    public float greenDuration = 5f;
    public float redDuration = 3f;
    private int requiredRoundsToWin = 1;

    private bool playerInTrigger = false;
    private bool puzzleActive = false;
    private bool isGreenLight = false;

    private Transform playerTransform;
    private Vector3 lastPosition;
    private int completedRounds = 0;

    // Dodane dla debugowania
    public Text debugText;
    private string debugMessage = "";

    void Start()
    {
        Debug.Log("Initializing KolorŚcieżki script...");

        if (buttonOpen == null || buttonStart == null || CanvasPanel == null || boxanimator == null)
        {
            Debug.LogError("Niektóre obiekty UI lub animator nie są przypisane w Inspectorze!");
        }

        if (EventSystem.current == null)
        {
            Debug.LogError("Brakuje EventSystem w scenie! Dodaj GameObject -> UI -> EventSystem.");
        }
        else
        {
            Debug.Log("EventSystem found: " + EventSystem.current.gameObject.name);
        }

        buttonOpen.SetActive(false);
        buttonStart.SetActive(false);
        CanvasPanel.SetActive(false);

        // Sprawdź czy przyciski mają komponenty Button
        Button openButton = buttonOpen.GetComponent<Button>();
        Button startButton = buttonStart.GetComponent<Button>();

        if (openButton == null || startButton == null)
        {
            Debug.LogError("Przyciski nie mają komponentu Button!");
        }
        else
        {
            openButton.onClick.AddListener(ShowCanvasPanel);
            startButton.onClick.AddListener(StartPuzzle);
            Debug.Log("Button listeners added successfully");
        }

        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("Player found: " + player.name);
        }
        else
        {
            Debug.LogError("Nie znaleziono gracza z tagiem 'player'!");
        }

        // Sprawdź collidery
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Brak collidera na obiekcie!");
        }
        else
        {
            Debug.Log("Collider found: " + collider + ", isTrigger: " + collider.isTrigger);
            collider.isTrigger = true; // Upewnij się, że collider jest triggerem
        }

        UpdateDebugText("Initialization complete");
    }

    void Update()
    {
        // Debugowanie stanu UI
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Current UI state - buttonOpen: " + buttonOpen.activeSelf +
                     ", buttonStart: " + buttonStart.activeSelf +
                     ", CanvasPanel: " + CanvasPanel.activeSelf);

            Debug.Log("EventSystem current selected: " + (EventSystem.current.currentSelectedGameObject != null ?
                     EventSystem.current.currentSelectedGameObject.name : "null"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            playerInTrigger = true;
            buttonOpen.SetActive(true);
            CanvasPanel.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("Player entered trigger");
            UpdateDebugText("Player entered trigger - showing buttonOpen");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            playerInTrigger = false;
            buttonOpen.SetActive(false);
            buttonStart.SetActive(false);
            CanvasPanel.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Debug.Log("Player exited trigger");
            UpdateDebugText("Player exited trigger - hiding all UI");
        }
    }

    public void ShowCanvasPanel()
    {
        Debug.Log("Open button clicked");

        if (boxanimator != null)
        {
            boxanimator.SetTrigger("Open Box");
            Debug.Log("Triggered 'Open Box' animation");
        }
        else
        {
            Debug.LogWarning("Animator nie został przypisany!");
        }

        if (CanvasPanel != null)
        {
            CanvasPanel.SetActive(true);
            Debug.Log("CanvasPanel activated");
        }
        else
        {
            Debug.LogWarning("CanvasPanel nie został przypisany!");
        }

        if (buttonStart != null)
        {
            buttonStart.SetActive(true);
            Debug.Log("buttonStart activated");

            // Upewnij się, że przycisk jest interaktywny
            Button startButton = buttonStart.GetComponent<Button>();
            if (startButton != null)
            {
                startButton.interactable = true;
                Debug.Log("buttonStart is interactable");
            }
        }

        if (buttonOpen != null)
        {
            buttonOpen.SetActive(false);
            Debug.Log("buttonOpen deactivated");
        }

        UpdateDebugText("CanvasPanel shown - ready to start puzzle");
    }

    public void StartPuzzle()
    {
        Debug.Log("Start button clicked");

        if (!puzzleActive)
        {
            puzzleActive = true;
            completedRounds = 0;

            if (buttonStart != null)
            {
                buttonStart.SetActive(false);
                Debug.Log("buttonStart deactivated");
            }

            if (CanvasPanel != null)
            {
                CanvasPanel.SetActive(false);
                Debug.Log("CanvasPanel deactivated");
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Debug.Log("Starting puzzle routine");
            UpdateDebugText("Puzzle started - green light phase");
            StartCoroutine(LightPuzzleRoutine());
        }
    }

    IEnumerator LightPuzzleRoutine()
    {
        while (puzzleActive)
        {
            // Zielone światło
            isGreenLight = true;
            SetLightsColor(Color.green);
            Debug.Log("Zielone światło – możesz się poruszać.");
            UpdateDebugText("Green light - you can move");
            yield return new WaitForSeconds(greenDuration);

            // Czerwone światło
            isGreenLight = false;
            SetLightsColor(Color.red);
            Debug.Log("Czerwone światło – NIE ruszaj się!");
            UpdateDebugText("Red light - DON'T move!");
            lastPosition = playerTransform.position;

            yield return new WaitForSeconds(redDuration);

            Vector3 currentPosition = playerTransform.position;

            // Porównanie tylko po osi X
            if (Mathf.Abs(currentPosition.x - lastPosition.x) > 0.05f)
            {
                Debug.Log("Gracz poruszył się podczas czerwonego światła! Cofnięcie na początek.");
                UpdateDebugText("Player moved! Resetting position.");
                playerTransform.position = startPathPoint.position;
                playerTransform.rotation = startPathPoint.rotation;

                yield return new WaitForSeconds(3f);
                completedRounds = 0;
                continue;
            }
            else
            {
                completedRounds++;
                Debug.Log("Udało się! Przetrwałeś rundę: " + completedRounds);
                UpdateDebugText("Round survived: " + completedRounds);
            }

            if (completedRounds >= requiredRoundsToWin)
            {
                Debug.Log("Zagadkę rozwiązano poprawnie!");
                UpdateDebugText("Puzzle completed successfully!");
                puzzleActive = false;
                SetLightsColor(Color.white);
                yield break;
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private void SetLightsColor(Color color)
    {
        foreach (Light light in signalLights)
        {
            if (light != null)
                light.color = color;
        }
    }

    // Metoda do aktualizacji tekstu debugowego
    private void UpdateDebugText(string message)
    {
        debugMessage = message;
        if (debugText != null)
        {
            debugText.text = debugMessage;
        }
        Debug.Log(message);
    }

    // Metoda do wyświetlania debug info w GUI
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 500, 30), debugMessage, style);
    }
}