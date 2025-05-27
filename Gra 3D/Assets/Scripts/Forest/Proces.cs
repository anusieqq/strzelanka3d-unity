using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Proces : MonoBehaviour
{
    public Canvas canvas;
    public GameObject panelstart;
    public Button buttonstart; 

    void Start()
    {
        canvas.gameObject.SetActive(false);
        panelstart.SetActive(false);
        buttonstart.gameObject.SetActive(false);

        buttonstart.onClick.AddListener(OnStartClicked);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("player")) return;

        canvas.gameObject.SetActive(true);
        panelstart.SetActive(true);
        buttonstart.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("player")) return;

        canvas.gameObject.SetActive(false);
        buttonstart.gameObject.SetActive(false);
        panelstart.SetActive(false);
     

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnStartClicked()
    {
        panelstart.SetActive(false);
    }
}
