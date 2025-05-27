using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinimapTextureSwitcher : MonoBehaviour
{
    private RawImage minimapImage;
    private static MinimapTextureSwitcher instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedTextureSwitch(scene.name));
    }

    private IEnumerator DelayedTextureSwitch(string sceneName)
    {
        yield return null;

        // ZnajdŸ komponent minimapy
        GameObject minimapObject = GameObject.Find("Minimap");
        if (minimapObject == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu minimapy w scenie!");
            yield break;
        }

        minimapImage = minimapObject.GetComponent<RawImage>();
        if (minimapImage == null)
        {
            Debug.LogWarning("Znaleziono obiekt minimapy, ale brak komponentu RawImage!");
            yield break;
        }

        // ZnajdŸ now¹ kamerê minimapy w bie¿¹cej scenie
        GameObject cameraObject = GameObject.Find("MinimapCamera");
        if (cameraObject == null)
        {
            Debug.LogWarning("Nie znaleziono kamery minimapy w scenie!");
            yield break;
        }

        Camera minimapCamera = cameraObject.GetComponent<Camera>();
        if (minimapCamera == null || minimapCamera.targetTexture == null)
        {
            Debug.LogWarning("Kamera minimapy nie ma przypisanej RenderTexture!");
            yield break;
        }

        minimapImage.texture = minimapCamera.targetTexture;
        Debug.Log($"Ustawiono teksturê minimapy dla sceny: {sceneName}");
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
