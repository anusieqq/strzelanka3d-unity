using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCDialog : MonoBehaviour
{
    public string[] dialogi; // Lista dialog�w NPC
    public GameObject dymek; // Panel UI dymka dialogowego
    public TextMeshProUGUI tekstDymka; // TextMeshPro dla tekstu NPC
    public TMP_InputField poleOdpowiedzi; // Pole tekstowe na odpowied�
    public string poprawnaOdpowiedz; // Poprawna odpowied�
    public Gun gunScript; // Referencja do skryptu Gun.cs
    public Animator boxAnimator;

    private int indeksDialogu = 0;
    private bool wKolizji = false;
    private int liczba_pr�b = 0;
    private int max_pr�b = 2;

    void Start()
    {
        dymek.SetActive(false);
        poleOdpowiedzi.gameObject.SetActive(false);
        UkryjKursor();

        // Debugowanie wpisywania tekstu
        poleOdpowiedzi.onValueChanged.AddListener(delegate { Debug.Log("Wpisano: " + poleOdpowiedzi.text); });
    }

    void Update()
    {
        if (wKolizji && Input.GetKeyDown(KeyCode.E))
        {
            PokazNastepnyDialog();
        }

        // Sprawdzenie odpowiedzi po naci�ni�ciu Enter
        if (poleOdpowiedzi.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            SprawdzOdpowiedz();
        }
    }

    private void PokazNastepnyDialog()
    {
        if (indeksDialogu < dialogi.Length - 1)
        {
            tekstDymka.text = dialogi[indeksDialogu];
            dymek.SetActive(true);
            poleOdpowiedzi.gameObject.SetActive(false);
            indeksDialogu++;
        }
        else if (indeksDialogu == dialogi.Length - 1)
        {
            tekstDymka.text = dialogi[indeksDialogu];
            dymek.SetActive(true);
            poleOdpowiedzi.gameObject.SetActive(true);
            poleOdpowiedzi.interactable = true;
            indeksDialogu++;

            // Ustawienie kursora w polu tekstowym i odblokowanie wpisywania
            Invoke("AktywujPoleTekstowe", 0f);
        }
    }

    private void AktywujPoleTekstowe()
    {
        poleOdpowiedzi.text = "";
        poleOdpowiedzi.ActivateInputField();
        poleOdpowiedzi.Select();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.gameObject.tag != "Finish")
        {
            wKolizji = true;
            indeksDialogu = 0;
            liczba_pr�b = 0; // Reset pr�b
            PokazNastepnyDialog();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            wKolizji = false;
            dymek.SetActive(false);
            poleOdpowiedzi.gameObject.SetActive(false);
            indeksDialogu = 0;
            UkryjKursor();
        }
    }

    private void UkryjKursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SprawdzOdpowiedz()
    {
        if (liczba_pr�b >= max_pr�b)
        {
            tekstDymka.text = "Przekroczono limit pr�b.";
            poleOdpowiedzi.interactable = false;
            poleOdpowiedzi.gameObject.SetActive(false);
            this.gameObject.tag = "Finish"; // Ustawienie tagu po niepowodzeniu
            Invoke("ZakonczenieDialogu", 2f);
            return;
        }

        Debug.Log("Sprawdzana odpowied�: " + poleOdpowiedzi.text);
        poleOdpowiedzi.interactable = false;

        if (poleOdpowiedzi.text.Trim().ToUpper() == poprawnaOdpowiedz.ToUpper())
        {
            Debug.Log("Poprawna odpowied�!");
            tekstDymka.text = "Brawo! To poprawna odpowied�!";

            if (boxAnimator != null)
            {
                boxAnimator.SetTrigger("Open Box");
            }

            this.gameObject.tag = "Finish"; // Ustawienie tagu po sukcesie
            poleOdpowiedzi.gameObject.SetActive(false);
            Invoke("ZakonczenieDialogu", 2f);
        }
        else
        {
            liczba_pr�b++;
            Debug.Log("Z�a odpowied�!");

            if (liczba_pr�b < max_pr�b)
            {
                tekstDymka.text = "Niestety, to b��dna odpowied�. Spr�buj jeszcze raz.\n Pozosta�a jeszcze jedna pr�ba";
                poleOdpowiedzi.interactable = true;
                poleOdpowiedzi.text = "";
                poleOdpowiedzi.ActivateInputField();
                poleOdpowiedzi.Select();
            }
            else
            {
                tekstDymka.text = "To by�a ostatnia pr�ba.";
                poleOdpowiedzi.gameObject.SetActive(false);
                this.gameObject.tag = "Finish"; // Ustawienie tagu po niepowodzeniu
                Invoke("ZakonczenieDialogu", 2f);
            }
        }
    }



    private void ZakonczenieDialogu()
    {
        dymek.SetActive(false);
        UkryjKursor();
    }
}