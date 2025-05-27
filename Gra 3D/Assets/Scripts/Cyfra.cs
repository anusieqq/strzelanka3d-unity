using UnityEngine;

public class ZbieranieCyfry : MonoBehaviour
{
    public PanelCollision panelCollision;
    public Serwer serwer;
    public Terminal terminal;
    public ZagadkowyPrzedmiot zagadkowyPrzedmiot;
    public NPCDialog npcDialog;

    void Start()
    {
        if (panelCollision == null)
            panelCollision = FindObjectOfType<PanelCollision>();

        if (serwer == null)
            serwer = FindObjectOfType<Serwer>();

        if (terminal == null)
            terminal = FindObjectOfType<Terminal>();

        if (zagadkowyPrzedmiot == null)
            zagadkowyPrzedmiot = FindObjectOfType<ZagadkowyPrzedmiot>();

        if (npcDialog == null)
            npcDialog = FindObjectOfType<NPCDialog>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.CompareTag("Box"))
        {
            bool skrzyniaOtwarta =
                (serwer != null && serwer.czyskrzyniaotwarta) ||
                (terminal != null && terminal.czyskrzyniaotwarta) ||
                (zagadkowyPrzedmiot != null && zagadkowyPrzedmiot.czyskrzyniaotwarta) ||
                (npcDialog != null && npcDialog.czyskrzyniaotwarta);

            Debug.Log($"Serwer otwarty: {serwer?.czyskrzyniaotwarta} | Terminal: {terminal?.czyskrzyniaotwarta}");

            if (!skrzyniaOtwarta)
            {
                Debug.Log("Skrzynia jeszcze zamkniêta — nic nie zbieram");
                return;
            }

            foreach (GameObject number in GameObject.FindGameObjectsWithTag("Numbers"))
            {
                if (Vector3.Distance(transform.position, number.transform.position) < 2f)
                {
                    if (number.TryGetComponent(out Cyfra cyfra))
                    {
                        panelCollision?.AddZebranaCyfre(cyfra.cyfraID[0]);
                    }
                    Destroy(number);
                }
            }
        }
    }

}
