using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public Text wave_Text;

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
        wave_Text.text = ("Wave : " + wave_Index.ToString() + " / 10");
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
        if (wave.Wave_nb_Runner > 0)
        {
            for (int i = 0; i < wave.Wave_nb_Runner; i++)
            {
                SpawnEnnemy(wave.Wave_Runner);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
        }

        if (wave.Wave_Manchot_nbr > 0)
        {
            for (int i = 0; i < wave.Wave_Manchot_nbr; i++)
            {
                SpawnEnnemy(wave.Wave_Manchot);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
        }
        if (wave.Wave_Count_Kamikaze > 0)
        {
            for (int i = 0; i < wave.Wave_Count_Kamikaze; i++)
            {
                SpawnEnnemy(wave.Wave_Kamikaze);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
        }
        if (wave.Wave_Count_CRS > 0)
        {
            for (int i = 0; i < wave.Wave_Count_CRS; i++)
            {
                SpawnEnnemy(wave.Wave_CRS);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
        }
        if (wave.Wave_Count_Volant > 0)
        {
            for (int i = 0; i < wave.Wave_Count_Volant; i++)
            {
                SpawnEnnemy(wave.Wave_Volant);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
        }
        if (wave.Wave_Count_Boss > 0)
        {
            for (int i = 0; i < wave.Wave_Count_Boss; i++)
            {
                SpawnEnnemy(wave.Wave_Boss);
                yield return new WaitForSeconds(1f / wave.Wave_Rate);
            }
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
        Instantiate(listEvent[eventAlea], new Vector3(eventAlea,eventAlea,0), spawnPoint.rotation);
        Debug.Log("Event Launched !" + listEvent[eventAlea].ToString());

    }

}
