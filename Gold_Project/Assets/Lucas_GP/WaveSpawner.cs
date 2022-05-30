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

        for (int i = 0; i < wave.Wave_Count; i++)
        {
            SpawnEnnemy(wave.Wave_Enemy);
            yield return new WaitForSeconds(1f/wave.Wave_Rate);
        }
        wave_Index++;

        if (wave_Index == waves.Length)
        {
            Debug.Log("Level Complete ! Congratulation");
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
        Instantiate(listEvent[0], spawnPoint.position, spawnPoint.rotation);

    }
}
