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
	public Button button1, button2;

	public Dictionary<Button, KindOfTurret> buttonToEnum = new Dictionary<Button, KindOfTurret>();
	public Dictionary<Button, GameObject> buttonToBarricade = new Dictionary<Button, GameObject>();


	public GameObject barr1, barr2;
	public GameObject selectedItemInGame;
	public Deck deck;

	public void Awake()
	{
		if (gameManager == null)
		{
			gameManager = FindObjectOfType<GameManager>();
		}

		selectedItemInGame = null;
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
			dataManager.deckData.deckTurret[2] = KindOfTurret.Furnace;
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


		buttonToBarricade.Add(button1, barr1);
		buttonToBarricade.Add(button2, barr2);

	}

	public void DisplayCurrentBarricadeStats()
    {
		infoTurretText.text = ("Remaining life : " + selectedItemInGame.GetComponent<Baricade>().hp + "\n");
	}

	public void DisplayCurrentTurretStats()
	{
		Turret turret = selectedItemInGame.GetComponent<Turret>();

		int upgradeCost = 50;

		float range = turret.RangeConvertion(turret.range, false);

		switch (turret.currentLevel)
		{
			case 1:
                nameTurretText.text = turret.kindOfTurret.ToString();
				upgradeCost = 50;
				infoTurretText.text = ("Level : " + turret.currentLevel + "/" + turret.maxLevel + "\n" +
							   "Upgrade cost : " + (turret.turretPrice + upgradeCost) + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n");
				break;
			case 2:
                nameTurretText.text = turret.kindOfTurret.ToString();
				upgradeCost = 75;
				infoTurretText.text = ("Level : " + turret.currentLevel + "/" + turret.maxLevel + "\n" +
							   "Upgrade cost : " + (turret.turretPrice + upgradeCost) + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n");
				break;
			case 3:
                nameTurretText.text = turret.kindOfTurret.ToString();
				infoTurretText.text = ("Max Level" + "\n" +
							   "No more upgrade" + "\n" +
							   "Range : " + range + "\n" +
							   "HP : " + turret.maxHealthPoint + "/" + turret.currentHP + "\n" +
							   "Damage : " + turret.atqPoints + "\n");
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
                                   "Damage : " + turretData.atqPoints + "\n");

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
	
	public void Sell()
    {
		Pathfinding.Instance.GetGrid().GetXY(selectedItemInGame.transform.position, out int x, out int y);
        if (Pathfinding.Instance.GetNode(x, y).isTurret != null)
        {
			Pathfinding.Instance.GetNode(x, y).isTurret = null;
			Pathfinding.Instance.GetNode(x, y).isUsed = false;
			Pathfinding.Instance.mapHasChanged = true;
			SellTurret();
		}
		else if(Pathfinding.Instance.GetNode(x, y).isBarricade != null)
        {
			Pathfinding.Instance.GetNode(x, y).isBarricade = null;
			Pathfinding.Instance.GetNode(x, y).isUsed = false;
			Pathfinding.Instance.mapHasChanged = true;
			SellBarricade();
		}
	}

	public void SellTurret()
	{
		GameObject turret = selectedItemInGame;
		Turret turretScript = turret.GetComponent<Turret>();

		GameManager.Instance.truck.gold += turretScript.turretPrice / 2;
		GameManager.Instance.allTurret.Remove(turretScript);

		foreach (var enemy in GameManager.Instance.enemies)
		{
			enemy.attackingTurret.Remove(turretScript);
		}

		Destroy(turret.transform.parent.gameObject);
		selectedItemInGame = null;
	}

	public void SellBarricade()
    {
		GameManager.Instance.truck.gold += selectedItemInGame.GetComponent<Baricade>().price / 2;
		GameManager.Instance.allBarricade.Remove(selectedItemInGame.GetComponent<Baricade>());
		Destroy(selectedItemInGame.gameObject);
		selectedItemInGame = null;
	}

	public void Upgrade()
	{
		Turret turret = selectedItemInGame.GetComponent<Turret>();

		int upgradeCost = turret.turretPrice + 50;

		switch (turret.currentLevel)
		{
			case 1:
				upgradeCost = turret.turretPrice + 50;
				break;
			case 2:
				upgradeCost = turret.turretPrice + 75;
				break;
		}

		if (turret.isMaxLevel || turret.currentLevel >= turret.maxLevel || gameManager.truck.gold < upgradeCost)
        {
			return;
        }

		if (BuildManager.Instance.turretToBuild == KindOfTurret.DefaultDoNotUseIt)
        {
            turret.Upgrade();

            DisplayCurrentTurretStats();

			turret.anim.SetBool("upgrading", true);

			PlayerPrefs.SetInt("TowerUpgraded", PlayerPrefs.GetInt("TowerUpgraded") + 1);
		}
		
	}
}
