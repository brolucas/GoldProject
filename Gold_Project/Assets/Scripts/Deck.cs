using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour
{
    // Link the inventory button to a turretType
    public List<Button> inventoryButton = new List<Button>();
    public Dictionary<Button, KindOfTurret> buttonToEnumInventory = new Dictionary<Button, KindOfTurret>();

    // Link the deck slot button to a turretType
    public List<Button> deckButton = new List<Button>();
    public Dictionary<Button, KindOfTurret> buttonToEnumDeck = new Dictionary<Button, KindOfTurret>();

    private Button selectedDeckSlotButton;

    [SerializeField]
    private Image addDeckImage;
    [SerializeField]
    private GameObject removeDeckObj;

    public KindOfTurret turretSelected;

    public Image turretImageUI;

    public Text description;

    public Text descriptionPassive;

    private GameManager gameManager;

    [SerializeField]
    private GameObject TurretDeckAddRemove;

    public void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager.currentScene.name != "MainMenu")
            return;

        for (int i = 0; i < gameManager.turretDatabase.turrets.Count; i++)
        {
            buttonToEnumInventory.Add(inventoryButton[i], gameManager.turretDatabase.turrets[i].kindOfTurret);

            inventoryButton[i].GetComponent<Image>().sprite = gameManager.turretDatabase.turrets[i].UIDesign;
        }
        int temp = PlayerPrefs.GetInt("levelReached");
        int turretCountTemp;
        switch (temp)
        {
            case 2:
                turretCountTemp = 5;
                break;
            case 3:
                turretCountTemp = 6;
                break;
            case 4:
                turretCountTemp = 7;
                break;
            case 5:
                turretCountTemp = 8;
                break;
            case 6:
                turretCountTemp = 9;
                break;
            case 7:
                turretCountTemp = 10;
                break;
            case 8:
                turretCountTemp = 10;
                break;
            case 9:
                turretCountTemp = 10;
                break;
            default:
                turretCountTemp = 4;
                break;

        }
          for (int i = 0; i < gameManager.turretDatabase.turrets.Count; i++)
          {

              inventoryButton[i].interactable = false;
          }
        for (int i = 0; i < turretCountTemp; i++)
        {
            inventoryButton[i].interactable = true;
        }
        
        TurretDeckAddRemove.SetActive(false);
    }

    public void SetSelectedTurretViaInventory(Button thisButton)
    {
        removeDeckObj.SetActive(false);

        KindOfTurret kindOfTurret = buttonToEnumInventory[thisButton];

        SetTurretSelected(thisButton, kindOfTurret);
    }

    public void SetSelectedTurretViaDeck(Button thisButton)
    {
        if (buttonToEnumDeck[thisButton] == KindOfTurret.DefaultDoNotUseIt)
            return;

        removeDeckObj.SetActive(true);

        selectedDeckSlotButton = thisButton;

        KindOfTurret kindOfTurret = buttonToEnumDeck[thisButton];

        SetTurretSelected(thisButton, kindOfTurret);
    }

    public void SetTurretSelected(Button thisButton, KindOfTurret kindOfTurret)
    {
        turretSelected = kindOfTurret;

        if (TurretDeckAddRemove.activeSelf == false)
        {
            TurretDeckAddRemove.SetActive(true);
        }

        TurretData turretData = gameManager.GetStatsKindOfTurret(kindOfTurret);

        description.text = turretData.description;
        descriptionPassive.text = turretData.descriptionPassive;
        turretImageUI.sprite = turretData.UIDesign;

        if (gameManager.dataManager.deckData.deckTurret.Contains(kindOfTurret))
        {
            //addDeckImage.color = Color.red;
            addDeckImage.GetComponentInChildren<Text>().text = "Already in Deck";
            addDeckImage.GetComponentInChildren<Text>().color = Color.red;
        }
        else
        {
            //addDeckImage.color = Color.white;
            addDeckImage.GetComponentInChildren<Text>().text = "Add to Deck";
            addDeckImage.GetComponentInChildren<Text>().color = Color.white;
        }
    }

    public void AddToDeck()
    {
        if (!gameManager.dataManager.deckData.deckTurret.Contains(KindOfTurret.DefaultDoNotUseIt))
            return;

        if (gameManager.dataManager.deckData.deckTurret.Contains(turretSelected))
            //You can't add more than 4 turrets 
            // Already in the deck 
            return;

        gameManager.TryToAddTurretInDeck(turretSelected);

        TurretDeckAddRemove.SetActive(false);
    }

    public void RemoveFromDeck()
    {
        if (!gameManager.dataManager.deckData.deckTurret.Contains(turretSelected))
        {
            Debug.Log("You can't remove this turret because it is not in the deck");
            return;
        }

        gameManager.RemoveTurretFromDeck(turretSelected, selectedDeckSlotButton);

        buttonToEnumDeck[selectedDeckSlotButton] = KindOfTurret.DefaultDoNotUseIt;

        selectedDeckSlotButton.GetComponent<Image>().sprite = null;
        selectedDeckSlotButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        TurretDeckAddRemove.SetActive(false);
    }
}
