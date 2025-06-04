using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestAudio : MonoBehaviour
{
    public AudioClip forestSound; // Poprawiona nazwa zmiennej dla sceny Forest
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Pobierz komponent AudioSource
        audioSource = GetComponent<AudioSource>();

        // Sprawdzenie, czy AudioSource istnieje
        if (audioSource == null)
        {
            Debug.LogError("Brak komponentu AudioSource na GameObject: " + gameObject.name);
            return;
        }

        // Sprawdzenie, czy AudioClip jest przypisany
        if (forestSound == null)
        {
            Debug.LogError("Brak przypisanego AudioClip w polu forestSound!");
            return;
        }

        // Sprawdzenie ustawie� AudioSource
        Debug.Log($"AudioSource - Volume: {audioSource.volume}, Mute: {audioSource.mute}, Spatial Blend: {audioSource.spatialBlend}");

        // Przypisz klip d�wi�kowy do AudioSource
        audioSource.clip = forestSound;
        Debug.Log("Przypisano AudioClip: " + forestSound.name);

        // Sprawdzenie nazwy sceny
        Debug.Log("Aktualna nazwa sceny: " + SceneManager.GetActiveScene().name);

        // Sprawdzenie, czy scena to "Forest"
        if (SceneManager.GetActiveScene().name == "Forest")
        {
            Debug.Log("Scena Forest za�adowana, odtwarzam d�wi�k: " + forestSound.name);
            audioSource.Play();
            if (audioSource.isPlaying)
            {
                Debug.Log("D�wi�k jest odtwarzany!");
            }
            else
            {
                Debug.LogWarning("D�wi�k nie jest odtwarzany!");
            }
        }
        else
        {
            Debug.LogWarning("Aktualna scena to: " + SceneManager.GetActiveScene().name + ". D�wi�k nie zostanie odtworzony.");
        }

        // Sprawdzenie AudioListener
        if (FindObjectOfType<AudioListener>() == null)
        {
            Debug.LogError("Brak AudioListener w scenie!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Zatrzymaj d�wi�k, je�li scena si� zmieni
        if (SceneManager.GetActiveScene().name != "Forest" && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Scena zmieniona, zatrzymano d�wi�k.");
        }
    }

    // Zatrzymaj d�wi�k przy niszczeniu obiektu
    void OnDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Obiekt zniszczony, zatrzymano d�wi�k.");
        }
    }
}