using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public GameObject uiOpcje;

    private const string MIXER_Shoot = "ShootVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVolumeSettings();

            if (uiOpcje != null)
            {
                DontDestroyOnLoad(uiOpcje);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeVolumeSettings()
    {
        if (!PlayerPrefs.HasKey(MIXER_Shoot))
        {
            PlayerPrefs.SetFloat(MIXER_Shoot, 1f);
            PlayerPrefs.Save();
        }
    }

    public void SetShootVolume(float value)
    {
        if (audioMixer != null)
        {
            float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat(MIXER_Shoot, volume);
            PlayerPrefs.SetFloat(MIXER_Shoot, value);
            PlayerPrefs.Save();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (uiOpcje == null)
        {
            uiOpcje = GameObject.Find("Opcje"); 
            if (uiOpcje != null)
            {
                uiOpcje.transform.SetParent(null);
                DontDestroyOnLoad(uiOpcje);
                uiOpcje.SetActive(false); 
            }
        }
    }
}
