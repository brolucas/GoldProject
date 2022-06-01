using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartcilesAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5;
    void Start()
    {
        StartCoroutine(SelfDestroy(lifeTime));
    }

    IEnumerator SelfDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
