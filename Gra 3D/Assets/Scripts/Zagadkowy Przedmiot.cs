using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZagadkowyPrzedmiot : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI zagadkaText;
    public TextMeshProUGUI feedbackText;
    public Gun gunscript;
    public Animator boxanimator;
    public bool czyskrzyniaotwarta = false;

    private string[] answers = { "kompas", "zegarek", "szklanka" };
    private string correctAnswer = "zegarek";
    private string riddle = "Coœ, przed czym w œwiecie nic nie uciecze,\r\nco gnie ¿elazo, przegryza miecze,\r\npo¿era ptaki, zwierzêta, ziele,\r\nnajtwardszy kamieñ na m¹kê miele,\r\nkrólów nie szczêdzi, rozwala mury,\r\nponi¿a nawet najwy¿sze góry.\nWybierz symbol, który najlepiej przypomina ci coœ co przed sesj¹ czas staje siê najwa¿niejszym zasobem.";

    private int liczba_prób = 0;
    private int max_prób = 2;
    private bool zagadkaAktywna = true;

    private void Start()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.gameObject.tag != "Finish")
        {
            if (canvas != null && zagadkaText != null)
            {
                zagadkaText.text = riddle;
                feedbackText.text = "";
                canvas.gameObject.SetActive(true);
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

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);

                if (gunscript != null)
                {
                    gunscript.enabled = true;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (!zagadkaAktywna) return;

        Debug.Log("Wybrano przedmiot " + selectedAnswer);

        if (feedbackText != null)
        {
            if (selectedAnswer == correctAnswer)
            {
                feedbackText.text = "Brawo! Poprawna odpowiedŸ";
                feedbackText.color = Color.green;

                if (boxanimator != null)
                {
                    boxanimator.SetTrigger("Open Box");
                    czyskrzyniaotwarta = true;
                }

                ZakonczenieZagadki();
            }

            

            else
            {
                liczba_prób++;
                feedbackText.text = "Niestety b³êdna odpowiedŸ. Spróbuj jeszcze raz \n Pozosta³a jedna próba";
                feedbackText.color = Color.red;

               
            }

            if (liczba_prób >= max_prób)
            {
                feedbackText.text = "Przekroczono limit prób.";
                ZakonczenieZagadki();
            }
        }
    }

    void ZakonczenieZagadki()
    {
        zagadkaAktywna = false;
        this.gameObject.tag = "Finish";
        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(2f);

        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        if (gunscript != null)
        {
            gunscript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canvas != null && canvas.gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                GameObject clickedObject = hit.collider.gameObject;
                string tag = clickedObject.tag;

                foreach (string answer in answers)
                {
                    if (tag == answer)
                    {
                        CheckAnswer(tag);
                        break;
                    }
                }
            }
        }
    }
}
