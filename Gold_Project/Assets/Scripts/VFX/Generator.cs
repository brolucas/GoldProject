using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] GameObject projecile;
    [SerializeField] float delais = 2;
    bool cd = true;
    // Update is called once per frame
    void Update()
    {
        if (cd)
        {
            StartCoroutine(Delais());
            cd = false;
        }
    }
    IEnumerator Delais()
    {
        yield return new WaitForSeconds(delais);
        cd = true;
        Instantiate(projecile, transform.position, Quaternion.Euler(0, 0, 0));
    }
}
