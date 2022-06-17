using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanalisateurProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = -transform.up * speed;
    }


}
