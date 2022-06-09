using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<EnemiesTemp> enemies = new List<EnemiesTemp>();

    public List<Turret> allTurret = new List<Turret>();

    private static GameManager instance = null;

    public GameObject rangeSprite;

    public TurretDatabase turretDatabase;

    public truck truck;

    public List<Button> deckButton = new List<Button>();
    public List<KindOfTurret> deckTurret = new List<KindOfTurret>();

    public Dictionary<Button, KindOfTurret> buttonToEnumDeck = new Dictionary<Button, KindOfTurret>();

    private Shop shop;
    public List<GameObject> baricades = new List<GameObject>();
    public UnityEngine.SceneManagement.Scene currentScene { get; set; }

    // Game Instance Singleton
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        currentScene = SceneManager.GetActiveScene();

        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;

        enemies.Clear();

        for (int i = 0; i < 4; i++)
        {
            deckTurret.Add(KindOfTurret.DefaultDoNotUseIt);
        }

        shop = FindObjectOfType<Shop>();

        DontDestroyOnLoad(this.gameObject);

        foreach (TurretData turretData in turretDatabase.turrets)
        {
            turretData.description = ("Name : " + turretData.label + "\n\n" +
                                      "Cost : " + turretData.turretPrice + "\n" +
                                      "HP : " + turretData.healthPoints + "\n" +
                                      "Range : " + turretData.range + "\n" +
                                      "Damage : " + turretData.atqPoints + "\n" +
                                      "Fire rate : " + turretData.fireRate + "/s"+ "\n" +
                                      "Target : " + turretData.targetType);
        }

        if (currentScene.name == "MainMenu")
        {
            GetComponent<TimerController>().enabled = false;
            GetComponent<WaveSpawner>().enabled = false;

            for (int i = 0; i < deckButton.Count; i++)
            {
                buttonToEnumDeck.Add(deckButton[i], deckTurret[i]);

                Debug.Log("Main Menu");
            }
        }
        else
        {
            /*deckButton.Clear();


            GetComponent<TimerController>().enabled = true;
            GetComponent<WaveSpawner>().enabled = true;

            shop.deck.Clear();
            deckButton.Clear();

            Destroy(GetComponent<Deck>());

            for (int i = 0; i < deckTurret.Count; i++)
            {
                shop.deck.Add(deckTurret[i]);
            }
            Debug.Log("Other Scene");*/

        }
    }

    public TurretData GetStatsKindOfTurret(KindOfTurret kindOfTurret)
    {
        // Get the stats in the scriptable object TurretDatabase based on the type kindOfTurret

        TurretData turretData = GameManager.Instance.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

        return turretData;
    }

    public void TryToAddTurretInDeck(KindOfTurret kindOfTurret)
    {
        for (int i = 0; i < deckButton.Count; i++)
        {
            if (buttonToEnumDeck[deckButton[i]] == KindOfTurret.DefaultDoNotUseIt)
            {
                buttonToEnumDeck[deckButton[i]] = kindOfTurret;
                deckTurret[i] = kindOfTurret;
                deckButton[i].GetComponent<Image>().sprite = GetStatsKindOfTurret(kindOfTurret).UIDesign;
                break;
            }

            /*if (deckTurret.Count == 0)
                return;

            if (GetStatsKindOfTurret(deckTurret[i]) != null)
            {
                buttonToEnumDeck.Add(deckButton[i], deckTurret[i]);
            }*/

            /*if (GetStatsKindOfTurret(deckTurret[i]) == null)
            {
                deckButton[i].GetComponent<Image>().sprite = null;
            }
            else
            {
                deckButton[i].GetComponent<Image>().sprite = GetStatsKindOfTurret(deckTurret[i]).UIDesign;
            }*/
        }
    }

    public void RemoveTurretFromDeck(KindOfTurret kindOfTurret)
    {
        /*buttonToEnumDeck.ContainsValue(kindOfTurret) = kindOfTurret;
        

        for (int i = 0; i < deckTurret.Count; i++)
        {
            if (deckTurret[i] == kindOfTurret)
            {
                deckTurret[i] = KindOfTurret.DefaultDoNotUseIt;
                break;
            }

            *//*if (deckTurret.Count == 0)
                return;

            if (GetStatsKindOfTurret(deckTurret[i]) != null)
            {
                buttonToEnumDeck.Add(deckButton[i], deckTurret[i]);
            }*/

            /*if (GetStatsKindOfTurret(deckTurret[i]) == null)
            {
                deckButton[i].GetComponent<Image>().sprite = null;
            }
            else
            {
                deckButton[i].GetComponent<Image>().sprite = GetStatsKindOfTurret(deckTurret[i]).UIDesign;
            }*//*
        }*/
    }
}
