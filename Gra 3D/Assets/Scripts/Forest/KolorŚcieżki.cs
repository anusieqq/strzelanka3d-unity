using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        buttonOpen.SetActive(false);
        buttonStart.SetActive(false);
        CanvasPanel.SetActive(false);

        buttonOpen.GetComponent<Button>().onClick.AddListener(ShowCanvasPanel);
        buttonStart.GetComponent<Button>().onClick.AddListener(StartPuzzle);

        GameObject player = GameObject.FindGameObjectWithTag("player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Nie znaleziono gracza z tagiem 'player'!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            buttonOpen.SetActive(true);
            CanvasPanel.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("Kolizja ze skrzynką");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            buttonOpen.SetActive(false);
            buttonStart.SetActive(false);
            CanvasPanel.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ShowCanvasPanel()
    {
       
            boxanimator.SetTrigger("Open Box");
            CanvasPanel.SetActive(true);
            buttonOpen.SetActive(false);
            buttonStart.SetActive(true);
        
    }

    public void StartPuzzle()
    {
        if (!puzzleActive)
        {
            puzzleActive = true;
            completedRounds = 0;
            buttonStart.SetActive(false);

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
            yield return new WaitForSeconds(greenDuration);

            // Czerwone światło
            isGreenLight = false;
            SetLightsColor(Color.red);
            Debug.Log("Czerwone światło – NIE ruszaj się!");
            lastPosition = playerTransform.position;

            yield return new WaitForSeconds(redDuration);

            Vector3 currentPosition = playerTransform.position;

            // Porównanie tylko po osi X
            if (Mathf.Abs(currentPosition.x - lastPosition.x) > 0.05f)
            {
                Debug.Log("Gracz poruszył się podczas czerwonego światła! Cofnięcie na początek.");
                playerTransform.position = startPathPoint.position;
                playerTransform.rotation = startPathPoint.rotation;

                yield return new WaitForSeconds(3f); // czekamy przed nową próbą
                completedRounds = 0; // resetujemy postęp
                continue; 
            }
            else
            {
                completedRounds++;
                Debug.Log("Udało się! Przetrwałeś rundę: " + completedRounds);
            }

            if (completedRounds >= requiredRoundsToWin)
            {
                Debug.Log("Zagadkę rozwiązano poprawnie!");
                puzzleActive = false;
                SetLightsColor(Color.white);
                yield break;
            }

            yield return new WaitForSeconds(3f); // Czekanie 3 sekundy przed kolejną rundą
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
}
