using System;
using System.Collections;
using UnityEngine;

enum State
{
    Wait,
    Play,
    Event
}
public class WaveSpawner : MonoBehaviour
{
    private State WS_State = State.Wait;
    [SerializeField]
    private Transform WS_Ennemy_Prefab;

    [SerializeField]
    private Transform WS_Spawn_Point;

    [SerializeField]
    private float WS_Time_Between_Wave = 5f;

    private float WS_Countdown = 2f;

    private int WS_Wave_Index = 5;

    private int WS_Number_Ennemy_In_Wave;

    [SerializeField]
    private Event[] WS_List_Event;

    private Boolean WS_Event = false;


    // Update is called once per frame
    void Update()
    {
        if (WS_Countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            WS_Countdown = WS_Time_Between_Wave;
            WS_State = State.Play;
        }
        if (WS_State == State.Wait)
        {
            WS_Countdown -= Time.deltaTime;

        }
        if (WS_Number_Ennemy_In_Wave == 0 )
        {
            SpawnEvent();
            WS_State = State.Wait;
        }
        /*if (WS_State == State.Event)
        {
            SpawnEvent();
            WS_State = State.Wait;
        }*/
        
    }

    IEnumerator SpawnWave()
    {
        WS_Wave_Index++;

        Debug.Log("Apparition d'une vague");

        for (int i = 0; i < WS_Wave_Index; i++)
        {
            SpawnEnnemy();
            yield return new WaitForSeconds(0.5f);
        }
        WS_Number_Ennemy_In_Wave = 0;

    }

    void SpawnEnnemy()
    {
        Instantiate(WS_Ennemy_Prefab, WS_Spawn_Point.position, WS_Spawn_Point.rotation);
    }
    void SpawnEvent()
    {
        Instantiate(WS_List_Event[0], WS_Spawn_Point.position, WS_Spawn_Point.rotation);
        WS_Number_Ennemy_In_Wave = WS_Wave_Index;

    }
}
