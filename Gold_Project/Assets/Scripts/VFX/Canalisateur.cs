using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canalisateur : MonoBehaviour
{
    [SerializeField] int charge;
    [SerializeField] bool tir;
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject Electricity;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetInteger("canalisation", charge);
        if (tir)
        {
            tir = false;
            ParticleSystem bolt = Instantiate(particle,new Vector3( transform.position.x , transform.position.y , transform.position.z -1), Quaternion.Euler(0, 0, 0));
            bolt.transform.localScale = new Vector3(.25f + (float)charge/4, .25f + (float)charge / 4, .25f + (float)charge / 4);
            GameObject Projectile = Instantiate(Electricity, transform.position, Quaternion.Euler(0, 0, 0));
            Projectile.transform.localScale = new Vector3(.25f + (float)charge / 10, .25f + (float)charge / 10, .25f + (float)charge / 10);
            charge = 0;
        }
    }
}
