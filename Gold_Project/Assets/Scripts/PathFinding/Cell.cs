using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool isTurret;
    public bool isBarricade;
    public bool isEvent;

    public Cell()
    {
        this.isTurret = false;
        this.isBarricade = false;
        this.isEvent = false;
    }

}
