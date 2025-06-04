using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStartPositioner : MonoBehaviour
{
    private string targetSceneName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PositionPlayerOnSceneLoad(string sceneName)
    {
        targetSceneName = sceneName;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            Debug.Log($"Scena {scene.name} za�adowana. Ustawiam pozycj� gracza.");
            StartCoroutine(PositionPlayerAtStartPoint());
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator PositionPlayerAtStartPoint()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);

        if (Day.Instance != null)
        {
            Debug.Log("Resetuj� czas za pomoc� Day.Instance.");
            Day.Instance.ResetTime();
        }
        else
        {
            Debug.LogWarning("Nie znaleziono Day.Instance w scenie.");
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("player");
        if (playerObject == null)
        {
            Debug.LogWarning("Nie znaleziono gracza w scenie. Tworz� nowego gracza.");
            playerObject = new GameObject("Player");
            playerObject.tag = "player";
            playerObject.AddComponent<PlayerController>();
            DontDestroyOnLoad(playerObject);
        }
        else
        {
            Debug.Log($"Znaleziono gracza: {playerObject.name} na pozycji: {playerObject.transform.position}");
        }

        CharacterController controller = playerObject.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            Debug.Log("Znaleziono CharacterController i tymczasowo wy��czono.");
        }

        GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        if (startPoint != null)
        {
            playerObject.transform.position = startPoint.transform.position;
            playerObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log($"Gracz ustawiony na StartPoint: {startPoint.transform.position} z rotacj� (0, -180, 0)");
        }
        else
        {
            Debug.LogWarning("Nie znaleziono StartPoint w scenie. U�ywam domy�lnej pozycji.");
            playerObject.transform.position = new Vector3(0f, 1f, 0f);
            playerObject.transform.rotation = Quaternion.Euler(0, -180, 0);
            Debug.Log("Gracz ustawiony na domy�lnych wsp�rz�dnych: (0, 1, 0) z rotacj� (0, -180, 0)");
        }

        if (controller != null)
        {
            controller.enabled = true;
            Debug.Log("CharacterController ponownie w��czony.");
        }

        // Opcjonalnie: zniszcz ten obiekt po ustawieniu gracza
        // Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}