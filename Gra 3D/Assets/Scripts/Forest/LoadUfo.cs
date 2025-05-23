using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUfo : MonoBehaviour
{
    public bool canLoadScene = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!canLoadScene) return;

        if (other.CompareTag("player"))
        {
            StartCoroutine(LoadSceneAndPositionPlayer());
        }
    }

    private IEnumerator LoadSceneAndPositionPlayer()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("scenaufo");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return null;

       
        GameObject player = GameObject.FindGameObjectWithTag("player");
        GameObject startPoint = GameObject.Find("StartPoint");

        if (player != null && startPoint != null)
        {
            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning("Nie znaleziono gracza lub StartPoint w scenie.");
        }
    }
}
