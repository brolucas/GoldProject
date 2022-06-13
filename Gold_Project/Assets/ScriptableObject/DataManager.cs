using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //public GameManager gameManager;

    public static DataManager Instance;

    public DeckData deckData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        /*if (gameManager == null)
        {
            gameManager = GameManager.Instance.GetComponent<GameManager>();
        }
        if (gameManager.GetComponent<GameManager>().enabled == false)
        {
            gameManager.GetComponent<GameManager>().enabled = true;
        }*/

        if (deckData == null)
        {
            deckData = GetComponent<DeckData>();
        }
        if (deckData.deckTurret.Count == 0 && deckData.deckTurret.Count < 4)
        {
            deckData.deckTurret.Clear();

            for (int i = 0; i < 4; i++)
            {
                deckData.deckTurret.Add(KindOfTurret.DefaultDoNotUseIt);
            }
        }
    }
}
