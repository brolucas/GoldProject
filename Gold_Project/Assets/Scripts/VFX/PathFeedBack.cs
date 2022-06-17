using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFeedBack : MonoBehaviour
{
    public List<Vector3> path;
    [SerializeField] EnemiesTemp lastEnemy;
    [SerializeField] GameObject linePath;
    LineRenderer line;
    public bool B;

    void Start()
    {
        GameObject linePathLevel = Instantiate(linePath, transform.position, transform.rotation);
        //c est plus simple mwouahaha
        line = linePathLevel.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance.enemies.Count > 0)
        {
            lastEnemy = GameManager.Instance.enemies[GameManager.Instance.enemies.Count-1];
            path = lastEnemy.GetComponent<EnemiesTemp>().pathVectorList;
        }

        if (GameManager.Instance.enemies.Count <= 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                line.SetPosition(i, new Vector3(0,0,0));
            }
        }
        else
        {
            line.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                line.SetPosition(i, path[i]);
            }

        }
        //pa le choix


    }

    private void OnDisable()
    {
            Debug.LogWarning("OnDisable");

    }

  
}
