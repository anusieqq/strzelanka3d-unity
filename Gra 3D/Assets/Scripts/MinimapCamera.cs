using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player; // Referencja do transformacji gracza
    public float height = 10f; // Wysokoœæ kamery nad graczem

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + height, player.position.z);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("Player reference not set in MinimapCamera script!");
        }
    }
}