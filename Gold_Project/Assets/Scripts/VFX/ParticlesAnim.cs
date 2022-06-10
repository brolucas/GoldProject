using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAnim : MonoBehaviour
{
    private Animator Anim;
    void Start()
    {
        Anim = GetComponent<Animator>();
        Anim.SetBool("particles", true);
        
    }

}
