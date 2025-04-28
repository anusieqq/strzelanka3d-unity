using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadForest : MonoBehaviour
{
    private void Start()
    {
        
    }
    public bool canLoadScene = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (!canLoadScene) return; 

        if (other.CompareTag("player"))
        {
            Debug.Log("Gracz dotkn¹³ drzwi. £adowanie sceny...");
            SceneManager.LoadScene("Forest");
        }
    }
}
