using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpliterRay : MonoBehaviour
{
    [SerializeField] Vector3 postionOrigine;
    public Vector3 positionTarget;
    public Vector3 positionTarget2;
    private LineRenderer Line;
    public Transform target;
    public Transform target2;
    [SerializeField] bool doubleTarget;
    private void Start()
    {
        postionOrigine = transform.position;
        Line = GetComponent<LineRenderer>();
        Line.SetPosition(1, postionOrigine);
    }
    private void Update()
    {
        positionTarget = target.position;
        Line.SetPosition(0, positionTarget);
        if (doubleTarget)
        {
            positionTarget2 = target2.position;
            Line.SetPosition(2, positionTarget2);
        }
        else
        {
            Line.SetPosition(2, postionOrigine);
        }
    }
}
