using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    public EnemiesTemp enemy;
    private Pathfinding pathfinding;
    public GameObject spawn, end;
    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(15, 7, this.transform);
        enemy.endPoint = end;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EnemiesTemp enemyTest = Instantiate(enemy, spawn.transform.position, spawn.transform.rotation);
        }
    }
}
