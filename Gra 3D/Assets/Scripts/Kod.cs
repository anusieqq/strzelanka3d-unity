using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class PanelCollision : MonoBehaviour
{
    [Header("Referencje UI")]
    public GameObject inputFieldPanel;
    public TMP_InputField inputField;
    public TMP_Text displayText;

    [Header("Ustawienia Kodu")]
    public string correctCode = "2137";
    public bool sprawdzajZagadki = true;

    private string rawText = "";
    private string zebraneCyfry = "";
    public Animator dooranimator;
    public LoadForest loadForestScript;



    // Kolory dla kolejnych pozycji kursora
    private readonly Color[] koloryKursora = new Color[]
    {
        Color.blue,      
        Color.green,     
        Color.red,       
        new Color(0.5f, 0f, 0.5f) 
    };

    private readonly Dictionary<char, Color> koloryCyfr = new Dictionary<char, Color>()
    {
        {'2', Color.blue},
        {'1', Color.green},
        {'3', Color.red},
        {'7', new Color(0.5f, 0f, 0.5f)}
    };

    private void Start()
    {
        if (inputFieldPanel != null)
            inputFieldPanel.SetActive(false);
            displayText.gameObject.SetActive(false);

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputChanged);
            inputField.customCaretColor = true; 
        }

        if (loadForestScript != null)
            loadForestScript.enabled = false;

    }

    private void UpdateCaretColor()
    {
        if (inputField != null)
        {
            int pozycja = rawText.Length;
            if (pozycja < koloryKursora.Length)
            {
                inputField.caretColor = koloryKursora[pozycja];
            }
            else
            {
                inputField.caretColor = Color.white; // Domyœlny kolor poza zakresem
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Kolizja z: {other.name} (Tag: {other.tag}, Layer: {LayerMask.LayerToName(other.gameObject.layer)})");

        if (other.CompareTag("player"))
        {
            Debug.Log("Wykryto gracza w strefie triggera");

            if (sprawdzajZagadki)
            {
                GameObject[] rozwiazaneZagadki = GameObject.FindGameObjectsWithTag("Finish");
                Debug.Log($"Znaleziono {rozwiazaneZagadki.Length}/4 rozwi¹zanych zagadek");

                if (rozwiazaneZagadki.Length < 4)
                {
                    if (displayText != null)
                    {
                        displayText.gameObject.SetActive(true);
                        displayText.text = "<b><color=red>Rozwi¹¿ wszystkie zagadki, aby kontynuowaæ!</color></b>";
                    }

                    if (inputFieldPanel != null)
                    {
                        inputFieldPanel.SetActive(false);
                    }

                    return;
                }
            }

            // Sprawdzenie przeciwników
            GameObject[] przeciwnicy = GameObject.FindGameObjectsWithTag("Enemy");
            if (przeciwnicy.Length > 0)
            {
                if (displayText != null)
                {
                    displayText.gameObject.SetActive(true);
                    displayText.text = "<b><color=red>Zabij wszystkich przeciwników, aby kontynuowaæ!</color></b>";
                }

                if (inputFieldPanel != null)
                {
                    inputFieldPanel.SetActive(false);
                }

                return;
            }

            // Wszystkie warunki spe³nione
            if (inputFieldPanel != null)
            {
                inputFieldPanel.SetActive(true);
            }

            if (inputField != null)
            {
                rawText = "";
                inputField.text = "";
                inputField.ActivateInputField();
                UpdateCaretColor();
            }

            if (displayText != null)
            {
                displayText.gameObject.SetActive(true);
            }

            PokazZebraneCyfry();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Gracz opuœci³ strefê triggera");

            if (inputFieldPanel != null)
            {
                inputFieldPanel.SetActive(false);
            }

            if (inputField != null)
            {
                rawText = "";
                inputField.text = "";
            }

            if (displayText != null)
            {
                displayText.gameObject.SetActive(false); 
            }
        }
    }
    private void OnInputChanged(string displayedText)
    {
        string newRawText = "";
        bool zawieraNieprawidloweZnaki = false;

        foreach (char c in displayedText)
        {
            if (char.IsDigit(c) && newRawText.Length < 4)
            {
                newRawText += c;
            }
            else if (!char.IsDigit(c))
            {
                zawieraNieprawidloweZnaki = true;
            }
        }

        if (zawieraNieprawidloweZnaki)
        {
            if (displayText != null)
            {
                displayText.text = "<b><color=red>Wpisuj tylko cyfry!</color></b>";
            }

            // Wyczyœæ pole i pozwól u¿ytkownikowi wpisaæ ponownie
            inputField.text = rawText; // Przywróæ poprzedni poprawny stan
            inputField.ActivateInputField();
            UpdateCaretColor();
            return;
        }

        rawText = newRawText;
        UpdateCaretColor();

        if (rawText.Length == correctCode.Length)
        {
            if (rawText == correctCode)
            {
                Debug.Log("Poprawny kod!");
                OnCorrectCode();
            }
            else
            {
                Debug.Log("Niepoprawny kod!");
                if (displayText != null)
                {
                    displayText.text = "<b><color=red>Niepoprawny kod, spróbuj ponownie.</color></b>";
                }

                // Wyczyœæ pole i pozwól u¿ytkownikowi wpisaæ ponownie
                inputField.text = "";
                rawText = "";
                inputField.ActivateInputField();
                UpdateCaretColor();
            }
        }
    }


    private string GetColoredText(string text)
    {
        string result = "";
        for (int i = 0; i < text.Length; i++)
        {
            char cyfra = text[i];
            if (koloryCyfr.TryGetValue(cyfra, out Color kolor))
            {
                result += $"<color=#{ColorUtility.ToHtmlStringRGB(kolor)}>{cyfra}</color>";
            }
            else
            {
                result += cyfra;
            }
        }
        return result;
    }

    private void OnCorrectCode()
    {
        Debug.Log("Kod zaakceptowany! Otwieranie drzwi...");
        dooranimator.SetTrigger("Open Door");
        if (loadForestScript != null)
            loadForestScript.enabled = true;
        loadForestScript.canLoadScene = true;

    }


    public void AddZebranaCyfre(char cyfra)
    {
        if (zebraneCyfry.Length < 4 && !zebraneCyfry.Contains(cyfra.ToString()))
        {
            zebraneCyfry += cyfra;
            PokazZebraneCyfry();
        }
    }

    private void PokazZebraneCyfry()
    {
        if (displayText != null)
        {
            string formattedText = "<b>Zebrane cyfry:</b>\n\n";
            for (int i = 0; i < zebraneCyfry.Length; i++)
            {
                char cyfra = zebraneCyfry[i];
                if (koloryCyfr.TryGetValue(cyfra, out Color kolor))
                {
                    formattedText += $"<color=#{ColorUtility.ToHtmlStringRGB(kolor)}>{cyfra}</color> ";
                }
                else
                {
                    formattedText += $"{cyfra} ";
                }
            }
            displayText.text = formattedText.Trim();
        }
    }

    public void ResetujZebraneCyfry()
    {
        zebraneCyfry = "";
        PokazZebraneCyfry();
    }
}