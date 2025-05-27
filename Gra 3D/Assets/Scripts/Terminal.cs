using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI terminalText;
    public TextMeshProUGUI feedbackText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] buttonTexts;
    public bool czyskrzyniaotwarta = false;

    private string correctAnswer = "/home/student/projekt/server.exe";
    private string[] answers =
    {
        "C:\\Users\\Alien\\Documents\\Server.exe",
        "C:\\Users\\Guest\\Desktop\\Server.exe",
        "C:\\System32\\Server.exe",
        "/home/student/projekt/server.exe"
    };

    private string riddle = "Plik serwera zosta³ przeniesiony. Oto dostêpne lokalizacje:\n\n" +
                            "Wskazówka:\n" +
                            "Nie ufaj nieznanym goœciom, systemowe pliki bywaj¹ z³udne, a kosmici s¹ podejrzani.";

    public Gun gunScript;
    public Animator boxAnimator;

    private int liczba_prób = 0;
    private int max_prób = 2;
    private bool zagadkaAktywna = true;

    void Start()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        if (answerButtons != null && answerButtons.Length >= 4 && buttonTexts.Length >= 4)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                buttonTexts[i].text = answers[i];
                int index = i;
                answerButtons[i].onClick.AddListener(() => CheckAnswer(answers[index]));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.tag != "Finish")
        {
            if (canvas != null && terminalText != null && zagadkaAktywna)
            {
                terminalText.text = riddle;
                feedbackText.text = "";
                canvas.gameObject.SetActive(true);
                liczba_prób = 0;
                zagadkaAktywna = true;

                if (gunScript != null)
                {
                    gunScript.enabled = false;
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);

                if (gunScript != null)
                {
                    gunScript.enabled = true;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (!zagadkaAktywna) return;

        Debug.Log("Klikniêto odpowiedŸ: " + selectedAnswer);

        // Jeœli odpowiedŸ poprawna:
        if (selectedAnswer == correctAnswer)
        {
            feedbackText.text = "Brawo! Poprawna odpowiedŸ!";
            feedbackText.color = Color.green;

            if (boxAnimator != null)
            {
                boxAnimator.SetTrigger("Open Box");
                czyskrzyniaotwarta = true;
            }

            ZakonczenieZagadki();
            return; // Zatrzymaj dalsze sprawdzanie
        }

        

        // Sprawdzamy, czy liczba prób nie przekroczy³a limitu
        if (liczba_prób >= max_prób)
        {
            feedbackText.text = "Przekroczono limit prób!";
            feedbackText.color = Color.red;
            ZakonczenieZagadki();
        }

        else
        {
            liczba_prób++;
            feedbackText.text = "Niestety b³êdna odpowiedŸ. Spróbuj jeszcze raz \n Pozosta³a jedna próba";
            feedbackText.color = Color.red;


        }
    }
    void ZakonczenieZagadki()
    {
        zagadkaAktywna = false;
        this.tag = "Finish";
        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(2f);

        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        if (gunScript != null)
        {
            gunScript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}