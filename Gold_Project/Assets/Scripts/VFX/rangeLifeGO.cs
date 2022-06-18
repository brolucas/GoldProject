using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangeLifeGO : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(Delais());



        }
    }
        IEnumerator Delais()
        {
        yield return new WaitForSeconds(.075f);
            Destroy(gameObject);
        }
}
