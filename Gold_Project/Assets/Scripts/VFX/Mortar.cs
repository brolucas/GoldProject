using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float y;
    float x = 0;
    float max = 1;
    public Vector3 target;


    void Update()
    {
        x += .03f;
        transform.localScale = new Vector3(courbe(x),courbe(x),courbe(x));
        if(transform.localScale.z < max)
        {
            Instantiate(explosion, transform.position, Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, .06f);
    }

    float courbe (float x)
    {
        float f2x = (-1 *(x * x)) + (y * x) +1;
        return f2x;
    }
}
