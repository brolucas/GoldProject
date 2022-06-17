using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangeLifeGO : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("hi");
            Destroy(gameObject);
            
            
        }
    }
}
