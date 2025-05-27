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
    public bool czyskrzyniaotwarta = false;

    private string correctAnswer = "ufo";
    private string riddle = "Serwer znikn¹³! Analizujê œlad danych...Gdzie on mo¿e byæ?\nSerwer ostatnio by³ widziany w chmurze";

    private int liczba_prób = 0;
    private int max_prób = 2;
    private bool zagadkaAktywna = true;

    void Start()
    {
        if (zagadka != null)
        {
            zagadka.gameObject.SetActive(false);
        }

        foreach (Button btn in butons)
        {
            Button localBtn = btn; // potrzebne do lambdy
            localBtn.onClick.AddListener(() => CheckAnswerFromImage(localBtn));
        }
    }

    private void CheckAnswerFromImage(Button btn)
    {
        if (!zagadkaAktywna) return;

        Image img = btn.GetComponent<Image>();
        if (img != null && img.sprite != null)
        {
            string selectedName = img.sprite.name;
            Debug.Log("Klikniêto: " + selectedName);

            if (selectedName == correctAnswer)
            {
                feedbackText.text = "Poprawna odpowiedŸ!";
                feedbackText.color = Color.green;
                czyskrzyniaotwarta = true;
                if (boxAnimator != null)
                {
                    boxAnimator.SetTrigger("Open Box");

                }
                zakonczenieZagadki();
            }
            else
            {
                liczba_prób++;
                feedbackText.text = "Z³a odpowiedŸ! \n Pozosta³a jedna próba";
                feedbackText.color = Color.red;

                if (liczba_prób >= max_prób)
                {
                    feedbackText.text = "Przekroczono limit prób.";
                    zakonczenieZagadki();
                }
            }
        }
        else
        {
            Debug.LogWarning("Brak obrazka w przycisku: " + btn.name);
        }
    }

    private void zakonczenieZagadki()
    {
        zagadkaAktywna = false;
        this.gameObject.tag = "Finish";
        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(2f);

        if (zagadka != null)
        {
            zagadka.gameObject.SetActive(false);
        }

        if (gunscript != null)
        {
            gunscript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.gameObject.tag != "Finish")
        {
            if (zagadka != null)
            {
                zagadka.gameObject.SetActive(true);
                zagadkaText.text = riddle;
                feedbackText.text = "";
                liczba_prób = 0;
                zagadkaAktywna = true;

                if (gunscript != null)
                {
                    gunscript.enabled = false;
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}