using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider shootslider;

    const string MIXER_Shoot = "ShootVolume";


    private void Awake()
    {
       
        float savedVolume = PlayerPrefs.GetFloat("ShootVolume", 1f);
        shootslider.value = savedVolume;

       
        mixer.SetFloat(MIXER_Shoot, Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20);

     
        shootslider.onValueChanged.AddListener(SetShootVolume);
    }


    void SetShootVolume(float value)
    {
        Debug.Log("Shoot volume changed: " + value);
        mixer.SetFloat(MIXER_Shoot, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

  

    

}
