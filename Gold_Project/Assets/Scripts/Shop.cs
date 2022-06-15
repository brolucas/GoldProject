using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{ 
	private GameManager gameManager;
	private DataManager dataManager;

    public Text infoTurretText;
    public Text nameTurretText;

	public List<Button> deckButtons = new List<Button>();
	public List<Button> deckButtonsBarricade = new List<Button>();

	public Dictionary<Button, KindOfTurret> buttonToEnum = new Dictionary<Button, KindOfTurret>();
	public Dictionary<Button, GameObject> buttonToBarricade = new Dictionary<Button, GameObject>();


	public GameObject barr1, barr2;
	public GameObject selectedTurretInGame;
	public Deck deck;

	public void Awake()
	{
		if (gameManager == null)
		{
			gameManager = FindObjectOfType<GameManager>();
		}

		selectedTurretInGame = null;
	}

    private void Start()
    {
        deck = GetComponent<Deck>();
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

		if (SceneManager.GetActiveScene().name == "Level Tuto")
		{
			dataManager.deckData.deckTurret[0] = KindOfTurret.Basic;
			dataManager.deckData.deckTurret[1] = KindOfTurret.Mortar;
			dataManager.deckData.deckTurret[2] = KindOfTurret.Discord;
			dataManager.deckData.deckTurret[3] = KindOfTurret.SniperTower;
			
		}
		for (int i = 0; i < deckButtons.Count; i++)
		{
			buttonToEnum.Add(deckButtons[i], dataManager.deckData.deckTurret[i]);

			TurretData turretData = gameManager.GetStatsKindOfTurret(dataManager.deckData.deckTurret[i]);

			if (turretData.kindOfTurret == KindOfTurret.DefaultDoNotUseIt)
			{
				Debug.LogWarning("There is a turret in the deck that doesn't have a DataBase yet !!");
				continue;
			}

			deckButtons[i].GetComponent<Image>().sprite = turretData.UIDesign;
		}
		buttonToBarricade.Add(deckButtonsBarricade[0], barr1);
		buttonToBarricade.Add(deckButtonsBarricade[1], barr2);

	}

	public void DisplayCurrentTurretStats()
	{
		Turret turret = selectedTurretInGame.GetComponent<Turret>();

		int upgradeCost = 50;

		float range = turret.RangeConvertion(turret.range, false);

		switch (turret.currentLevel)
		{
			case 1:
				upgradeCost = 50;
				infoTurretText.text = ("Level : " + turret.currentLevel + "/" + turret.maxLevel + "\n" +
							   "Upgrade cost : " + (turret.turretPrice + upgradeCost) + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n" +
							   "Target : " + turret.targetType);
				break;
			case 2:
				upgradeCost = 75;
				infoTurretText.text = ("Level : " + turret.currentLevel + "/" + turret.maxLevel + "\n" +
							   "Upgrade cost : " + (turret.turretPrice + upgradeCost) + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n" +
							   "Target : " + turret.targetType);
				break;
			case 3:
				infoTurretText.text = ("Max Level" + "\n" +
							   "No more upgrade" + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n" +
							   "Target : " + turret.targetType);
				break;
			default:
				break;
		}
	}

    public void PurchaseTurret(Button thisButton)
    {
        //Debug.Log("<COLOR=Green>Turret Selcted</COLOR>");
        if (PlayerPrefs.GetInt("tuto", 0) == 1)
        {
            //Debug.Log("<COLOR=Blue>Turret Selcted</COLOR>");
            KindOfTurret kindOfTurret = buttonToEnum[thisButton];

            TurretData turretData = gameManager.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);
            nameTurretText.text = turretData.kindOfTurret.ToString();
            infoTurretText.text = ("Price : " + turretData.turretPrice + "\n" +
                                   "Range : " + turretData.range + "\n" +
                                   "Life Points : " + turretData.healthPoints + "\n" +
                                   "Damage : " + turretData.atqPoints + "\n" +
                                   "Target : " + turretData.targetType);

			BuildManager.Instance.SetTurretToBuild(kindOfTurret);
		}
	}

	public void PurchaseBarricade(Button thisButton)
	{
		if (PlayerPrefs.GetInt("tuto", 0) == 1)
		{
			Debug.Log("<COLOR=Blue>Turret Selcted</COLOR>");
			GameObject barricade = buttonToBarricade[thisButton];

			infoTurretText.text = ("Price : " + barricade.GetComponent<Baricade>().price + "\n" +
								   "Life : " + barricade.GetComponent<Baricade>().hp + "\n");

			BuildManager.Instance.SetBarricadeToBuild(barricade);
		}
	}
	
	public void SellTurret()
	{
		GameObject turret = selectedTurretInGame;
		Turret turretScript = turret.GetComponent<Turret>();

		Pathfinding.Instance.GetGrid().GetXY(turret.transform.position, out int x, out int y);
		Pathfinding.Instance.GetNode(x, y).isTurret = null;
		Pathfinding.Instance.GetNode(x, y).isUsed = false;
		Pathfinding.Instance.mapHasChanged = true;

		GameManager.Instance.truck.gold += turretScript.turretPrice / 2;
		GameManager.Instance.allTurret.Remove(turretScript);
		selectedTurretInGame = null;

		foreach (var enemy in GameManager.Instance.enemies)
		{
			enemy.attackingTurret.Remove(turretScript);
		}

		Destroy(turret.transform.parent.gameObject);
	}

	public void Upgrade()
	{
		Turret turret = selectedTurretInGame.GetComponent<Turret>();

		turret.Upgrade();

		DisplayCurrentTurretStats();
	}
}
