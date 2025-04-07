using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Transform up, down, left, right;
    public float spreadAmount = 0.02f;   // Iloœæ rozproszenia przy strzale
    public float returnSpeed = 5f;       // Prêdkoœæ powrotu do normalnej pozycji
    public float zoomAmount = 0.5f;      // Wartoœæ przybli¿enia przy klikniêciu PPM

    private Vector3 upStart, downStart, leftStart, rightStart;
    private bool isZoomedIn = false;     // Flaga sprawdzaj¹ca, czy PPM zosta³o wciœniête

    void Start()
    {
        // Pocz¹tkowa pozycja
        upStart = up.localPosition;
        downStart = down.localPosition;
        leftStart = left.localPosition;
        rightStart = right.localPosition;
    }



    void Update()
    {
        // Sprawdzamy, czy klikniêto prawy przycisk myszy 
        if (Input.GetMouseButton(1)) 
        {
            isZoomedIn = true;
            ApplyZoom();  
        }
        else
        {
            isZoomedIn = false;
            ReturnToNormal();  
        }
    }

 
    void ApplyZoom()
    {
        // Zmiana pozycji celownika na przybli¿on¹
        up.localPosition = upStart - new Vector3(0, zoomAmount, 0);
        down.localPosition = downStart + new Vector3(0, zoomAmount, 0);
        left.localPosition = leftStart - new Vector3(zoomAmount, 0, 0);
        right.localPosition = rightStart + new Vector3(zoomAmount, 0, 0);
    }

    void ReturnToNormal()
    {
        // Powrót do pocz¹tkowej pozycji 
        up.localPosition = Vector3.Lerp(up.localPosition, upStart, Time.deltaTime * returnSpeed);
        down.localPosition = Vector3.Lerp(down.localPosition, downStart, Time.deltaTime * returnSpeed);
        left.localPosition = Vector3.Lerp(left.localPosition, leftStart, Time.deltaTime * returnSpeed);
        right.localPosition = Vector3.Lerp(right.localPosition, rightStart, Time.deltaTime * returnSpeed);
    }

    // Funkcja rozszerzania celownika przy strzale
    public void Expand()
    {
        up.localPosition = upStart + new Vector3(0, spreadAmount, 0);
        down.localPosition = downStart - new Vector3(0, spreadAmount, 0);
        left.localPosition = leftStart - new Vector3(spreadAmount, 0, 0);
        right.localPosition = rightStart + new Vector3(spreadAmount, 0, 0);
    }
}
