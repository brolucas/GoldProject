using System;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    public static int WS_Enemy_Alives = 0;

    public Wave[] WS_Waves;

    [SerializeField]
    private Transform WS_Spawn_Point;

    [SerializeField]
    private float WS_Time_Between_Wave = 5f;

    private float WS_Countdown = 2f;

    private int WS_Wave_Index = 0;


    [SerializeField]
    private Event[] WS_List_Event;



    // Update is called once per frame
    void Update()
    {
        if (WS_Enemy_Alives > 0)
        {
            return;
        }
        if (WS_Countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            WS_Countdown = WS_Time_Between_Wave;
            return;
        }
        
         WS_Countdown -= Time.deltaTime;

        
        
    }

    IEnumerator SpawnWave()
    {
        Wave wave = WS_Waves[WS_Wave_Index];

        Debug.Log("Apparition d'une vague");

        for (int i = 0; i < wave.Wave_Count; i++)
        {
            SpawnEnnemy(wave.Wave_Enemy);
            yield return new WaitForSeconds(1f/wave.Wave_Rate);
        }
        WS_Wave_Index++;

    }

    void SpawnEnnemy(GameObject ennemy)
    {
        Instantiate(ennemy, WS_Spawn_Point.position, WS_Spawn_Point.rotation);
        WS_Enemy_Alives++;
    }
    void SpawnEvent()
    {
        Instantiate(WS_List_Event[0], WS_Spawn_Point.position, WS_Spawn_Point.rotation);

    }
}
