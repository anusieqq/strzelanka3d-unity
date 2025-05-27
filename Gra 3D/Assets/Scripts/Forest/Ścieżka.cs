using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ścieżka : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        panel.SetActive(false);
    }

  
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        panel.SetActive(true);  
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player")) return;

        panel.SetActive(false);
    }
}
