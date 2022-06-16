using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float speed;
    [SerializeField] GameObject particulesMort;
    [SerializeField] GameObject particulesExplosion;
    [SerializeField] float lifeTime;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = -transform.up * speed;
        StartCoroutine(Delais());
    }

    IEnumerator Delais()
    {
        yield return new WaitForSeconds(lifeTime);
        Instantiate(particulesMort, transform.position, Quaternion.Euler(0, 0, 0));
        Instantiate(particulesExplosion, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
