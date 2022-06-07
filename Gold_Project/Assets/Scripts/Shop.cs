using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private TurretDatabase turretDatabase;

    private BuildManager buildManager;
    private GameManager gameManager;

    public Text infoTurretText;

    public List<KindOfTurret> deck = new List<KindOfTurret>();

    public List<Button> deckButtons = new List<Button>();

    public Dictionary<Button, KindOfTurret> buttonToEnum = new Dictionary<Button, KindOfTurret>();

    public GameObject selectedTurretInGame;

    public void Awake()
    {
        if (turretDatabase == null)
        {
            TurretDatabase turretDatabaseNew = (TurretDatabase)ScriptableObject.CreateInstance(typeof(TurretDatabase));

            turretDatabase = turretDatabaseNew;
        }
    }

    private void Start()
    {
        buildManager = BuildManager.Instance;
        gameManager = GameManager.Instance;

        for (int i = 0; i < deckButtons.Count; i++)
        {
            buttonToEnum.Add(deckButtons[i], deck[i]);

            TurretData turretData = turretDatabase.turrets.Find(data => data.kindOfTurret == deck[i]);

            deckButtons[i].GetComponent<Image>().sprite = turretData.UIDesign;
        }
    }

    public void PurchaseTurret(Button thisButton)
    {
        KindOfTurret kindOfTurret = buttonToEnum[thisButton];

        TurretData turretData = turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

        infoTurretText.text = ("Price : " + turretData.turretPrice + "\n" +
                               "Range : " + turretData.range + "\n" +
                               "Life Points : " + turretData.healthPoints + "\n" +
                               "Damage : " + turretData.atqPoints);

        buildManager.SetTurretToBuild(kindOfTurret);
    }
    
    public void SellTurret()
    {
        GameObject turret = selectedTurretInGame;

        GameManager.Instance.truck.gold += selectedTurretInGame.GetComponent<Turret>().turretPrice / 2;

        selectedTurretInGame = null;

        Destroy(turret.transform.parent.gameObject);
    }
}
