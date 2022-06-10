using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
	[SerializeField]
	public static int enemyAlive = 0;

	public Wave[] waves;

	[SerializeField]
	private List<Transform> spawnPoint = new List<Transform>();

	[SerializeField]
	private Transform spawnPoint2;

	[SerializeField]
	private float timeBetweenWave = 5f;

	[SerializeField]
	private float countdown = 2f;

	private int wave_Index = 0;
	public Text wave_Text;

	private bool lastWave = false;

	public GameObject wave_Victory_Screen;

	[SerializeField]
	private GameObject[] listEvent;

	private bool notDone = false;


	private void Start()
	{
		System.Random alea = new System.Random();
		int eventAlea = alea.Next(3, 5);
		waves[eventAlea]._event = true;
		eventAlea = alea.Next(7, 10);
		waves[eventAlea]._event = true;
		enemyAlive = 0;
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
		if (lastWave && SceneManager.GetActiveScene().name != "MainMenu")
		{
			if (enemyAlive <= 0)
			{
				wave_Victory_Screen.SetActive(true);
				if (!notDone)
				{
					GoToGame.levelIndex++;
					notDone = true;
					if (GoToGame.levelIndex >= 10)
					{
						SceneManager.LoadScene("Credits");
						GoToGame.levelIndex = 1;
					}
				}
				//Time.timeScale = 0;
			}
			//this.enabled = false;
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
		System.Random alea = new System.Random();
		int Alea = alea.Next(0,spawnPoint.Capacity);
		Instantiate(ennemy, spawnPoint[Alea].position, spawnPoint[Alea].rotation);
		enemyAlive++;
	}
	public void SpawnEvent()
	{
		System.Random alea = new System.Random();
		int noevent = alea.Next(0,3);
		int x1 = alea.Next(1, 11);
		int y1 = alea.Next(0, 6);
		Vector3 temp = new Vector3(x1, y1, 0);

		switch (noevent)
		{
			case 0:
				Debug.Log("Event Launched !" + listEvent[0].ToString());

				Pathfinding.Instance.GetGrid().GetXY(temp, out int x, out int y);
				Pathfinding.Instance.GetNode(x, y).isEvent = listEvent[0];
				Pathfinding.Instance.GetNode(x + 1, y).isEvent = listEvent[0];
				Pathfinding.Instance.GetNode(x, y).isUsed = true;
				Pathfinding.Instance.GetNode(x + 1, y).isUsed = true;
				Vector3 position = Pathfinding.Instance.GetGrid().GetWorldPosition(x,y);
				position = new Vector3(position.x + Pathfinding.Instance.GetGrid().cellSize / 2, position.y + Pathfinding.Instance.GetGrid().cellSize / 2);
				
				Vector3 position1 = Pathfinding.Instance.GetGrid().GetWorldPosition(x+1, y);
				position1 = new Vector3(position1.x + Pathfinding.Instance.GetGrid().cellSize / 2, position1.y + Pathfinding.Instance.GetGrid().cellSize / 2);
				
				GameObject istevent = Instantiate(listEvent[0], position, Quaternion.identity);
				GameObject istevent1 = Instantiate(listEvent[0], position1, Quaternion.identity);

				// istevent.transform.GetChild(1).localScale = new Vector3(0, 0, 0);
				break;

			case 1:
				Debug.Log("Event Launched !" + listEvent[1].ToString());
				
				if (GameManager.Instance.baricades.Capacity != 0)
				{
					foreach(GameObject element in GameManager.Instance.baricades)
					{
						element.GetComponent<Baricade>().takeDamage((element.GetComponent<Baricade>().baseHp * 33) / 100);

					}
				}
				break;
			case 2:
				Debug.Log("Event Launched !" + listEvent[2].ToString());
				spawnPoint.Add(spawnPoint2);
				break;
			case 3:
				Debug.Log("Event Launched !" + listEvent[3].ToString());
				break;
		}
		

	}

}
