using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour
{
    public List<Button> inventoryButton = new List<Button>();
    public Dictionary<Button, KindOfTurret> buttonToEnumInventory = new Dictionary<Button, KindOfTurret>();

    [SerializeField]
    private Image AddDeckImage;

    public KindOfTurret turretSelected;

    public Image turretImageUI;

    public Text description;

    private GameManager gameManager;

    [SerializeField]
    private GameObject TurretDeckBuilder;

    public void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager.currentScene.name != "MainMenu")
            return;

        /*TurretData turretData = GameManager.Instance.GetStatsKindOfTurret(KindOfTurret.SniperTower);
        description.text = turretData.description;

        */

        for (int i = 0; i < gameManager.turretDatabase.turrets.Count; i++)
        {
            buttonToEnumInventory.Add(inventoryButton[i], gameManager.turretDatabase.turrets[i].kindOfTurret);

            inventoryButton[i].GetComponent<Image>().sprite = gameManager.turretDatabase.turrets[i].UIDesign;

            /*TurretData turretData = turretDatabase.turrets.Find(data => data.kindOfTurret == deck[i]);

            if (turretData == null)
            {
                Debug.LogError("There is a turret in the deck that doesn't have a DataBase yet !!");
                continue;
            }

            deckButtons[i].GetComponent<Image>().sprite = turretData.UIDesign;*/
        }

        TurretDeckBuilder.SetActive(false);
    }

    public void SetSelectedTurretViaInventory(Button thisButton)
    {
        KindOfTurret kindOfTurret = buttonToEnumInventory[thisButton];

        SetTurretSelected(thisButton, kindOfTurret);
    }

    public void SetSelectedTurretViaDeck(Button thisButton)
    {
        KindOfTurret kindOfTurret = gameManager.buttonToEnumDeck[thisButton];

        SetTurretSelected(thisButton, kindOfTurret);
    }

    public void SetTurretSelected(Button thisButton, KindOfTurret kindOfTurret)
    {
        turretSelected = kindOfTurret;

        if (TurretDeckBuilder.activeSelf == false)
        {
            TurretDeckBuilder.SetActive(true);
        }

        TurretData turretData = gameManager.GetStatsKindOfTurret(kindOfTurret);

        description.text = turretData.description;
        turretImageUI.sprite = turretData.UIDesign;

        if (gameManager.deckTurret.Contains(kindOfTurret))
        {
            AddDeckImage.color = Color.red;
        }
        else
        {
            AddDeckImage.color = Color.white;
        }
    }

    public void AddToDeck()
    {
        if (!gameManager.deckTurret.Contains(KindOfTurret.DefaultDoNotUseIt))
            return;

        if (gameManager.deckTurret.Contains(turretSelected))
            //You can't add more than 4 turrets 
            // Already in the deck 
            return;

        gameManager.TryToAddTurretInDeck(turretSelected);

        TurretDeckBuilder.SetActive(false);
    }

    public void DeleteFromDeck()
    {
       
        //gameManager.UpdateDeck();

        TurretDeckBuilder.SetActive(false);
    }
}
