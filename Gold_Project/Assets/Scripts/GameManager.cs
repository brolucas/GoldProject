using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<EnemiesTemp> enemies = new List<EnemiesTemp>();

    public List<Turret> allTurret = new List<Turret>();
    public List<Baricade> allBarricade = new List<Baricade>();

    private static GameManager instance = null;

    public GameObject rangeSprite;

    public TurretDatabase turretDatabase;

    public truck truck;

    public DataManager dataManager;
    //public DeckData deckData;
    public Deck deck;

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

    public void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;

        dataManager = DataManager.Instance;
        /*if (deckData == null)
        {
            deckData = dataManager.deckData;
        }*/
        //PlayerPrefs.DeleteAll();

        currentScene = SceneManager.GetActiveScene();

        enemies.Clear();

        shop = FindObjectOfType<Shop>();

        foreach (TurretData turretData in turretDatabase.turrets)
        {
            turretData.description = ("Name : " + turretData.label + "\n\n" +
                                      "Cost : " + turretData.turretPrice + "\n" +
                                      "HP : " + turretData.healthPoints + "\n" +
                                      "Range : " + turretData.range + "\n" +
                                      "Damage : " + turretData.atqPoints + "\n" +
                                      "Fire rate : " + turretData.fireRate + "/s" + "\n");
        }

        if (currentScene.name == "MainMenu")
        {
            GetComponent<TimerController>().enabled = false;
            GetComponent<WaveSpawner>().enabled = false;
            
            for(int i = 0; i < deck.deckButton.Count; i++)
            {
                dataManager.deckData.deckTurret[i] = KindOfTurret.DefaultDoNotUseIt;
            }

            // For each slot add a type to it
            for (int i = 0; i < deck.deckButton.Count; i++)
            {
                deck.buttonToEnumDeck.Add(deck.deckButton[i], dataManager.deckData.deckTurret[i]);
            }


        }
    }

    private void Start()
    {
        deck = GetComponent<Deck>();

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            for (int i = 0; i < deck.deckButton.Count; i++)
            {
                deck.deckButton[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }

        }
    }
    public void Update()
    {
        dataManager = DataManager.Instance;
    }
    public TurretData GetStatsKindOfTurret(KindOfTurret kindOfTurret)
    {
        // Get the stats in the scriptable object TurretDatabase based on the type kindOfTurret

        TurretData turretData = GameManager.Instance.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

        return turretData;
    }

    public void TryToAddTurretInDeck(KindOfTurret kindOfTurret)
    {
        for (int i = 0; i < deck.deckButton.Count; i++)
        {
            if (deck.buttonToEnumDeck[deck.deckButton[i]] == KindOfTurret.DefaultDoNotUseIt)
            {
                //Change value in the dictionnary
                deck.buttonToEnumDeck[deck.deckButton[i]] = kindOfTurret;

                //Change value in the DECK in dataManager.deckData
                dataManager.deckData.deckTurret[i] = kindOfTurret;

                deck.deckButton[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                deck.deckButton[i].GetComponent<Image>().sprite = GetStatsKindOfTurret(kindOfTurret).UIDesign;
                break;
            }
        }
    }

    public void RemoveTurretFromDeck(KindOfTurret kindOfTurret, Button slotButton)
    {
        for (int i = 0; i < dataManager.deckData.deckTurret.Count; i++)
        {
            if (dataManager.deckData.deckTurret[i] == kindOfTurret)
            {
                dataManager.deckData.deckTurret[i] = KindOfTurret.DefaultDoNotUseIt;
                break;
            }
        }
    }
}
