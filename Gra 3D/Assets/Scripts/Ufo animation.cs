using UnityEngine;

public class UFOAnimation : MonoBehaviour
{
    private float floatSpeed = 1.5f; // Prêdkoœæ oscylacji
    private float floatHeight = 1f; // Wysokoœæ oscylacji
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Zapamiêtaj pocz¹tkow¹ pozycjê UFO
    }

    void Update()
    {
        // Oscylacja góra-dó³ (efekt lewitowania)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
