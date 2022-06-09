using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{ 
    private BuildManager buildManager;
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
        buildManager = BuildManager.Instance;
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

        for (int i = 0; i < deckButtons.Count; i++)
        {
            buttonToEnum.Add(deckButtons[i], dataManager.deckData.deckTurret[i]);

            TurretData turretData = gameManager.GetStatsKindOfTurret(dataManager.deckData.deckTurret[i]);

            if (turretData == null)
            {
                Debug.LogError("There is a turret in the deck that doesn't have a DataBase yet !!");
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
        KindOfTurret kindOfTurret = buttonToEnum[thisButton];

        TurretData turretData = gameManager.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

        infoTurretText.text = ("Price : " + turretData.turretPrice + "\n" +
                               "Range : " + turretData.range + "\n" +
                               "Life Points : " + turretData.healthPoints + "\n" +
                               "Damage : " + turretData.atqPoints +"\n"+
                               "Target : " + turretData.targetType);

        buildManager.SetTurretToBuild(kindOfTurret);
    }
    
    public void SellTurret()
    {
        GameObject turret = selectedTurretInGame;

        GameManager.Instance.truck.gold += selectedTurretInGame.GetComponent<Turret>().turretPrice / 2;

        selectedTurretInGame = null;

        Destroy(turret.transform.parent.gameObject);
    }

    public void Upgrade()
    {
        Turret turret = selectedTurretInGame.GetComponent<Turret>();

        turret.Upgrade();

        DisplayCurrentTurretStats();
    }
}
