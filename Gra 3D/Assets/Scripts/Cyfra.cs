using System.Collections.Generic;
using UnityEngine;

public class ZbieranieCyfry : MonoBehaviour
{
    public PanelCollision panelCollision; // Referencja do PanelCollision

    void Start()
    {
        if (panelCollision == null)
        {
            panelCollision = FindObjectOfType<PanelCollision>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && this.CompareTag("Box"))
        {
            foreach (GameObject number in GameObject.FindGameObjectsWithTag("Numbers"))
            {
                if (Vector3.Distance(transform.position, number.transform.position) < 2f)
                {
                    if (number.TryGetComponent(out Cyfra cyfra))
                    {
                        if (panelCollision != null)
                        {
                            panelCollision.AddZebranaCyfre(cyfra.cyfraID[0]); // Dodajemy pierwsz¹ literê (zak³adaj¹c, ¿e cyfraID to string np. "1")
                        }
                    }
                    Destroy(number);
                }
            }
        }
    }
}