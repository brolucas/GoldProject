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

    private bool lastWave = false;



    [SerializeField]
    private GameObject[] listEvent;


    private void Start()
    {
        System.Random alea = new System.Random();
        int eventAlea = alea.Next(3, 5);
        waves[eventAlea]._event = true;
        eventAlea = alea.Next(7, 10);
        waves[eventAlea]._event = true;
    }
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
        if (lastWave)
        {
            this.enabled = false;
        }
        
        
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

        if (wave_Index == waves.Length-1)
        {
            Debug.Log("LAST WAVES ! ");
            lastWave = true;
        }
        else
        {
            wave_Index++;

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
