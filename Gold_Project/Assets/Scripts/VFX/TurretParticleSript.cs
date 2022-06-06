using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretParticleSript : MonoBehaviour
{
    [SerializeField] private ParticleSystem particules;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Instantiate(particules, transform.position, transform.rotation);
    }

   
}
