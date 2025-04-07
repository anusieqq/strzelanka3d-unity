using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public Canvas canvas; // Przypisz obiekt Canvas w Inspectorze
    public TextMeshProUGUI terminalText; // Przypisz obiekt TextMeshProUGUI w Inspectorze
    public TextMeshProUGUI feedbackText; // Komunikat zwrotny dla gracza
    public Button[] answerButtons; // Przypisz przyciski odpowiedzi w Inspectorze
    public TextMeshProUGUI[] buttonTexts; // Przypisz teksty przyciskÛw w Inspectorze

    private string correctAnswer = "/home/student/projekt/server.exe"; // Poprawna odpowiedü
    private string[] answers =
    {
        "C:\\Users\\Alien\\Documents\\Server.exe",
        "C:\\Users\\Guest\\Desktop\\Server.exe",
        "C:\\System32\\Server.exe",
        "/home/student/projekt/server.exe"
    };

    private string riddle = "Plik serwera zosta≥ przeniesiony. Oto dostÍpne lokalizacje:\n\n" +
                            "WskazÛwka:\n" +
                            "Nie ufaj nieznanym goúciom, systemowe pliki bywajπ z≥udne, a kosmici sπ podejrzani.";

    public Gun gunScript; // Referencja do skryptu Gun
    public Animator boxAnimator; // Referencja do Animatora skrzynki

    void Start()
    {
        // Jeúli canvas jest przypisany w inspectorze, ustaw go jako niewidoczny na poczπtku gry
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        // Sprawdzamy, czy wszystkie odpowiednie elementy sπ przypisane w inspectorze
        if (answerButtons != null && answerButtons.Length >= 4 && buttonTexts.Length >= 4)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                buttonTexts[i].text = answers[i]; // Ustawienie tekstu na przyciskach
                int index = i; // Lokalna kopia zmiennej dla delegata
                Debug.Log("Przypisano listener do przycisku: " + answers[i]); // Debugowanie

                answerButtons[i].onClick.AddListener(() => CheckAnswer(answers[index])); // Dodanie listenera
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player")) // Sprawdzenie, czy obiekt ma tag "Player"
        {
            if (canvas != null && terminalText != null)
            {
                terminalText.text = riddle;
                feedbackText.text = ""; // WyczyúÊ poprzedni komunikat
                canvas.gameObject.SetActive(true); // Aktywacja canvasa po kolizji

                // Wy≥πczenie skryptu Gun
                if (gunScript != null)
                {
                    gunScript.enabled = false; // Wy≥πczenie skryptu broni
                    Debug.Log("Skrypt Gun zosta≥ wy≥πczony.");
                }

                // W≥πcz kursor myszy
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Kursor zosta≥ w≥πczony.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false); // Dezaktywacja canvasa po opuszczeniu strefy

                // W≥πczenie skryptu Gun
                if (gunScript != null)
                {
                    gunScript.enabled = true; // W≥πczenie skryptu broni
                    Debug.Log("Skrypt Gun zosta≥ w≥πczony.");
                }

                // Ukryj kursor myszy i zablokuj go ponownie
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Kursor zosta≥ ukryty.");
            }
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        Debug.Log("KlikniÍto odpowiedü: " + selectedAnswer); // Debugowanie, ktÛra odpowiedü zosta≥a klikniÍta

        if (feedbackText != null)
        {
            if (selectedAnswer == correctAnswer)
            {
                feedbackText.text = "Brawo! Poprawna odpowiedü!";
                feedbackText.color = Color.green;
                Debug.Log("Poprawna odpowiedü!"); // Debugowanie poprawnej odpowiedzi

                // Uruchomienie animacji otwierania skrzynki
                if (boxAnimator != null)
                {
                    boxAnimator.SetTrigger("Open Box");
                    Debug.Log("Animacja otwierania skrzynki zosta≥a uruchomiona.");
                }
            }
            else
            {
                feedbackText.text = "Niestety, b≥Ídna odpowiedü. SprÛbuj jeszcze raz!";
                feedbackText.color = Color.red;
                Debug.Log("B≥Ídna odpowiedü!"); // Debugowanie b≥Ídnej odpowiedzi
            }
        }
    }
}
