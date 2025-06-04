using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingAudio : MonoBehaviour
{
    public AudioClip buildingSound; // Publiczne pole do wyboru klipu d�wi�kowego
    private AudioSource audioSource;

    void Awake()
    {
        // Zachowaj obiekt mi�dzy scenami
        DontDestroyOnLoad(gameObject);

        // Pobierz komponent AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Dodano AudioSource do: " + gameObject.name);
        }

        // Sprawdzenie, czy AudioClip jest przypisany
        if (buildingSound == null)
        {
            Debug.LogError("Brak przypisanego AudioClip w polu buildingSound!");
            return;
        }

        // Przypisz klip d�wi�kowy do AudioSource
        audioSource.clip = buildingSound;
        audioSource.loop = true;
        Debug.Log("Przypisano AudioClip: " + buildingSound.name);
    }

    void Start()
    {
        // Odtwarzaj muzyk� tylko na scenie BUILDING
        if (SceneManager.GetActiveScene().name == "BUILDING")
        {
            Debug.Log("Scena BUILDING za�adowana, odtwarzam d�wi�k: " + buildingSound.name);
            PlayGameMusic();
        }
        else
        {
            Debug.LogWarning("Aktualna scena to: " + SceneManager.GetActiveScene().name + ". D�wi�k gry nie zostanie odtworzony.");
        }
    }

    void Update()
    {
        // Zatrzymaj d�wi�k, je�li scena zmieni si� na inn� ni� BUILDING
        if (SceneManager.GetActiveScene().name != "BUILDING" && audioSource.isPlaying)
        {
            StopGameMusic();
            Debug.Log("Scena zmieniona, zatrzymano d�wi�k gry.");
        }
    }

    public void PlayGameMusic()
    {
        if (audioSource != null && buildingSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("Odtwarzanie muzyki gry: " + buildingSound.name);
        }
    }

    public void StopGameMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Zatrzymano muzyk� gry.");
        }
    }

    void OnDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Obiekt BuildingAudio zniszczony, zatrzymano d�wi�k.");
        }
    }
}