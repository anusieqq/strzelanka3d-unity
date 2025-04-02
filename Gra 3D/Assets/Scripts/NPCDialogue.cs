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

    private int indeksDialogu = 0;
    private bool wKolizji = false;

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
        if (other.CompareTag("player"))
        {
            wKolizji = true;
            indeksDialogu = 0;
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
        Debug.Log("Sprawdzana odpowied�: " + poleOdpowiedzi.text);
        poleOdpowiedzi.interactable = false; // Blokuje pole po wpisaniu odpowiedzi

        if (poleOdpowiedzi.text.Trim().ToUpper() == poprawnaOdpowiedz.ToUpper())
        {
            Debug.Log("Poprawna odpowied�!");
            tekstDymka.text = "Brawo! To poprawna odpowied�!";
        }
        else
        {
            Debug.Log("Z�a odpowied�!");
            tekstDymka.text = "Niestety, to b��dna odpowied�.";
        }

        poleOdpowiedzi.gameObject.SetActive(false);
        Invoke("ZakonczenieDialogu", 2f);
    }

    private void ZakonczenieDialogu()
    {
        dymek.SetActive(false);
        UkryjKursor();
    }
}