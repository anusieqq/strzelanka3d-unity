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
    private string riddle = "Co�, przed czym w �wiecie nic nie uciecze,\r\nco gnie �elazo, przegryza miecze,\r\npo�era ptaki, zwierz�ta, ziele,\r\nnajtwardszy kamie� na m�k� miele,\r\nkr�l�w nie szcz�dzi, rozwala mury,\r\nponi�a nawet najwy�sze g�ry.\nWybierz symbol, kt�ry najlepiej przypomina ci co� co przed sesj� czas staje si� najwa�niejszym zasobem.";

    private int liczba_pr�b = 0;
    private int max_pr�b = 2;
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
                liczba_pr�b = 0;
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
                feedbackText.text = "Brawo! Poprawna odpowied�";
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
                liczba_pr�b++;
                feedbackText.text = "Niestety b��dna odpowied�. Spr�buj jeszcze raz \n Pozosta�a jedna pr�ba";
                feedbackText.color = Color.red;

               
            }

            if (liczba_pr�b >= max_pr�b)
            {
                feedbackText.text = "Przekroczono limit pr�b.";
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
