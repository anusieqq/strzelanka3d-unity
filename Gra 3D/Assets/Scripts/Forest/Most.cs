using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Most : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;

    [Header("Tiles Setup")]
    [SerializeField] private Transform[] tiles;
    [SerializeField] private int[] correctPath = new int[9];

    [Header("UI Settings")]
    [SerializeField] private Canvas bridgeCanvas; // UI z przyciskami lewy/prawy
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;

    [SerializeField] private Canvas startCanvas; // UI tylko z przyciskiem Start
    [SerializeField] private Button startButton;

    [Header("Position Settings")]
    [SerializeField] private float uiDistance = 2f;
    [SerializeField] private float uiHeight = 1.5f;
    [SerializeField] private float uiRightOffset = 0.3f;

    [Header("Player Height Offset")]
    [SerializeField] private float playerYOffset = 0.5f;

    private int currentStep = 0;
    private Transform[,] tilePositions = new Transform[9, 2];
    private bool bridgeActive = false;
    private Collider bridgeCollider;

    void Awake()
    {
        bridgeCollider = GetComponent<Collider>();
        if (bridgeCollider == null)
        {
            Debug.LogError("Brak komponentu Collider na moœcie!", this);
            enabled = false;
            return;
        }

        if (!bridgeCollider.isTrigger)
        {
            Debug.LogWarning("Collider mostu nie jest ustawiony jako Trigger!", this);
            bridgeCollider.isTrigger = true;
        }
    }

    void Start()
    {
        InitializePlayer();
        InitializeTiles();
        SetupButtons();

        bridgeCanvas.gameObject.SetActive(false); // Ukryj UI mostu
        startCanvas.gameObject.SetActive(false);  // Ukryj UI startowe

        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        Debug.Log("Most system initialized", this);
    }

    void InitializePlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Znaleziono gracza: " + player.name, this);
            }
            else
            {
                Debug.LogError("Nie znaleziono obiektu z tagiem 'player'!", this);
                enabled = false;
                return;
            }
        }

        if (playerCamera == null)
        {
            playerCamera = player.GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("Nie znaleziono kamery gracza!", this);
                enabled = false;
            }
        }
    }

    void InitializeTiles()
    {
        if (tiles.Length != 18)
        {
            Debug.LogError("Wymagane jest dok³adnie 18 kafelków! Obecnie: " + tiles.Length, this);
            enabled = false;
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            tilePositions[i, 0] = tiles[i * 2];
            tilePositions[i, 1] = tiles[i * 2 + 1];
        }
        Debug.Log("Zainicjalizowano kafelki mostu", this);
    }

    void SetupButtons()
    {
        if (leftButton == null || rightButton == null)
        {
            Debug.LogError("Przyciski nie s¹ przypisane!", this);
            enabled = false;
            return;
        }

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(() => SelectTile(0));
        rightButton.onClick.AddListener(() => SelectTile(1));

        UpdateButtonLabels();
        Debug.Log("Przyciski zainicjalizowane", this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        Debug.Log("Gracz wszed³ w obszar mostu: " + other.name, this);
        startCanvas.gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player")) return;

        Debug.Log("Gracz opuœci³ obszar mostu: " + other.name, this);
        DeactivateBridge();
        startCanvas.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnStartButtonClicked()
    {
        ActivateBridge(); // w³¹cz przyciski
        startCanvas.gameObject.SetActive(false); // schowaj przycisk Start
    }

    void ActivateBridge()
    {
        if (bridgeActive) return;

        Debug.Log("Aktywacja mostu", this);
        bridgeActive = true;

        SetBridgeUI(true);
        PositionUI();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void PositionUI()
    {
        if (player == null || playerCamera == null || bridgeCanvas == null) return;

        Vector3 uiPosition = player.position
                           + player.forward * uiDistance
                           + player.right * uiRightOffset;
        uiPosition.y = player.position.y + uiHeight;

        bridgeCanvas.transform.position = uiPosition;
        bridgeCanvas.transform.LookAt(playerCamera.transform);
        bridgeCanvas.transform.rotation *= Quaternion.Euler(0, 180, 0);
        bridgeCanvas.transform.rotation *= Quaternion.Euler(10, 0, 0);
    }

    void Update()
    {
        if (bridgeActive)
        {
            PositionUI();
        }
    }

    void SelectTile(int choice)
    {
        if (!bridgeActive) return;

        // SprawdŸ czy wybrano poprawny kafelek
        if (choice == correctPath[currentStep])
        {
            Vector3 tilePos = tilePositions[currentStep, choice].position;

            Collider tileCollider = tilePositions[currentStep, choice].GetComponent<Collider>();
            float tileTopY = tilePos.y;

            if (tileCollider != null)
            {
                tileTopY = tileCollider.bounds.max.y;
            }

            Collider playerCollider = player.GetComponent<Collider>();
            float playerBottomOffset = 0f;

            if (playerCollider != null)
            {
                playerBottomOffset = playerCollider.bounds.min.y - player.position.y;
            }

            Vector3 playerTargetPos = new Vector3(tilePos.x, tileTopY - playerBottomOffset, tilePos.z);
            player.position = playerTargetPos;

            // Przesuñ most
            transform.position = new Vector3(tilePos.x, 117f, tilePos.z);

            currentStep++;

            if (currentStep >= correctPath.Length)
            {
                CompleteBridge();
            }
            else
            {
                UpdateButtonLabels(); // <- AKTUALIZUJEMY NUMERY
            }
        }
        else
        {
            ResetBridge();
        }
    }




    void ResetBridge()
    {
        Debug.Log("Resetowanie mostu", this);

        player.position = transform.position;
        currentStep = 0;
        UpdateButtonLabels();
    }

    void CompleteBridge()
    {
        Debug.Log("Most ukoñczony pomyœlnie!", this);
        DeactivateBridge();
    }

    void DeactivateBridge()
    {
        if (!bridgeActive) return;

        Debug.Log("Deaktywacja mostu", this);
        bridgeActive = false;
        SetBridgeUI(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateButtonLabels()
    {
        if (leftButtonText == null || rightButtonText == null) return;

        leftButtonText.text = (currentStep * 2 + 1).ToString();
        rightButtonText.text = (currentStep * 2 + 2).ToString();

        Debug.Log($"Zaktualizowano etykiety: L={leftButtonText.text}, R={rightButtonText.text}", this);
    }

    void SetBridgeUI(bool active)
    {
        if (bridgeCanvas == null) return;

        bridgeCanvas.gameObject.SetActive(active);

        foreach (Transform child in bridgeCanvas.transform)
        {
            child.gameObject.SetActive(active);
        }

        Debug.Log("UI mostu: " + (active ? "aktywne" : "nieaktywne"), this);
    }

    void OnValidate()
    {
        if (correctPath.Length != 9)
        {
            System.Array.Resize(ref correctPath, 9);
            Debug.LogWarning("Poprawiono d³ugoœæ tablicy correctPath na 9", this);
        }
    }
}
