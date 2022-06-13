using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float y;
    float x = 0;
    float max = 1;

    void Update()
    {
        x += .03f;
        transform.localScale = new Vector3(courbe(x),courbe(x),courbe(x));
        if(transform.localScale.z < max)
        {
            Instantiate(explosion, transform.position, Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }
    }

    float courbe (float x)
    {
        float f2x = (-1 *(x * x)) + (y * x) +1;
        return f2x;
    }
}
