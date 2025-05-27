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
    public GameObject OpenPanel;
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

    public Text debugText;
    private string debugMessage = "";

    void Start()
    {
        Debug.Log("Initializing KolorŚcieżki script...");

        // Sprawdzenie przypisań
        if (buttonOpen == null || buttonStart == null || CanvasPanel == null || boxanimator == null)
        {
            Debug.LogError("Niektóre obiekty UI lub animator nie są przypisane w Inspectorze!");
        }

        if (EventSystem.current == null)
        {
            Debug.LogError("Brakuje EventSystem w scenie!");
        }

        // Wyłącz UI na start
        buttonOpen.SetActive(false);
        buttonStart.SetActive(false);
        CanvasPanel.SetActive(false);
        OpenPanel.SetActive(false);

        // Obsługa przycisków
        Button openButton = buttonOpen.GetComponent<Button>();
        Button startButton = buttonStart.GetComponent<Button>();

        if (openButton != null && startButton != null)
        {
            openButton.onClick.AddListener(ShowCanvasPanel);
            startButton.onClick.AddListener(StartPuzzle);
        }
        else
        {
            Debug.LogError("Brakuje komponentów Button!");
        }

        // Znajdź gracza
        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Nie znaleziono gracza z tagiem 'player'!");
        }

        // Ustaw trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("Brak collidera na obiekcie!");
        }

        UpdateDebugText("Initialization complete");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("UI states: Open:" + buttonOpen.activeSelf + ", Start:" + buttonStart.activeSelf + ", Panel:" + CanvasPanel.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            playerInTrigger = true;

            buttonOpen.SetActive(true);
            CanvasPanel.SetActive(false);
            OpenPanel.SetActive(true);

            StartCoroutine(ShowCursorWithDelay());

            Debug.Log("Player entered trigger");
            UpdateDebugText("Player in trigger – showing buttonOpen");
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
            OpenPanel.SetActive(false);

            HideCursor();

            Debug.Log("Player exited trigger");
            UpdateDebugText("Player left trigger – hiding UI");
        }
    }

    public void ShowCanvasPanel()
    {
        Debug.Log("Open button clicked");

        if (boxanimator != null)
        {
            boxanimator.SetTrigger("Open Box");
        }

        CanvasPanel.SetActive(true);
        buttonStart.SetActive(true);
        buttonOpen.SetActive(false);

        Button startButton = buttonStart.GetComponent<Button>();
        if (startButton != null)
        {
            startButton.interactable = true;
        }

        StartCoroutine(ShowCursorWithDelay());
        UpdateDebugText("Canvas shown – ready to start puzzle");
    }

    public void StartPuzzle()
    {
        if (!puzzleActive)
        {
            puzzleActive = true;
            completedRounds = 0;

            buttonStart.SetActive(false);
            CanvasPanel.SetActive(false);

            HideCursor();
            UpdateDebugText("Puzzle started");

            StartCoroutine(LightPuzzleRoutine());
        }
    }

    IEnumerator LightPuzzleRoutine()
    {
        while (puzzleActive)
        {
            // GREEN light
            isGreenLight = true;
            SetLightsColor(Color.green);
            UpdateDebugText("Green light – move!");
            yield return new WaitForSeconds(greenDuration);

            // RED light
            isGreenLight = false;
            SetLightsColor(Color.red);
            UpdateDebugText("Red light – STOP!");
            lastPosition = playerTransform.position;
            yield return new WaitForSeconds(redDuration);

            Vector3 currentPosition = playerTransform.position;
            if (Mathf.Abs(currentPosition.x - lastPosition.x) > 0.05f)
            {
                UpdateDebugText("You moved! Resetting...");
                playerTransform.position = startPathPoint.position;
                playerTransform.rotation = startPathPoint.rotation;
                completedRounds = 0;
                yield return new WaitForSeconds(3f);
                continue;
            }

            completedRounds++;
            UpdateDebugText("Round survived: " + completedRounds);

            if (completedRounds >= requiredRoundsToWin)
            {
                UpdateDebugText("Puzzle completed!");
                SetLightsColor(Color.white);
                puzzleActive = false;
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

    private void UpdateDebugText(string message)
    {
        debugMessage = message;
        if (debugText != null)
        {
            debugText.text = debugMessage;
        }
        Debug.Log(message);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 20,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(10, 10, 500, 30), debugMessage, style);
    }

    // Nowa metoda: pokazywanie kursora z delikatnym opóźnieniem
    private IEnumerator ShowCursorWithDelay()
    {
        yield return null; // poczekaj jedną klatkę
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Nowa metoda: ukrywanie kursora
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
