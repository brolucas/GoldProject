using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private Grid<Cell> grid;
    public int x;
    public int y;

    public bool isTurret;
    public bool isBarricade;
    public bool isEvent;

    public Cell(Grid<Cell> g, int x, int y)
    {
        this.grid = g;
        this.x = x;
        this.y = y;

        this.isTurret = false;
        this.isBarricade = false;
        this.isEvent = false;
    }

}
