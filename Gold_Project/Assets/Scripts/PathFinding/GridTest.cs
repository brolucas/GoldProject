using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    private Grid grid;

    BuildManager buildManager;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(4, 2, 3f, new Vector3(this.transform.position.x, this.transform.position.y));

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
