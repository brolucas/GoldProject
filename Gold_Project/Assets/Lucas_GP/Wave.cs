using UnityEngine;

[System.Serializable]
public class Wave{

    
    public GameObject Wave_Runner;
    public int Wave_nb_Runner;

    public GameObject Wave_Manchot;
    public int Wave_Manchot_nbr;

    public GameObject Wave_Kamikaze;
    public int Wave_Count_Kamikaze;

    public GameObject Wave_CRS;
    public int Wave_Count_CRS;

    public GameObject Wave_Volant;
    public int Wave_Count_Volant;

    public GameObject Wave_Boss;
    public int Wave_Count_Boss;

    public bool _event;
    public float Wave_Rate;

}
