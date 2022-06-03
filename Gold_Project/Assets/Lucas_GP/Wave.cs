using UnityEngine;

[System.Serializable]
public class Wave{

    
    public GameObject Wave_Base;
    public int Wave_Count_Base;

    public GameObject Wave_Fast;
    public int Wave_Count_Fast;

    public GameObject Wave_Slow;
    public int Wave_Count_Slow;

    public GameObject Wave_Fly;
    public int Wave_Count_Fly;

    public bool _event;
    public float Wave_Rate;

}
