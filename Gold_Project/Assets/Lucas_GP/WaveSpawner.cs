using System;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    public static int enemyAlive = 0;

    public Wave[] waves;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private float timeBetweenWave = 5f;

    private float countdown = 2f;

    private int wave_Index = 0;



    [SerializeField]
    private GameObject[] listEvent;



    // Update is called once per frame
    void Update()
    {
        if (enemyAlive > 0)
        {
            return;
        }
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWave;
            return;
        }
         countdown -= Time.deltaTime;

        
        
    }

    IEnumerator SpawnWave()
    {
        Wave wave = waves[wave_Index];

        Debug.Log("Apparition d'une vague");

        if (wave._event)
        {
            SpawnEvent();
        }
        for (int i = 0; i < wave.Wave_Count_Fast; i++)
        {
            SpawnEnnemy(wave.Wave_Fast);
            yield return new WaitForSeconds(1f/wave.Wave_Rate);
        }
        for (int i = 0; i < wave.Wave_Count_Slow; i++)
        {
            SpawnEnnemy(wave.Wave_Slow);
            yield return new WaitForSeconds(1f / wave.Wave_Rate);
        }
        for (int i = 0; i < wave.Wave_Count_Base; i++)
        {
            SpawnEnnemy(wave.Wave_Base);
            yield return new WaitForSeconds(1f / wave.Wave_Rate);
        }
        for (int i = 0; i < wave.Wave_Count_Fly; i++)
        {
            SpawnEnnemy(wave.Wave_Fly);
            yield return new WaitForSeconds(1f / wave.Wave_Rate);
        }
        wave_Index++;

        if (wave_Index == waves.Length)
        {
            Debug.Log("LAST WAVES ! ");
            this.enabled = false;
        }

    }

    void SpawnEnnemy(GameObject ennemy)
    {
        Instantiate(ennemy, spawnPoint.position, spawnPoint.rotation);
        enemyAlive++;
    }
    public void SpawnEvent()
    {
        System.Random alea = new System.Random();
        int eventAlea = alea.Next(0, 10);
        Instantiate(listEvent[eventAlea], spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Event Launched !" + listEvent[eventAlea].ToString());

    }

}
