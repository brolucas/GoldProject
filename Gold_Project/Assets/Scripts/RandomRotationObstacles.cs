using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationObstacles : MonoBehaviour
{
    public GameObject obstacle;
    private int angle;

    void Start()
    {
        obstacle = GetComponent<Transform>().gameObject;
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                angle = 90;
                break;
            case 1:
                angle = 180;
                break;
            case 2:
                angle = -90;
                break;
            default:
                angle = 0;
                break;

        }

        obstacle.transform.rotation = Quaternion.Euler(0,0,angle);
    }

}
