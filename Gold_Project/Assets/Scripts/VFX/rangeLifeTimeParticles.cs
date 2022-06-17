using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangeLifeTimeParticles : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float rangeTarget;
    [SerializeField] float lifeTime;
    private ParticleSystem particles;
    [SerializeField] float y;


    private void Update()
    {
        if (target == null)
            return;

        particles = GetComponent<ParticleSystem>();
        lifeTime = particles.startSpeed;
        var main = particles.main;
        main.startLifetime = Pythagore(transform.position.x - target.transform.position.x, transform.position.y - target.transform.position.y) * particles.startSpeed / y;

    }

    private float Pythagore(float nb1, float nb2)
    {
        return Mathf.Sqrt((nb1 * nb1) + (nb2 * nb2));
    }

}
