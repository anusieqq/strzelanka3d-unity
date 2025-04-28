using UnityEngine;

public class Cyfra : MonoBehaviour
{
    public string cyfraID = "0"; // Ustaw odpowiedni¹ cyfrê (2, 1, 3 lub 7)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            PanelCollision panel = FindObjectOfType<PanelCollision>();
            if (panel != null && !string.IsNullOrEmpty(cyfraID))
            {
                panel.AddZebranaCyfre(cyfraID[0]);
                Destroy(gameObject); // Usuñ zebran¹ cyfrê
            }
        }
    }
}