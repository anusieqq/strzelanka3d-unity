using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBox : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("Open Door", true);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetBool("Open Door", false);
        }
    }
}
