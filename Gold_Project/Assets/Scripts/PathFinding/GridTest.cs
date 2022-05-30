using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    private Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(4, 2, 3f, new Vector3(10, 0));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetTurret(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
