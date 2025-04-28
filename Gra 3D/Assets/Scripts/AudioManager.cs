using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;

    private const string MIXER_Shoot = "ShootVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVolumeSettings();
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
}
