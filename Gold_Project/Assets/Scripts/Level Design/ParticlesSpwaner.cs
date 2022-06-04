using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSpwaner : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Instantiate(particle, transform.position, Quaternion.identity);

        }
    }
}
