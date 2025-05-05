using UnityEngine;

public class MinimapLayerController : MonoBehaviour
{
    public Camera minimapCamera;
    public Transform player;
    public float thresholdY = 3.0f; // Granica miêdzy parterem a piêtrem

    public LayerMask parterMask;
    public LayerMask firstFloorMask;

    void Update()
    {
        if (player.position.y < thresholdY)
        {
            minimapCamera.cullingMask = parterMask;
        }
        else
        {
            minimapCamera.cullingMask = firstFloorMask;
        }
    }
}
