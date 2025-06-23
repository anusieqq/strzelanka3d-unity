using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStartPositioner : MonoBehaviour
{
    private static bool positionerExists = false;
    private string targetSceneName;
    private bool sceneLoaded = false;
    [SerializeField] private float sceneLoadDelay = 0.5f; // Konfigurowalne opóŸnienie w sekundach

    private void Awake()
    {
        // Zapobiegaj duplikacji positionera
        if (positionerExists)
        {
            Debug.LogWarning("PlayerStartPositioner ju¿ istnieje, niszczenie duplikatu.");
            Destroy(gameObject);
            return;
        }

        positionerExists = true;
        DontDestroyOnLoad(gameObject);
        Debug.Log("PlayerStartPositioner utworzony i ustawiony jako DontDestroyOnLoad.");
    }

    public void PositionPlayerOnSceneLoad(string sceneName)
    {
        // Rozpocznij proces ³adowania sceny i ustawiania pozycji
        targetSceneName = sceneName;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"£adowanie sceny: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wywo³ywane, gdy scena zostanie za³adowana
        if (scene.name == targetSceneName)
        {
            sceneLoaded = true;
            Debug.Log($"Scena {scene.name} za³adowana, uruchamianie PositionPlayerRoutine.");
            StartCoroutine(PositionPlayerRoutine());
        }
        else
        {
            Debug.LogWarning($"Za³adowna scena ({scene.name}) nie pasuje do oczekiwanej ({targetSceneName}).");
        }
    }

    private IEnumerator PositionPlayerRoutine()
    {
        // Poczekaj na skonfigurowane opóŸnienie, aby scena siê w pe³ni za³adowa³a
        Debug.Log($"Czekanie na opóŸnienie ³adowania sceny: {sceneLoadDelay}s");
        yield return new WaitForSeconds(sceneLoadDelay);

        // Poczekaj dodatkowe klatki dla pewnoœci
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // ZnajdŸ lub stwórz gracza
        GameObject playerObject = FindOrCreatePlayer();
        if (playerObject == null)
        {
            Debug.LogError("Nie uda³o siê znaleŸæ ani stworzyæ obiektu gracza!");
            yield break;
        }

        // ZnajdŸ StartPoint
        GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        Vector3 targetPosition;
        Quaternion targetRotation;

        if (startPoint != null)
        {
            targetPosition = startPoint.transform.position;
            targetRotation = startPoint.transform.rotation;
            Debug.Log($"Znaleziono StartPoint w pozycji: {targetPosition}, rotacja: {targetRotation}");
        }
        else
        {
            targetPosition = new Vector3(14.25f, 3.8f, 68.9f);
            targetRotation = Quaternion.Euler(0, -180, 0);
            Debug.LogWarning($"StartPoint nie znaleziony, u¿ywanie domyœlnej pozycji: {targetPosition}, rotacja: {targetRotation}");
        }

        // SprawdŸ, czy pozycja jest prawid³owa
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
        {
            Debug.LogError($"Nieprawid³owa pozycja StartPointa: {targetPosition}, u¿ywanie domyœlnej pozycji.");
            targetPosition = new Vector3(14.25f, -10.0f, 68.9f);
            targetRotation = Quaternion.Euler(0, -180, 0);
        }

        // Tymczasowo wy³¹cz CharacterController
        CharacterController controller = playerObject.GetComponent<CharacterController>();
        bool wasControllerEnabled = false;
        if (controller != null)
        {
            wasControllerEnabled = controller.enabled;
            controller.enabled = false;
            Debug.Log("CharacterController tymczasowo wy³¹czony.");
        }

        // Ustaw pozycjê i rotacjê
        playerObject.transform.position = targetPosition;
        playerObject.transform.rotation = targetRotation;
        Debug.Log($"Ustawiono pozycjê gracza: {playerObject.transform.position}, rotacja: {playerObject.transform.rotation}");

        // Zresetuj fizykê
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
            Debug.Log("Fizyka Rigidbody zresetowana.");
        }

        // Ponownie w³¹cz CharacterController
        if (controller != null && wasControllerEnabled)
        {
            controller.enabled = true;
            Debug.Log("CharacterController ponownie w³¹czony.");
        }

        // Oznacz w PlayerPrefs, ¿e pozycja zosta³a ustawiona
        PlayerPrefs.SetInt("PositionSetByPositioner", 1);
        PlayerPrefs.Save();
        Debug.Log("Zapisano w PlayerPrefs: PositionSetByPositioner = 1");

        // Zniszcz ten obiekt
        positionerExists = false;
        Debug.Log("Niszczenie PlayerStartPositioner.");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }

    private GameObject FindOrCreatePlayer()
    {
        // ZnajdŸ gracza lub stwórz nowego, jeœli nie istnieje
        GameObject playerObject = GameObject.FindGameObjectWithTag("player");

        if (playerObject == null)
        {
            Debug.LogWarning("Gracz nie znaleziony, tworzenie nowego obiektu gracza.");
            playerObject = new GameObject("Player");
            playerObject.tag = "player";
            playerObject.AddComponent<PlayerController>();

            // Dodaj wymagane komponenty
            if (playerObject.GetComponent<Rigidbody>() == null)
                playerObject.AddComponent<Rigidbody>();

            if (playerObject.GetComponent<CharacterController>() == null)
                playerObject.AddComponent<CharacterController>();
        }
        else
        {
            Debug.Log($"Znaleziono obiekt gracza: {playerObject.name}");
        }

        return playerObject;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("PlayerStartPositioner zniszczony.");
    }
}