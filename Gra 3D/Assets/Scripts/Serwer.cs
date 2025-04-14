using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Serwer : MonoBehaviour
{
    public Canvas zagadka;
    public Gun gunscript;
    public TextMeshProUGUI zagadkaText;
    public TextMeshProUGUI feedbackText;
    public Button[] butons;
    public Animator boxAnimator;

    private string correctAnswer = "ufo"; 

    private string riddle = "Serwer znikn��! Analizuj� �lad danych...Gdzie on mo�e by�?\nSerwer ostatnio by� widziany w chmurze";

    void Start()
    {
        if (zagadka != null)
        {
            zagadka.gameObject.SetActive(false);
        }

        foreach (Button btn in butons)
        {
            Button localBtn = btn; // potrzebne do zamkni�cia w lambdzie
            localBtn.onClick.AddListener(() => CheckAnswerFromImage(localBtn));
        }
    }

    private void CheckAnswerFromImage(Button btn)
    {
        Image img = btn.GetComponent<Image>();
        if (img != null && img.sprite != null)
        {
            string selectedName = img.sprite.name;
            Debug.Log("Klikni�to: " + selectedName);

            if (selectedName == correctAnswer)
            {
                feedbackText.text = "Poprawna odpowied�!";
                feedbackText.color = Color.green;
                boxAnimator.SetTrigger("Open Box");
            }
            else
            {
                feedbackText.text = "Z�a odpowied�!";
                feedbackText.color = Color.red;
            }
        }
        else
        {
            Debug.LogWarning("Brak obrazka w przycisku: " + btn.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (zagadka != null)
            {
                zagadka.gameObject.SetActive(true);
                zagadkaText.text = riddle;
                feedbackText.text = "";

                if (gunscript != null)
                {
                    gunscript.enabled = false;
                    Debug.Log("Skrypt Gun zosta� wy��czony");
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Kursor zosta� w��czony");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (zagadka != null)
            {
                zagadka.gameObject.SetActive(false);

                if (gunscript != null)
                {
                    gunscript.enabled = true;
                    Debug.Log("Skrypt Gun zosta� w��czony");
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Kursor zosta� wy��czony");
            }
        }
    }
}
