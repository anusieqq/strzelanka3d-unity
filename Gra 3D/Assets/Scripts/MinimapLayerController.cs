using UnityEngine;
using UnityEngine.SceneManagement;

public class MinimapLayerController : MonoBehaviour
{
    public Camera minimapCamera;
    public Transform player;
    public float thresholdY = 3.0f; // Granica miêdzy parterem a piêtrem

    public LayerMask parterMask;
    public LayerMask firstFloorMask;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ZnajdŸ gracza
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
            Debug.Log("MinimapLayerController: Player reference updated to " + player.name);
        }
        else
        {
            Debug.LogWarning("MinimapLayerController: PlayerController.Instance not found after scene load.");
        }

        // ZnajdŸ kamerê minimapy
        if (minimapCamera == null)
        {
            minimapCamera = GameObject.FindWithTag("MinimapCamera")?.GetComponent<Camera>();
            if (minimapCamera == null)
            {
                Debug.LogWarning("MinimapLayerController: MinimapCamera not found in scene: " + scene.name);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (player == null || minimapCamera == null)
        {
            if (PlayerController.Instance != null)
            {
                player = PlayerController.Instance.transform;
                Debug.Log("MinimapLayerController: Updated player reference to PlayerController.Instance.");
            }
            else
            {
                Debug.LogWarning("MinimapLayerController: Player or MinimapCamera is null!");
                return;
            }
        }

        if (player.position.y < thresholdY)
        {
            minimapCamera.cullingMask = parterMask;
        }
        else
        {
            minimapCamera.cullingMask = firstFloorMask;
        }
    }
}