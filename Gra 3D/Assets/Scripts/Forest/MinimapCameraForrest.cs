using UnityEngine;

public class MinimapCameraForrest : MonoBehaviour
{
    public Transform player; // Referencja do transformacji gracza
    public float height = 10f; // Wysoko�� kamery nad graczem

    void LateUpdate()
    {
        // Je�li gracz nie jest przypisany, spr�buj go znale��
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Je�eli gracz zosta� znaleziony, ustaw pozycj� i rotacj� kamery
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + height, player.position.z);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("Player not found for MinimapCamera!");
        }
    }
}
