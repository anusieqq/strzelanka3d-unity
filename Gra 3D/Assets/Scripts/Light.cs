using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    public Light torchLight;          
    public float minIntensity = 0.5f;  
    public float maxIntensity = 5.0f;  
    public float lightSpeed = 1.5f;   
    public Color baseColor = new Color(1.0f, 0.5f, 0.2f);


    private void Start()
    {
            torchLight.type = LightType.Point;
            torchLight.shadows = LightShadows.Soft;
    }
    void Update()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Sin(Time.time * lightSpeed) * 0.5f + 0.5f);
        torchLight.intensity = intensity;
        torchLight.color = baseColor * intensity;

     
    }


}
