using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpliterRay : MonoBehaviour
{
    [SerializeField] public Vector3 posOrigin;
    public Vector3 positionTarget;
    public Vector3 positionTarget2;
    private LineRenderer Line;
    public Transform target;
    public Transform target2;
    public bool doubleTarget;

    [SerializeField] ParticleSystem sparks;

    private void Start()
    {
        //posOrigin = transform.position;
        Line = GetComponent<LineRenderer>();
        Line.SetPosition(1, posOrigin);
    }
    private void Update()
    {
        if(positionTarget != null && target != null)
        {
            positionTarget = target.position;
            Line.SetPosition(0, positionTarget);
            Instantiate(sparks, new Vector3(positionTarget.x, positionTarget.y, positionTarget.z - 1), Quaternion.Euler(0, 0, 0), this.transform);
        }
        else
        {
            Line.SetPosition(1, posOrigin);
        }


        if (doubleTarget && positionTarget2 != null && target != null)
        {
            positionTarget2 = target2.position;
            Line.SetPosition(2, positionTarget2);
            Instantiate(sparks, new Vector3(positionTarget2.x, positionTarget2.y, positionTarget2.z - 1), Quaternion.Euler(0, 0, 0), this.transform);
        }
        else
        {
            Line.SetPosition(2, posOrigin);
        }
    }
}
