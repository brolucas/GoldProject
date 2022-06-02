using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class truckDmgVFX : MonoBehaviour
{
    [SerializeField] private Gradient truckColor;
    [SerializeField] private float duration;

    [ContextMenu("color")]
    public void TruckDammageVFX()
    {
        for(int i = 0; i <= 99 ; i++)
        {
            StartCoroutine(GradientTruck(i));
            //Debug.Log(i);
        }
        
        
    }

    IEnumerator GradientTruck(int i)
    {
        yield return new WaitForSeconds(duration * i / 100);
        SpriteRenderer truckRenderer = GetComponent<SpriteRenderer>();
        truckRenderer.color = truckColor.Evaluate((float)i/100);
    }
}
