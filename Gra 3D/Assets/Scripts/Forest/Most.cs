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
    [SerializeField] private Canvas bridgeCanvas;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;

    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Button startButton;

    [Header("Position Settings")]
    [SerializeField] private float uiDistance = 2f;
    [SerializeField] private float uiHeight = 1.5f;
    [SerializeField] private float uiRightOffset = 0.3f;

    [Header("Player Height Offset")]
    [SerializeField] private float playerYOffset = 0.5f;

    [Header("Start Settings")]
    [SerializeField] private Transform startTile;


    private int currentStep = 0;
    private Transform[,] tilePositions = new Transform[9, 2];
    private bool bridgeActive = false;
    private Collider bridgeCollider;
    private bool hasStarted = false;

    private Vector3 startPos;
    private Quaternion startRot;

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

        startPos = transform.position;
        startRot = transform.rotation;

        bridgeCanvas.gameObject.SetActive(false);
        startCanvas.gameObject.SetActive(false);

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        if (hasStarted)
        {
            ActivateBridge();
        }
        else
        {
            startCanvas.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player")) return;

        DeactivateBridge();
        startCanvas.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnStartButtonClicked()
    {
        ActivateBridge();
        startCanvas.gameObject.SetActive(false);
        hasStarted = true;
    }

    void ActivateBridge()
    {
        if (bridgeActive) return;

        bridgeActive = true;
        SetBridgeUI(true);
        PositionUI();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void PositionUI()
    {
        if (player == null || playerCamera == null || bridgeCanvas == null) return;

        Vector3 uiPosition = player.position + player.forward * uiDistance + player.right * uiRightOffset;
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

        if (choice == correctPath[currentStep])
        {
            // Poprawny wybór
            Vector3 tilePos = tilePositions[currentStep, choice].position;

            Collider tileCollider = tilePositions[currentStep, choice].GetComponent<Collider>();
            float tileTopY = tileCollider != null ? tileCollider.bounds.max.y : tilePos.y;

            Collider playerCollider = player.GetComponent<Collider>();
            float playerBottomOffset = playerCollider != null ? playerCollider.bounds.min.y - player.position.y : 0f;

            Vector3 playerTargetPos = new Vector3(tilePos.x, tileTopY - playerBottomOffset, tilePos.z);
            player.position = playerTargetPos;

            // zatrzymaj fizykê
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // przesuñ most
            transform.position = new Vector3(tilePos.x, tileTopY + 0.2f, tilePos.z);

            currentStep++;

            if (currentStep >= correctPath.Length)
            {
                // Ukoñczono most
                CompleteBridge();
            }
            else
            {
                UpdateButtonLabels();
            }
        }
        else
        {
            Debug.Log("Z³y wybór! Restart mostu.");

            currentStep = 0;

            if (startTile != null)
            {
                float tileTopY = startTile.GetComponent<Collider>().bounds.max.y;

                Collider playerCollider = player.GetComponent<Collider>();
                float playerBottomOffset = playerCollider != null ? playerCollider.bounds.min.y - player.position.y : 0f;

                Vector3 safePlayerPos = new Vector3(startTile.position.x, tileTopY - playerBottomOffset, startTile.position.z);
                TeleportPlayer(safePlayerPos);

                // Cofnij most na pozycjê startow¹
                transform.position = startPos;
                transform.rotation = startRot;

                UpdateButtonLabels();
            }
        }


    }


    void TeleportPlayer(Vector3 targetPosition)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        player.position = targetPosition;

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    void CompleteBridge()
    {
        Debug.Log("Most ukoñczony pomyœlnie!", this);
        DeactivateBridge();
    }


    void DeactivateBridge()
    {
        if (!bridgeActive) return;

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
    }

    void SetBridgeUI(bool active)
    {
        if (bridgeCanvas == null) return;

        bridgeCanvas.gameObject.SetActive(active);

        foreach (Transform child in bridgeCanvas.transform)
        {
            child.gameObject.SetActive(active);
        }
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