using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretParticleSript : MonoBehaviour
{
    [SerializeField] private ParticleSystem particules;

    [ContextMenu("Particles")]
    public void particlesTurret()
    {
        Instantiate(particules, transform.position, transform.rotation);
    }
    
}
