using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinimapTextureSwitcher : MonoBehaviour
{
    public RenderTexture MinimapCamerabuilding;
    public RenderTexture MinimapCameraForrest;

    private RawImage minimapImage;
    private static MinimapTextureSwitcher instance;

    void Awake()
    {
        // Zapobiegaj duplikacji obiektu przy ³adowaniu nowej sceny
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedTextureSwitch(scene.name));
    }

    IEnumerator DelayedTextureSwitch(string sceneName)
    {
        yield return null; // Czekaj na koniec klatki

        // Szukaj w³aœciwego obiektu minimapy
        GameObject minimapObject = GameObject.Find("Minimap"); // Zmieñ na w³aœciw¹ nazwê
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

        // Prze³¹cz teksturê
        SwitchTextureByScene(sceneName);
    }

    void SwitchTextureByScene(string sceneName)
    {
        if (minimapImage == null) return;

        Debug.Log($"Prze³¹czanie minimapy dla sceny: {sceneName}");

        switch (sceneName)
        {
            case "BUILDING":
                minimapImage.texture = MinimapCamerabuilding;
                Debug.Log("Ustawiono minimapê na BUILDING.");
                break;

            case "FORREST":
                minimapImage.texture = MinimapCameraForrest;
                Debug.Log("Ustawiono minimapê na FORREST.");
                break;

            default:
                Debug.LogWarning($"Brak przypisanej minimapy dla sceny: {sceneName}");
                break;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}