using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dcManch : MonoBehaviour
{
    [SerializeField] private bool alive = true;
    [SerializeField] private List<ParticleSystem> mortManchot;
    private void Update()
    {
        if (!alive)
        {
            for (int i = 0; i < mortManchot.Count; i++)
            {
                Instantiate(mortManchot[i], transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
