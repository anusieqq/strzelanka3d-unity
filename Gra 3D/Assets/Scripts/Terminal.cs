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

    private string riddle = "Plik serwera zosta� przeniesiony. Oto dost�pne lokalizacje:\n\n" +
                            "Wskaz�wka:\n" +
                            "Nie ufaj nieznanym go�ciom, systemowe pliki bywaj� z�udne, a kosmici s� podejrzani.";

    public Gun gunScript;
    public Animator boxAnimator;

    private int liczba_pr�b = 0;
    private int max_pr�b = 2;
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
                liczba_pr�b = 0;
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

        Debug.Log("Klikni�to odpowied�: " + selectedAnswer);

        // Je�li odpowied� poprawna:
        if (selectedAnswer == correctAnswer)
        {
            feedbackText.text = "Brawo! Poprawna odpowied�!";
            feedbackText.color = Color.green;

            if (boxAnimator != null)
            {
                boxAnimator.SetTrigger("Open Box");
                czyskrzyniaotwarta = true;
            }

            ZakonczenieZagadki();
            return; // Zatrzymaj dalsze sprawdzanie
        }

        

        // Sprawdzamy, czy liczba pr�b nie przekroczy�a limitu
        if (liczba_pr�b >= max_pr�b)
        {
            feedbackText.text = "Przekroczono limit pr�b!";
            feedbackText.color = Color.red;
            ZakonczenieZagadki();
        }

        else
        {
            liczba_pr�b++;
            feedbackText.text = "Niestety b��dna odpowied�. Spr�buj jeszcze raz \n Pozosta�a jedna pr�ba";
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