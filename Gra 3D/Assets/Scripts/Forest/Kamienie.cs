using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamienie : MonoBehaviour
{
    [System.Serializable]
    public class Stone
    {
        public GameObject stoneObject;
        public Light stoneLight;
    }

    public List<Stone> stones = new List<Stone>();
    public List<Color> availableColors = new List<Color>();
    public float lightOnDuration = 1.0f;
    public float delayBetweenLights = 0.5f;
    public Canvas canvas;
    public GameObject button;

    private List<Stone> sequence = new List<Stone>();
    private List<Color> colorSequence = new List<Color>();
    private int currentInputIndex = 0;
    private bool playerTurn = false;
    private bool gameStarted = false;
    private Stone hoveredStone = null;

    private float currentLightOnDuration;
    private float currentDelayBetweenLights;
    public float speedUpFactor = 0.85f; // 15% szybsze wyœwietlanie po b³êdzie

    void Start()
    {
        canvas.gameObject.SetActive(false);
        TurnOffAllLights();

        currentLightOnDuration = lightOnDuration;
        currentDelayBetweenLights = delayBetweenLights;
    }

    void TurnOffAllLights()
    {
        foreach (var stone in stones)
        {
            if (stone.stoneLight != null)
                stone.stoneLight.enabled = false;

            if (stone.stoneObject != null)
            {
                Renderer rend = stone.stoneObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    Material mat = rend.material;
                    mat.color = Color.white;
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    public void StartGame()
    {
        canvas.gameObject.SetActive(false);
        if (!gameStarted)
        {
            StartCoroutine(PlaySequence());
            gameStarted = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            canvas.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player"))
            return;

        // Ukrywaj canvas tylko jeœli gra siê nie toczy
        if (!playerTurn)
        {
            canvas.gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(1f);

        sequence.Clear();
        colorSequence.Clear();

        // Lista dostêpnych kamieni (niezniszczonych)
        List<Stone> availableStones = stones.FindAll(s => s.stoneObject != null);

        if (availableStones.Count == 0)
        {
            Debug.Log("Wszystkie kamienie zniszczone, koniec gry.");
            yield break;
        }

        // D³ugoœæ sekwencji – mo¿e byæ mniejsza lub równa liczbie dostêpnych kamieni
        int seqLength = Mathf.Min(availableStones.Count, stones.Count);

        // Losowa kolejnoœæ sekwencji:
        List<Stone> shuffledStones = new List<Stone>(availableStones);
        ShuffleList(shuffledStones);

        for (int i = 0; i < seqLength; i++)
        {
            Stone stone = shuffledStones[i];
            Color color = availableColors[i % availableColors.Count];

            sequence.Add(stone);
            colorSequence.Add(color);

            // Zapal œwiat³o
            stone.stoneLight.color = color;
            stone.stoneLight.enabled = true;

            // Materia³ z emisj¹
            Renderer rend = stone.stoneObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                mat.color = color;
                mat.SetColor("_EmissionColor", color);
                mat.EnableKeyword("_EMISSION");
            }

            yield return new WaitForSeconds(currentLightOnDuration);

            // Zgaœ œwiat³o, materia³y na bia³o
            stone.stoneLight.enabled = false;

            Renderer rendOff = stone.stoneObject.GetComponent<Renderer>();
            if (rendOff != null)
            {
                Material mat = rendOff.material;
                mat.color = Color.white;
                mat.DisableKeyword("_EMISSION");
            }

            yield return new WaitForSeconds(currentDelayBetweenLights);
        }

        // Zapal wszystkie na koniec z kolorem i emisj¹
        for (int i = 0; i < sequence.Count; i++)
        {
            Stone stone = sequence[i];
            Color color = colorSequence[i];

            stone.stoneLight.color = color;
            stone.stoneLight.enabled = true;

            Renderer rend = stone.stoneObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                mat.color = color;
                mat.SetColor("_EmissionColor", color);
                mat.EnableKeyword("_EMISSION");
            }
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerTurn = true;
        currentInputIndex = 0;
    }

    void Update()
    {
        if (playerTurn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Stone newHover = null;

                foreach (var stone in stones)
                {
                    if (stone != null && stone.stoneObject == hit.transform.gameObject)
                    {
                        newHover = stone;
                        break;
                    }
                }

                if (newHover != hoveredStone)
                {
                    ResetHoverEffect();
                    hoveredStone = newHover;
                    ApplyHoverEffect();
                }

                if (Input.GetMouseButtonDown(0) && hoveredStone != null)
                {
                    CheckPlayerInput(hoveredStone);
                }
            }
            else
            {
                ResetHoverEffect();
                hoveredStone = null;
            }
        }
    }

    void CheckPlayerInput(Stone clickedStone)
    {
        if (clickedStone == sequence[currentInputIndex])
        {
            // Poprawne klikniêcie – niszcz kamieñ
            Destroy(clickedStone.stoneObject);

            currentInputIndex++;

            if (currentInputIndex >= sequence.Count)
            {
                Debug.Log("Sukces!");
                playerTurn = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            Debug.Log("B³¹d! Restart sekwencji z pozosta³ych kamieni i szybsze tempo.");

            playerTurn = false;

            // Przyspieszamy wyœwietlanie sekwencji
            currentLightOnDuration *= speedUpFactor;
            currentDelayBetweenLights *= speedUpFactor;

            StartCoroutine(RestartSequenceAfterDelay());
        }
    }

    IEnumerator RestartSequenceAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        TurnOffAllLights();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(PlaySequence());
    }

    void ResetGame()
    {
        currentInputIndex = 0;
        sequence.Clear();
        colorSequence.Clear();
        TurnOffAllLights();
        gameStarted = false;
        canvas.gameObject.SetActive(true);

        currentLightOnDuration = lightOnDuration;
        currentDelayBetweenLights = delayBetweenLights;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ApplyHoverEffect()
    {
        if (hoveredStone != null && hoveredStone.stoneObject != null)
        {
            Renderer rend = hoveredStone.stoneObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                mat.SetColor("_EmissionColor", Color.white * 0.6f);
                mat.EnableKeyword("_EMISSION");
            }
        }
    }

    void ResetHoverEffect()
    {
        if (hoveredStone != null && hoveredStone.stoneObject != null)
        {
            int index = sequence.IndexOf(hoveredStone);
            if (index >= 0 && index < colorSequence.Count)
            {
                Renderer rend = hoveredStone.stoneObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    Color originalColor = colorSequence[index];
                    Material mat = rend.material;
                    mat.SetColor("_EmissionColor", originalColor);
                    mat.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    // Pomocnicza metoda do tasowania listy
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
