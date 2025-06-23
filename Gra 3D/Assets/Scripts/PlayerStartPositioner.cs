using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStartPositioner : MonoBehaviour
{
    private static bool positionerExists = false;
    private string targetSceneName;
    private bool sceneLoaded = false;
    [SerializeField] private float sceneLoadDelay = 0.5f; // Konfigurowalne op�nienie w sekundach

    private void Awake()
    {
        // Zapobiegaj duplikacji positionera
        if (positionerExists)
        {
            Debug.LogWarning("PlayerStartPositioner ju� istnieje, niszczenie duplikatu.");
            Destroy(gameObject);
            return;
        }

        positionerExists = true;
        DontDestroyOnLoad(gameObject);
        Debug.Log("PlayerStartPositioner utworzony i ustawiony jako DontDestroyOnLoad.");
    }

    public void PositionPlayerOnSceneLoad(string sceneName)
    {
        // Rozpocznij proces �adowania sceny i ustawiania pozycji
        targetSceneName = sceneName;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"�adowanie sceny: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wywo�ywane, gdy scena zostanie za�adowana
        if (scene.name == targetSceneName)
        {
            sceneLoaded = true;
            Debug.Log($"Scena {scene.name} za�adowana, uruchamianie PositionPlayerRoutine.");
            StartCoroutine(PositionPlayerRoutine());
        }
        else
        {
            Debug.LogWarning($"Za�adowna scena ({scene.name}) nie pasuje do oczekiwanej ({targetSceneName}).");
        }
    }

    private IEnumerator PositionPlayerRoutine()
    {
        // Poczekaj na skonfigurowane op�nienie, aby scena si� w pe�ni za�adowa�a
        Debug.Log($"Czekanie na op�nienie �adowania sceny: {sceneLoadDelay}s");
        yield return new WaitForSeconds(sceneLoadDelay);

        // Poczekaj dodatkowe klatki dla pewno�ci
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Znajd� lub stw�rz gracza
        GameObject playerObject = FindOrCreatePlayer();
        if (playerObject == null)
        {
            Debug.LogError("Nie uda�o si� znale�� ani stworzy� obiektu gracza!");
            yield break;
        }

        // Znajd� StartPoint
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
            Debug.LogWarning($"StartPoint nie znaleziony, u�ywanie domy�lnej pozycji: {targetPosition}, rotacja: {targetRotation}");
        }

        // Sprawd�, czy pozycja jest prawid�owa
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
        {
            Debug.LogError($"Nieprawid�owa pozycja StartPointa: {targetPosition}, u�ywanie domy�lnej pozycji.");
            targetPosition = new Vector3(14.25f, -10.0f, 68.9f);
            targetRotation = Quaternion.Euler(0, -180, 0);
        }

        // Tymczasowo wy��cz CharacterController
        CharacterController controller = playerObject.GetComponent<CharacterController>();
        bool wasControllerEnabled = false;
        if (controller != null)
        {
            wasControllerEnabled = controller.enabled;
            controller.enabled = false;
            Debug.Log("CharacterController tymczasowo wy��czony.");
        }

        // Ustaw pozycj� i rotacj�
        playerObject.transform.position = targetPosition;
        playerObject.transform.rotation = targetRotation;
        Debug.Log($"Ustawiono pozycj� gracza: {playerObject.transform.position}, rotacja: {playerObject.transform.rotation}");

        // Zresetuj fizyk�
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
            Debug.Log("Fizyka Rigidbody zresetowana.");
        }

        // Ponownie w��cz CharacterController
        if (controller != null && wasControllerEnabled)
        {
            controller.enabled = true;
            Debug.Log("CharacterController ponownie w��czony.");
        }

        // Oznacz w PlayerPrefs, �e pozycja zosta�a ustawiona
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
        // Znajd� gracza lub stw�rz nowego, je�li nie istnieje
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