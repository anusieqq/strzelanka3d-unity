using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioManager : MonoBehaviour
{
    public AudioClip menuSound;
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuSound;
        audioSource.loop = true;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu") 
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (audioSource != null && menuSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMenuMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}