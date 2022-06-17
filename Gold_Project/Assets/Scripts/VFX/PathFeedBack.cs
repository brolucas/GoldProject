using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFeedBack : MonoBehaviour
{
    public List<Vector3> path;
    [SerializeField] EnemiesTemp lastEnemy;
    [SerializeField] GameObject linePath;
    LineRenderer line;

    void Start()
    {
        GameObject linePathLevel = Instantiate(linePath, transform.position, transform.rotation);
        //c est plus simple mwouahaha
        line = linePathLevel.GetComponent<LineRenderer>();
    }

    void Update()
    {
        lastEnemy = GetComponent<GameManager>().enemies[GetComponent<GameManager>().enemies.Count-1];
        path = lastEnemy.GetComponent<EnemiesTemp>().pathVectorList;
        line.positionCount = path.Count;
        //pa le choix
        for (int i = 0; i < path.Count; i++)
        {
            line.SetPosition(i, path[i]);
        }
    }
}
