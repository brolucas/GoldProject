using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{ 
    private GameManager gameManager;
    private DataManager dataManager;

    public Text infoTurretText;

    public List<Button> deckButtons = new List<Button>();

    public Dictionary<Button, KindOfTurret> buttonToEnum = new Dictionary<Button, KindOfTurret>();

    public GameObject selectedTurretInGame;

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
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

        for (int i = 0; i < deckButtons.Count; i++)
        {
            buttonToEnum.Add(deckButtons[i], dataManager.deckData.deckTurret[i]);

            TurretData turretData = gameManager.GetStatsKindOfTurret(dataManager.deckData.deckTurret[i]);

            if (turretData == null)
            {
                Debug.LogWarning("There is a turret in the deck that doesn't have a DataBase yet !!");
                continue;
            }

            deckButtons[i].GetComponent<Image>().sprite = turretData.UIDesign;
        }
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
        if (PlayerPrefs.GetInt("tuto", 0) == 1)
        {
            KindOfTurret kindOfTurret = buttonToEnum[thisButton];

            TurretData turretData = gameManager.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

            infoTurretText.text = ("Price : " + turretData.turretPrice + "\n" +
                                   "Range : " + turretData.range + "\n" +
                                   "Life Points : " + turretData.healthPoints + "\n" +
                                   "Damage : " + turretData.atqPoints + "\n" +
                                   "Target : " + turretData.targetType);

            BuildManager.Instance.SetTurretToBuild(kindOfTurret);
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
