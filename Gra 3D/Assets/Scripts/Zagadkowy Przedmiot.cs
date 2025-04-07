using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class ZagadkowyPrzedmiot : MonoBehaviour


{
    public Canvas canvas;
    public TextMeshProUGUI zagadkaText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI podpowiedŸText;
    private string[] answers =
    {
            "kompas",
            "zegarek",
            "szklanka"
    };
    private string correctAnswer = "zegarek";
    public Gun gunscript;
    public Animator boxanimator;
    private string riddle = "Coœ, przed czym w œwiecie nic nie uciecze,\r\nco gnie ¿elazo, przegryza miecze,\r\npo¿era ptaki, zwierzêta, ziele,\r\nnajtwardszy kamieñ na m¹kê miele,\r\nkrólów nie szczêdzi, rozwala mury,\r\nponi¿a nawet najwy¿sze góry.";
    private string wskazówka = "Wybierz symbol, który najlepiej przypomina ci coœ co przed sesj¹ czas staje siê najwa¿niejszym zasobem.";

    private void Start()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (canvas != null && zagadkaText != null)
            {
                zagadkaText.text = riddle;
                podpowiedŸText.text = wskazówka;
                feedbackText.text = "";
                canvas.gameObject.SetActive(true);

                if (gunscript != null)
                {
                    gunscript.enabled = false;
                    Debug.Log("Skrypt Gun zosta³ wy³¹czony");
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Kursor zosta³ w³¹czony");

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
                    Debug.Log("Skrypt Gun zosta³ w³¹czony");
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Kursor zosta³ wy³¹czony");

            }
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        Debug.Log("Wybrano przedmiot " + selectedAnswer);

        if (feedbackText != null)
        {
            if (selectedAnswer == correctAnswer)
            {
                feedbackText.text = "Brawo! Poprawna odpowiedŸ";
                feedbackText.color = Color.green;
                Debug.Log("Poprawna odpowiedŸ");

                if (boxanimator != null)
                {
                    boxanimator.SetTrigger("Open Box");
                    Debug.Log("Skrzynka otwarta");
                }

            }

            else
            {
                feedbackText.text = "Niestety b³êdna opdowiedŸ. Spróbuj jeszcze raz";
                feedbackText.color = Color.red;
                Debug.Log("B³êdna odpowiedŸ");
            }
        }
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

                Debug.Log("Klikniêto: " + clickedObject.name + " z tagiem: " + tag);


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