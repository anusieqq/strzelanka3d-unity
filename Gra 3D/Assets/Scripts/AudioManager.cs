using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip BuildingMusic;
    public AudioClip ForestMusic;
    public AudioClip UfoMusic;

    [Header("UI")]
    public GameObject optionsPanel;

    private AudioSource musicSource;
    private const string MIXER_Shoot = "ShootVolume";
    private string lastSceneMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // <- przeniesiono tutaj
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            InitializeVolumeSettings();
            HandleOptionsPanel();

            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("AudioManager utworzony jako singleton.");
        }
        else
        {
            Debug.LogWarning("Próba utworzenia kolejnej instancji AudioManager. Niszczenie duplikatu.");
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

        SetShootVolume(PlayerPrefs.GetFloat(MIXER_Shoot, 1f));
    }

    private void HandleOptionsPanel()
    {
        if (optionsPanel != null)
        {
            DontDestroyOnLoad(optionsPanel);
            optionsPanel.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopMusic();

        switch (scene.name)
        {
            case "Menu":
                PlayMusic(menuMusic);
                lastSceneMusic = "Menu";
                Debug.Log("Odtwarzanie muzyki menu w scenie Menu.");
                break;
            case "BUILDING":
                PlayMusic(BuildingMusic);
                lastSceneMusic = "BUILDING";
                Debug.Log("Odtwarzanie muzyki gry w scenie BUILDING.");
                break;
            case "Forest":
                PlayMusic(ForestMusic);
                lastSceneMusic = "Forest";
                Debug.Log("Odtwarzanie muzyki gry w scenie FOREST.");
                break;
            case "scenaufo":
                PlayMusic(UfoMusic);
                lastSceneMusic = "BUILDING";
                Debug.Log("Odtwarzanie muzyki gry w scenie UFO.");
                break;
            default:
                lastSceneMusic = null;
                Debug.Log("Brak przypisanej muzyki dla sceny: " + scene.name);
                break;
        }

        // Obs³uga panelu opcji
        if (optionsPanel == null)
        {
            optionsPanel = GameObject.Find("Opcje");
            if (optionsPanel != null)
            {
                optionsPanel.transform.SetParent(null);
                DontDestroyOnLoad(optionsPanel);
                optionsPanel.SetActive(false);
                Debug.Log("Znaleziono panel opcji i ustawiono na DontDestroyOnLoad.");
            }
            else
            {
                Debug.LogWarning("Nie znaleziono panelu opcji w scenie: " + scene.name);
            }
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
        {
            Debug.LogWarning("PlayMusic: musicSource lub clip jest null!");
            return;
        }

        if (musicSource.clip != clip || !musicSource.isPlaying)
        {
            musicSource.clip = clip;
            musicSource.Play();
            Debug.Log("Odtwarzanie muzyki: " + clip.name);
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.clip = null;
            Debug.Log("Muzyka zatrzymana.");
        }
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
        Debug.Log("Odtwarzanie muzyki menu.");
    }

    public void StopMenuMusic()
    {
        if (musicSource != null && musicSource.clip == menuMusic && musicSource.isPlaying)
        {
            StopMusic();
            Debug.Log("Zatrzymano muzykê menu.");
        }
    }

    public void PlayGameMusic()
    {
        // Odtwarzaj muzykê zale¿n¹ od aktualnej sceny
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "BUILDING":
                PlayMusic(BuildingMusic);
                Debug.Log("Odtwarzanie muzyki gry BUILDING.");
                break;
            case "Forest":
                PlayMusic(ForestMusic);
                Debug.Log("Odtwarzanie muzyki gry FOREST.");
                break;
            case "scenaufo":
                PlayMusic(UfoMusic);
                Debug.Log("Odtwarzanie muzyki gry UFO.");
                break;
            default:
                Debug.LogWarning("Brak muzyki gry dla sceny: " + currentScene);
                break;
        }
    }

    public void RestoreSceneMusic()
    {
        // Przywraca muzykê odpowiedni¹ dla aktualnej sceny
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Menu":
                PlayMenuMusic();
                break;
            case "BUILDING":
                PlayMusic(BuildingMusic);
                Debug.Log("Przywrócono muzykê BUILDING.");
                break;
            case "Forest":
                PlayMusic(ForestMusic);
                Debug.Log("Przywrócono muzykê FOREST.");
                break;
            case "scenaufo":
                PlayMusic(UfoMusic);
                Debug.Log("Przywrócono muzykê UFO.");
                break;
            default:
                StopMusic();
                Debug.Log("Brak muzyki do przywrócenia dla sceny: " + currentScene);
                break;
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

    public void ToggleOptionsMenu()
    {
        if (optionsPanel != null)
        {
            bool wasActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(!wasActive);

            if (optionsPanel.activeSelf)
            {
                PlayMenuMusic(); // Odtwarzaj muzykê menu w panelu opcji
            }
            else
            {
                RestoreSceneMusic(); // Przywróæ muzykê sceny po zamkniêciu opcji
            }

            // Pauzuj grê, gdy panel opcji jest aktywny (tylko w scenach gry)
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                Time.timeScale = optionsPanel.activeSelf ? 0f : 1f;
                Cursor.visible = optionsPanel.activeSelf;
                Cursor.lockState = optionsPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            }
            Debug.Log("Prze³¹czono panel opcji: " + (optionsPanel.activeSelf ? "Aktywny" : "Nieaktywny"));
        }
        else
        {
            Debug.LogWarning("optionsPanel jest null w ToggleOptionsMenu!");
        }
    }

    private void OnDestroy()
    {
        if (optionsPanel != null)
        {
            Destroy(optionsPanel);
            Debug.Log("Zniszczono panel opcji.");
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}