using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    private Grid<Cell> grid;

    BuildManager buildManager;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<Cell>(15, 7, 1f, new Vector3(this.transform.position.x, this.transform.position.y), (Grid<Cell> g, int x, int y) => new Cell(g, x, y));

        buildManager = BuildManager.Instance;
    }

    private void Update()
    {
        if (buildManager.GetTurretToBuild() == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            grid.SetTurret(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
