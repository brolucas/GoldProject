using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    #region Singleton
    private static BuildManager instance = null;

    // Game Instance Singleton
    public static BuildManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BuildManager>();

                return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;

        Debug.Log(instance);

    }
    #endregion

    public truck truck;

    public GameObject turretPrefab;

    public Shop shop;

    public KindOfTurret turretToBuild;

    private GameManager gameManager;

    public void Start()
    {
        if (turretPrefab == null)
        {
            turretPrefab = (GameObject)Resources.Load("Assets/Prefab/TurretPrefab.prefab", typeof(GameObject));
        }

        gameManager = GameManager.Instance;

        // == empty
        turretToBuild = KindOfTurret.DefaultDoNotUseIt;
    }

    public GameObject GetTurretToBuild()
    {
        return turretPrefab;
    }

    public void SetTurretToBuild(KindOfTurret kindOfTurretSelected)
    {
        turretToBuild = kindOfTurretSelected;
    }

    public void CreateTurret(Vector3 position, float cellSize = 0)
    {
        if (turretToBuild == KindOfTurret.DefaultDoNotUseIt)
            return;

        TurretData turretData = gameManager.GetStatsKindOfTurret(turretToBuild);

        if (truck.gold >= turretData.turretPrice)
        {
            if (turretToBuild == KindOfTurret.DefaultDoNotUseIt)
            {
                Debug.LogError("You are trying to create a turret but there is no turret selected !");
                return;
            }

            GameObject newTurret = Instantiate(turretPrefab, position, Quaternion.identity);
            Pathfinding.Instance.GetGrid().GetXY(position, out int x, out int y);
            Pathfinding.Instance.GetNode(x, y).SetIsWalkable(false);

            if (cellSize > 0)
            {
                newTurret.transform.GetChild(1).localScale = new Vector3(cellSize, cellSize, cellSize);
            }

            turretToBuild = KindOfTurret.DefaultDoNotUseIt;

            truck.gold -= turretData.turretPrice;
        }

    }
}
