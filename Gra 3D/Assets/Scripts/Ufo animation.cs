using UnityEngine;

public class UFOAnimation : MonoBehaviour
{
    private float floatSpeed = 1.5f; // Pr�dko�� oscylacji
    private float floatHeight = 1f; // Wysoko�� oscylacji
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Zapami�taj pocz�tkow� pozycj� UFO
    }

    void Update()
    {
        // Oscylacja g�ra-d� (efekt lewitowania)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
