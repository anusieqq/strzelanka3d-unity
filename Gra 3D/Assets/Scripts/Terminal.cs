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
    public TextMeshProUGUI[] buttonTexts; // Przypisz teksty przycisk�w w Inspectorze

    private string correctAnswer = "/home/student/projekt/server.exe"; // Poprawna odpowied�
    private string[] answers =
    {
        "C:\\Users\\Alien\\Documents\\Server.exe",
        "C:\\Users\\Guest\\Desktop\\Server.exe",
        "C:\\System32\\Server.exe",
        "/home/student/projekt/server.exe"
    };

    private string riddle = "Plik serwera zosta� przeniesiony. Oto dost�pne lokalizacje:\n\n" +
                            "Wskaz�wka:\n" +
                            "Nie ufaj nieznanym go�ciom, systemowe pliki bywaj� z�udne, a kosmici s� podejrzani.";

    public Gun gunScript; // Referencja do skryptu Gun
    public Animator boxAnimator; // Referencja do Animatora skrzynki

    void Start()
    {
        // Je�li canvas jest przypisany w inspectorze, ustaw go jako niewidoczny na pocz�tku gry
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        // Sprawdzamy, czy wszystkie odpowiednie elementy s� przypisane w inspectorze
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
                feedbackText.text = ""; // Wyczy�� poprzedni komunikat
                canvas.gameObject.SetActive(true); // Aktywacja canvasa po kolizji

                // Wy��czenie skryptu Gun
                if (gunScript != null)
                {
                    gunScript.enabled = false; // Wy��czenie skryptu broni
                    Debug.Log("Skrypt Gun zosta� wy��czony.");
                }

                // W��cz kursor myszy
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Kursor zosta� w��czony.");
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

                // W��czenie skryptu Gun
                if (gunScript != null)
                {
                    gunScript.enabled = true; // W��czenie skryptu broni
                    Debug.Log("Skrypt Gun zosta� w��czony.");
                }

                // Ukryj kursor myszy i zablokuj go ponownie
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Kursor zosta� ukryty.");
            }
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        Debug.Log("Klikni�to odpowied�: " + selectedAnswer); // Debugowanie, kt�ra odpowied� zosta�a klikni�ta

        if (feedbackText != null)
        {
            if (selectedAnswer == correctAnswer)
            {
                feedbackText.text = "Brawo! Poprawna odpowied�!";
                feedbackText.color = Color.green;
                Debug.Log("Poprawna odpowied�!"); // Debugowanie poprawnej odpowiedzi

                // Uruchomienie animacji otwierania skrzynki
                if (boxAnimator != null)
                {
                    boxAnimator.SetTrigger("Open Box");
                    Debug.Log("Animacja otwierania skrzynki zosta�a uruchomiona.");
                }
            }
            else
            {
                feedbackText.text = "Niestety, b��dna odpowied�. Spr�buj jeszcze raz!";
                feedbackText.color = Color.red;
                Debug.Log("B��dna odpowied�!"); // Debugowanie b��dnej odpowiedzi
            }
        }
    }
}
