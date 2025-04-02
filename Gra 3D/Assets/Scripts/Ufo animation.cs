using UnityEngine;

public class UFOAnimation : MonoBehaviour
{
    public float floatSpeed = 1.5f; // Prêdkoœæ oscylacji
    public float floatHeight = 2.5f; // Wysokoœæ oscylacji
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Pocz¹tkowa pozycja ufo
    }

    void Update()
    {
        // Oscylacja góra-dó³ (efekt lewitowania)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
