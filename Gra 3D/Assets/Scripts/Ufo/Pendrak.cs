using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendrak : MonoBehaviour
{
    public GameObject uploadPoint;

    public void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Kolizja z graczem w Pendrak!");
            if (uploadPoint != null)
            {
                Pendrive p = uploadPoint.GetComponent<Pendrive>();
                if (p != null)
                {
                    p.SetPendriveCollected();
                    Destroy(gameObject); 
                    Debug.Log("Pendrive zebrany i zniszczony!");
                }
                else
                {
                    Debug.LogError("Brak komponentu Pendrive na UploadPoint!");
                }
            }
            else
            {
                Debug.LogError("UploadPoint nie jest przypisany!");
            }
        }
    }
}