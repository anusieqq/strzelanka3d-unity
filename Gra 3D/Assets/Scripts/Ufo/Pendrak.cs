using UnityEngine;

public class Pendrak : MonoBehaviour
{
    public GameObject uploadPoint;

    void Start()
    {
        if (uploadPoint == null)
        {
            Debug.LogError($"Pendrak: UploadPoint nie jest przypisany w {gameObject.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log($"Pendrak: Kolizja z graczem w {gameObject.name}");
            if (uploadPoint != null)
            {
                Pendrive p = uploadPoint.GetComponent<Pendrive>();
                if (p != null)
                {
                    p.SetPendriveCollected();
                    Debug.Log("Pendrak: Wywo³ano SetPendriveCollected w Pendrive.");
                }
                else
                {
                    Debug.LogError("Pendrak: Brak komponentu Pendrive na UploadPoint!");
                }
            }
            else
            {
                Debug.LogError("Pendrak: UploadPoint nie jest przypisany!");
            }

            if (ServerManager.Instance != null)
            {
                ServerManager.Instance.SetPendriveCollected();
                Debug.Log("Pendrak: Wywo³ano SetPendriveCollected w ServerManager.");
            }
            else
            {
                Debug.LogError("Pendrak: ServerManager.Instance jest null!");
            }

            Destroy(gameObject);
            Debug.Log("Pendrak: Pendrive zebrany i zniszczony!");
        }
    }
}