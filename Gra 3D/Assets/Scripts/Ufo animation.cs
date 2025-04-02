using UnityEngine;

public class UFOAnimation : MonoBehaviour
{
    public float floatSpeed = 1.5f; // Pr�dko�� oscylacji
    public float floatHeight = 2.5f; // Wysoko�� oscylacji
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Pocz�tkowa pozycja ufo
    }

    void Update()
    {
        // Oscylacja g�ra-d� (efekt lewitowania)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
