using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canalisateur : MonoBehaviour
{
    [SerializeField] int charge;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetInteger("canalisation", charge);
    }
}
