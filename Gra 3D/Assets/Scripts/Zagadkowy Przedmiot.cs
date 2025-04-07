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
    public TextMeshProUGUI podpowied�Text;
    private string[] answers =
    {
            "kompas",
            "zegarek",
            "szklanka"
    };
    private string correctAnswer = "zegarek";
    public Gun gunscript;
    public Animator boxanimator;
    private string riddle = "Co�, przed czym w �wiecie nic nie uciecze,\r\nco gnie �elazo, przegryza miecze,\r\npo�era ptaki, zwierz�ta, ziele,\r\nnajtwardszy kamie� na m�k� miele,\r\nkr�l�w nie szcz�dzi, rozwala mury,\r\nponi�a nawet najwy�sze g�ry.";
    private string wskaz�wka = "Wybierz symbol, kt�ry najlepiej przypomina ci co� co przed sesj� czas staje si� najwa�niejszym zasobem.";

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
                podpowied�Text.text = wskaz�wka;
                feedbackText.text = "";
                canvas.gameObject.SetActive(true);

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
                    Debug.Log("Skrypt Gun zosta� w��czony");
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Kursor zosta� wy��czony");

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
                feedbackText.text = "Brawo! Poprawna odpowied�";
                feedbackText.color = Color.green;
                Debug.Log("Poprawna odpowied�");

                if (boxanimator != null)
                {
                    boxanimator.SetTrigger("Open Box");
                    Debug.Log("Skrzynka otwarta");
                }

            }

            else
            {
                feedbackText.text = "Niestety b��dna opdowied�. Spr�buj jeszcze raz";
                feedbackText.color = Color.red;
                Debug.Log("B��dna odpowied�");
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

                Debug.Log("Klikni�to: " + clickedObject.name + " z tagiem: " + tag);


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